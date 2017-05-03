using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ConsolCourse
{
    static class Convertor
    {

        private static string[] bordersG1 = new string[] { "...", "!?", "?!", ".", "!", "?", ")", "»" };
        private static string[] bordersG2 = new string[] { ",", ";", "—", "-", ":", "(", "«"};
        private static string[] conjs = { "a", "но", "и", "да", "или", "либо", "ни", "то", "зато", "однако", "же" };

        public static List<WordModel> convertToWord(string jsonword)
        {
            WordModelFromMyStem wordFromMyStem = new WordModelFromMyStem();
            try
            { 
                wordFromMyStem = JsonConvert.DeserializeObject<WordModelFromMyStem>(jsonword);
            }
            catch
            {
                throw new AbstractException("Ошибка json парсера морфологического анализа.", AbstractExceptionKey.Json);
            }
            List<WordModel> words = new List<WordModel>();
            if (wordFromMyStem.analysis == null || wordFromMyStem.analysis.Count == 0)
            {
                List<WordModel> backstack;
                bool flag = true;
                string text = wordFromMyStem.text;
                text = text.Replace(" ", "~ ~");
                text = text.Replace("\n", "~\n~");
                text = text.Replace("\t", "~\t~");
                string[] borders = text.Split(new char[] { '~' },StringSplitOptions.RemoveEmptyEntries);
                foreach (string currentBorder in borders)
                {
                    if (currentBorder == null || currentBorder == String.Empty) continue;
                    string border = currentBorder;
                    backstack = new List<WordModel>();
                    flag = true;
                    while (flag)
                    {
                        if(border == null || border == String.Empty) break;
                        flag = false;
                        foreach (string borderG1 in bordersG1){
                            if (border.StartsWith(borderG1)) {
                                flag = true;
                                words.Add(new WordModel(borderG1, false, PartsOfLanguage.G1));
                                border = border.Remove(0, borderG1.Length);
                                break;
                            }
                            else if (border.EndsWith(borderG1))
                            {
                                flag = true;
                                backstack.Add(new WordModel(borderG1, false, PartsOfLanguage.G1));
                                border = border.Remove(border.Length - borderG1.Length, borderG1.Length);
                                break;
                            }
                        }
                        if (flag) continue;

                        foreach (string borderG2 in bordersG2)
                        {
                            if (border.StartsWith(borderG2))
                            {
                                flag = true;
                                words.Add(new WordModel(borderG2, false, PartsOfLanguage.G2));
                                border = border.Remove(0, borderG2.Length);
                                break;
                            }
                            else if (border.EndsWith(borderG2))
                            {
                                flag = true;
                                backstack.Add(new WordModel(borderG2, false, PartsOfLanguage.G2));
                                border = border.Remove(border.Length - borderG2.Length, borderG2.Length);
                                break;
                            }
                        }
                        if (flag) continue;

                        if (border == "\n") { words.Add(new WordModel(border, false, PartsOfLanguage.R1)); }
                        else if (border == @"\s") { words.Add(new WordModel(String.Empty, false, PartsOfLanguage.R1)); }
                        else if(isNON(border)) { words.Add(new WordModel(border, false, PartsOfLanguage.NON)); }
                        else { words.Add(new WordModel(border, false, PartsOfLanguage.SPACE)); }
                    }
                    backstack.Reverse();
                    words.AddRange(backstack);
                }
            }
            else
            {
                WordModelFromMyStem.Analysis item = wordFromMyStem.analysis[0]; // Установлено снятие омонимии, возможен только 1 вариант
                PartsOfLanguage part = PartsOfLanguage.NON;
                string word = wordFromMyStem.text;
                string token = item.lex.Replace("ё", "е").ToLowerInvariant();
                List<Gender> genders = null;
                List<Die> dies = null;
                List<Count> counts = null;
                bool important = true;
                bool isA = false;
                bool isBrev = false;
                bool isV = false;
                bool isInf = false;
                bool isGer = false;
                bool isPart = false;
                bool isPers = false;

                string[] morfSign = item.gr.Split('=', ',', '(', ')', '|');

                foreach (string sign in morfSign)
                {
                    if (sign == null || sign == String.Empty) continue;
                    switch (sign)
                    {
                        case "A":
                            {
                                isA = true;
                                if (isA && isBrev) part = PartsOfLanguage.BREVA;
                                else part = PartsOfLanguage.A;
                                important = true;
                                break;
                            }
                        case "ANUM":
                            {
                                part = PartsOfLanguage.A;
                                important = true;
                                break;
                            }
                        case "APRO":
                            {
                                part = PartsOfLanguage.A;
                                important = false;
                                break;
                            }
                        case "NUM":
                        case "ADV":
                            {
                                part = PartsOfLanguage.NON;
                                important = true;
                                break;
                            }
                        case "COM":
                        case "PART":
                        case "ADVPRO":
                        case "INTJ":
                        case "PR":
                            {
                                part = PartsOfLanguage.NON;
                                important = false;
                                break;
                            }
                        case "S":
                            {
                                part = PartsOfLanguage.S;
                                important = true;
                                break;
                            }
                        case "SPRO":
                            {
                                part = PartsOfLanguage.SPRO;
                                important = false;
                                break;
                            }
                        case "V":
                            {
                                isV = true;
                                if (isV && isPers) part = PartsOfLanguage.V;
                                else if (isV && isGer) part = PartsOfLanguage.GER;
                                else if (isV && isInf) part = PartsOfLanguage.INF;
                                else if (isV && isPart) part = PartsOfLanguage.PARTCP;
                                else if (isV && isPart && isBrev) part = PartsOfLanguage.BREVP;
                                important = true;
                                break;
                            }
                        case "CONJ":
                            {
                                if (isCONJS(token))
                                { part = PartsOfLanguage.CONJS; }
                                else
                                { part = PartsOfLanguage.CONJP; }
                                important = false;
                                break;
                            }
                        case "ger":
                            {
                                isGer = true;
                                if (isV && isGer) { part = PartsOfLanguage.GER; important = true; }
                                break;
                            }
                        case "inf":
                            {
                                isInf = true;
                                if (isV && isInf) { part = PartsOfLanguage.INF; important = true; }
                                break;
                            }
                        case "1p":
                        case "2p":
                        case "3p":
                            {
                                isPers = true;
                                if (isV && isPers) { part = PartsOfLanguage.V; important = true;}
                                break;
                            }
                        case "partcp":
                            {
                                isPart = true;
                                if (isV && isPart) { part = PartsOfLanguage.PARTCP; important = true; }
                                else if (isV && isPart && isBrev) {part = PartsOfLanguage.BREVP; important = true;}
                                break;
                            }
                        case "brev":
                            {
                                isBrev = true;
                                if (isA && isBrev) { part = PartsOfLanguage.BREVA; important = true; }
                                else if (isV && isPart && isBrev) { part = PartsOfLanguage.BREVP; important = true; }
                                break;
                            }
                        case "parenth":
                            {
                                part = PartsOfLanguage.PARENTH;
                                important = false;
                                break;
                            }
                        case "praed":
                            {
                                part = PartsOfLanguage.PRAED;
                                important = true;
                                break;
                            }
                        case "nom":
                            {
                                if (dies == null) { dies = new List<Die>(); }
                                if (!dies.Contains(Die.NOM))
                                { dies.Add(Die.NOM); }
                                break;
                            }
                        case "gen":
                            {
                                if (dies == null) { dies = new List<Die>(); }
                                if (!dies.Contains(Die.GEN))
                                { dies.Add(Die.GEN); }
                                break;
                            }
                        case "dat":
                            {
                                if (dies == null) { dies = new List<Die>(); }
                                if (!dies.Contains(Die.DAT))
                                { dies.Add(Die.DAT); }
                                break;
                            }
                        case "acc":
                            {
                                if (dies == null) { dies = new List<Die>(); }
                                if (!dies.Contains(Die.ACC))
                                { dies.Add(Die.ACC); }
                                break;
                            }
                        case "ins":
                            {
                                if (dies == null) { dies = new List<Die>(); }
                                if (!dies.Contains(Die.INS))
                                { dies.Add(Die.INS); }
                                break;
                            }
                        case "abl":
                            {
                                if (dies == null) { dies = new List<Die>(); }
                                if (!dies.Contains(Die.ABL))
                                { dies.Add(Die.ABL); }
                                break;
                            }
                        case "voc":
                            {
                                if (dies == null) { dies = new List<Die>(); }
                                if (!dies.Contains(Die.VOC))
                                { dies.Add(Die.VOC); }
                                break;
                            }
                        case "m":
                            {
                                if (genders == null) { genders = new List<Gender>(); }
                                if (!genders.Contains(Gender.M))
                                { genders.Add(Gender.M); }
                                break;
                            }
                        case "f":
                            {
                                if (genders == null) { genders = new List<Gender>(); }
                                if (!genders.Contains(Gender.F))
                                { genders.Add(Gender.F); }
                                break;
                            }
                        case "n":
                            {
                                if (genders == null) { genders = new List<Gender>(); }
                                if (!genders.Contains(Gender.N))
                                { genders.Add(Gender.N); }
                                break;
                            }
                        case "mf":
                            {
                                if (genders == null) { genders = new List<Gender>(); }
                                if (!genders.Contains(Gender.M))
                                { genders.Add(Gender.M); }
                                if (!genders.Contains(Gender.F))
                                { genders.Add(Gender.F); }
                                break;
                            }
                        case "sg":
                            {
                                if (counts == null) { counts = new List<Count>(); }
                                if (!counts.Contains(Count.SG))
                                { counts.Add(Count.SG); }
                                break;
                            }
                        case "pl":
                            {
                                if (counts == null) { counts = new List<Count>(); }
                                if (!counts.Contains(Count.PL))
                                { counts.Add(Count.PL); }
                                break;
                            }
                    }
                }
                words.Add(new WordModel(word, token, important, part, genders, counts, dies));
            }
            return words;
        }


        private static bool isCONJS(string token)
        {
            foreach (string conj in conjs)
            {
                if (token == conj) return true;
            }
            return false;
        }

        private static bool isNON(string token)
        {
            foreach (char c in token)
            {
                if (!Char.IsLetterOrDigit(c)) return false;
            }
            return true;
        }

        private class WordModelFromMyStem
        {
            [JsonProperty("analysis")]
            public List<Analysis> analysis { get; set; }
            [JsonProperty("text")]
            public string text { get; set; }

            public class Analysis
            {
                [JsonProperty("lex")]
                public string lex { get; set; }
                [JsonProperty("gr")]
                public string gr { get; set; }
            }
        }
    }
}

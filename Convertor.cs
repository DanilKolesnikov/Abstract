using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ConsolCourse
{
    static class Convertor
    {

        private static string[] bordersG1 = new string[] { "...", "!?", "?!", ".)", "!)", "?)", ".", "!", "?", "»" };
        private static string[] bordersG2 = new string[] { ",", ";", "—", "-", ":", "(", ")", "«"};

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
                string token = item.lex.Replace("ё", "е").Replace("-", "").ToLowerInvariant();
                bool important = false;

                string[] morfSign = item.gr.Split('=', ',', '(', ')', '|');
                string[] importantParts = new string[] { "A","AVD","V","S"};

                foreach (string sign in morfSign)
                {
                    if (sign == null || sign == String.Empty) continue;
                    if(Array.IndexOf(importantParts,sign) >= 0) { important = true; break; }
                }
                words.Add(new WordModel(word, token, important, part));
            }
            return words;
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

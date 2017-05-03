using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsolCourse
{
    class Fragment
    {
        private List<WordModel> words;
        private uint leftWord;
        private uint rightWord;
        private bool haveWord;
        private FragmentType type;
        private List<string> leftBorders;
        private List<string> rightBorders;
        private List<WordModel> conj;
        private Fragment neighbor;
        private bool importantNeighbor;
        private Fragment obey;

        public Fragment(List<WordModel> words)
        {
            this.words = new List<WordModel>();
            leftBorders = new List<string>();
            rightBorders = new List<string>();
            conj = new List<WordModel>();
            haveWord = false;
            PartsOfLanguage maxPart = PartsOfLanguage.NON;
            int border = -1;
            for (int i = words.Count - 1; i <= 0; --i)
            {
                if (words[i].part == PartsOfLanguage.R1 || words[i].part == PartsOfLanguage.G1 || words[i].part == PartsOfLanguage.SPACE)
                {
                    if (words[i].word != String.Empty) { rightBorders.Add(words[i].word); }
                }
                else
                {
                    border = i;
                    break;
                }
            }
            rightBorders.Reverse();
            for (int i = 0; i <= border; ++i)
            {
                if (words[i].part == PartsOfLanguage.R1 || words[i].part == PartsOfLanguage.G2 || words[i].part == PartsOfLanguage.SPACE)
                {
                    if (words[i].word != String.Empty) { leftBorders.Add(words[i].word); }
                }
                else
                {
                    if (maxPart < words[i].part) { maxPart = words[i].part; }
                    if (words[i].important)
                    {
                        if (!haveWord)
                        {
                            rightWord = leftWord = words[i].wordNumber;
                            haveWord = true;
                        }
                        else
                        {
                            rightWord = words[i].wordNumber;
                        }
                    }
                    if (words[i].part == PartsOfLanguage.CONJP || words[i].part == PartsOfLanguage.CONJS)
                    {
                        conj.Add(words[i]);
                    }
                    if (words[i].word != String.Empty) { this.words.Add(words[i]); }
                }
                type = convertToFragmrntType(maxPart);
            }
        }

        private static FragmentType convertToFragmrntType(PartsOfLanguage part)
        {
            switch (part)
            {
                case PartsOfLanguage.V: return FragmentType.V;
                case PartsOfLanguage.BREVP: return FragmentType.BP;
                case PartsOfLanguage.BREVA: return FragmentType.BA;
                case PartsOfLanguage.PRAED: return FragmentType.PR;
                case PartsOfLanguage.PARTCP: return FragmentType.P;
                case PartsOfLanguage.GER: return FragmentType.G;
                case PartsOfLanguage.INF: return FragmentType.BA;
                case PartsOfLanguage.PARENTH: return FragmentType.IN;
                default:  return FragmentType.NON;
            }
        }

        public static Fragment Conection(Fragment a, Fragment b)
        {
            
        }

        public static Fragment Obey(Fragment a, Fragment b)
        {
            
        }

        public static Fragment Complication(Fragment a, Fragment b)
        {
            Fragment ans = null;
            if ((ans = Insert(a, b)) != null) { return ans; }
            else { return Concat(a, b); }
        }

        private static Fragment Insert(Fragment a, Fragment b)
        {

        }

        private static Fragment Concat(Fragment a, Fragment b)
        {

        }













        private enum FragmentType
        {
            NON, // пустое 
            IN, //  вводное слово
            I, //   инфинитив
            G, //   деепричастие
            P, //   причастие
            PR, //  прдикатив
            BA, //  краткое прилагательное
            BP, //  краткое причастие
            V //   глагол
        }


    }
}

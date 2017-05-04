using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsolCourse
{
    class Fragment
    {
        private int numberInSentense;
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

        public Fragment(List<WordModel> words, int number)
        {
            this.numberInSentense = number;
            this.words = new List<WordModel>();
            leftBorders = new List<string>();
            rightBorders = new List<string>();
            conj = new List<WordModel>();
            haveWord = false;
            PartsOfLanguage maxPart = PartsOfLanguage.NON;
            int rBorder = -1;
            int lBorder = words.Count;
            for (int i = words.Count - 1; i <= 0; --i)
            {
                if (words[i].part == PartsOfLanguage.R1 || words[i].part == PartsOfLanguage.G1 || words[i].part == PartsOfLanguage.SPACE)
                {
                    if (words[i].word != String.Empty) { rightBorders.Add(words[i].word); }
                }
                else
                {
                    rBorder = i;
                    break;
                }
            }
            rightBorders.Reverse();
            for (int i = 0; i <= rBorder; ++i)
            {
                if (words[i].part == PartsOfLanguage.R1 || words[i].part == PartsOfLanguage.G2 || words[i].part == PartsOfLanguage.SPACE)
                {
                    if (words[i].word != String.Empty) { leftBorders.Add(words[i].word); }
                }
                else
                {
                    lBorder = i;
                    break;
                }
            }
            for (int i = lBorder; i <= rBorder; ++i)
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
                case PartsOfLanguage.S: return FragmentType.S;
                case PartsOfLanguage.SPRO: return FragmentType.S;
                case PartsOfLanguage.A: return FragmentType.A;
                default: return FragmentType.NON;
            }
        }

        public static Fragment useKey(Fragment l, Fragment r, Fragment c)
        {
            
        }

        public static Fragment useMorf(Fragment l, Fragment r, Fragment c)
        {
            Fragment ans = null;
            if ((ans = Insert(l, c, r)) != null) { return ans; }
            else { return Concat(l, c , r); }
        }
        
        private static Fragment Insert(Fragment l, Fragment r, Fragment c)
        {
            if(l.type == FragmentType.G || c.type == FragmentType.G || r.type == FragmentType.G) { return InsertG(l, c, r); }
        }

        private static Fragment InsertG(Fragment l, Fragment r, Fragment c)
        {
            
        }

        private static Fragment Concat(Fragment l, Fragment r, Fragment c)
        {

        }
        
        private static Fragment isContact(Fragment a, Fragment b)
        {

        }










        private enum FragmentType
        {
            NON, // пустое 
            A, //   прилагательное
            S, //   существительное
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

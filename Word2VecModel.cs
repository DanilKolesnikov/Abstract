using System;
using System.IO;

namespace ConsolCourse
{
    class Word2VecModel:IComparable
    {
        public string word;
        public float[] vector;
        private const char space = (char)((int)'а' - 1);

        public Word2VecModel(string word, float[] vector)
        {
            this.word = word;
            this.vector = vector;
        }

        public Word2VecModel() : this(String.Empty, null) { }
        
        public void norm()
        {
            float mod = 0f;
            for (int i = 0; i < 500; i++)
                mod += vector[i]*vector[i];
            mod = (float) Math.Sqrt((float) mod);
            for (int i = 0; i < 500; i++)
                vector[i] /= mod;
        }

        public static Word2VecModel getWord(BinaryReader reader, int maxlenght)
        {
            string word = String.Empty;
            char buf;
            for (int i = 0; i < maxlenght; i++)
            {
                buf = reader.ReadChar();
                if (buf != space)
                {
                    word += buf;
                }
            }
            float[] vector = new float[500];
            for (int i = 0; i < 500; i++)
                vector[i] = reader.ReadSingle();
            return new Word2VecModel(word, vector);
        }

        internal static void setWord(BinaryWriter writer, Word2VecModel word, int maxlenght)
        {
            maxlenght -= word.word.Length;
            int i;
            for ( i = 0; i < word.word.Length; i++)
                writer.Write(word.word[i]);
            for ( i = 0; i < maxlenght; i++)
                writer.Write(space);
            for ( i = 0; i < word.vector.Length; i++)
                writer.Write(word.vector[i]);
        }

        public int CompareTo(object obj)
        {
            return String.Compare(this.word, ((Word2VecModel)obj).word);
        }
    }
}

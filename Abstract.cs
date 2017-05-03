using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConsolCourse
{
    static class Abstract
    {
        private static Dictionary<uint, Topic> stars = new Dictionary<uint, Topic>();

        public static void startAbstract(string input, string output, byte n)
        {
            initProbability(input);
            while (n > 0)
            {
                foreach (Topic t in stars.Values)
                {
                   t.iteration();
                }
                foreach (Topic t in stars.Values)
                {
                    t.normProbability();
                }
                Topic.switchToggle();
                n--;
            }
            writeProbability(input, output);
        }

        private static void initProbability(string input)
        {
            Topic previosTopic = null;
            Topic currentTopic = null;
            uint currentWord, wordCount;
            try
            {
                using (BinaryReader reader = new BinaryReader(new FileStream(input, FileMode.Open), Encoding.UTF8))
                {
                    wordCount = reader.ReadUInt32();
                    for (uint i = 0; i < wordCount; ++i)
                    {
                        currentWord = reader.ReadUInt32();
                        if (!stars.ContainsKey(currentWord))
                        {
                            currentTopic = new Topic(currentWord);
                            stars.Add(currentWord, currentTopic);
                        }
                        else
                        {
                            currentTopic = stars[currentWord];
                        }
                        currentTopic.addNeighbor(previosTopic);
                        previosTopic = currentTopic;
                    }
                    foreach (Topic t in stars.Values)
                    {
                        t.initProbability();
                    }
                }
            }
            catch
            {
                throw new AbstractException("Ошибка поиска ключевых слов.", AbstractExceptionKey.Abstract);
            }
        }

        private static void writeProbability(string input, string output)
        {
            uint currentWord;
            uint wordCount = 1;
            try
            {
                using (BinaryWriter writer = new BinaryWriter(new FileStream(output, FileMode.OpenOrCreate), Encoding.UTF8))
                using (BinaryReader reader = new BinaryReader(new FileStream(input, FileMode.Open), Encoding.UTF8))
                {
                    wordCount = reader.ReadUInt32();
                    for (uint i = 1; i <= wordCount; ++i)
                    {
                        writer.Write(i);
                        currentWord = reader.ReadUInt32();
                        writer.Write(stars[currentWord].ptabability);
                    }
                }
            }
            catch
            {
                throw new AbstractException("Ошибка записи ключевых слов.", AbstractExceptionKey.Abstract);
            }
        }
    }
}

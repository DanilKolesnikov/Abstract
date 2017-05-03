using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ConsolCourse
{
    static class FragmentAnalysis
    {
        private const float border = 0.2f;

        public static void startSegment(string input, string output)
        {
            if (File.Exists(input))
            {
                try
                {
                    using (StreamReader inputStream = new StreamReader(input, Encoding.UTF8))
                    using (BinaryWriter outputStream = new BinaryWriter(new FileStream(output, FileMode.OpenOrCreate), Encoding.UTF8))
                    {
                        uint sentenseLength = 0;
                        uint sentenseWeight = 0;
                        ulong sentenseStart = 0;
                        bool border = false;
                        List<ulong> buf = new List<ulong>();
                        ulong sbuf = 0 ;
                        WordModel.startRead();
                        WordModel currentWord = inputStream.EndOfStream ? null : WordModel.getWord(inputStream);
                        while (!inputStream.EndOfStream)
                        {
                            if (currentWord.part == PartsOfLanguage.G2 || currentWord.part == PartsOfLanguage.CONJS)
                            {
                                buf.Add(WordModel.Line);
                                while ((currentWord.part == PartsOfLanguage.G2 || currentWord.part == PartsOfLanguage.CONJS || currentWord.part == PartsOfLanguage.SPACE || currentWord.part == PartsOfLanguage.R1) && !inputStream.EndOfStream)
                                {
                                    currentWord = WordModel.getWord(inputStream);
                                }
                            }
                            else if (currentWord.part == PartsOfLanguage.G1)
                            {
                                while ((currentWord.part == PartsOfLanguage.G1 || currentWord.part == PartsOfLanguage.SPACE)&& !inputStream.EndOfStream)
                                {
                                    currentWord = WordModel.getWord(inputStream);
                                }
                                if (currentWord.part == PartsOfLanguage.G2) { sbuf = WordModel.Line; }
                                else { sbuf = 0; }
                                while ((currentWord.part == PartsOfLanguage.G2 || currentWord.part == PartsOfLanguage.SPACE) && !inputStream.EndOfStream)
                                {
                                    currentWord = WordModel.getWord(inputStream);
                                }
                                if (currentWord.part == PartsOfLanguage.R1 || isEnd(sentenseLength, sentenseWeight) || border)
                                {
                                    outputStream.Write(sentenseStart);
                                    outputStream.Write(true);
                                    foreach (ulong b in buf)
                                    {
                                        outputStream.Write(b);
                                        outputStream.Write(false);
                                    }
                                    buf.Clear();
                                    sentenseLength = 0;
                                    sentenseWeight = 0;
                                    if (sbuf != 0) { sentenseStart = sbuf; }
                                    else { sentenseStart = WordModel.Line; }
                                }
                                if (sbuf != 0) { buf.Add(sbuf); }
                                if (currentWord.part == PartsOfLanguage.R1) { border = false; }
                                else { border = true; }
                            }
                            else if (currentWord.part != PartsOfLanguage.SPACE && currentWord.part != PartsOfLanguage.R1)
                            {
                                sentenseLength++;
                                if (currentWord.important) { sentenseWeight++; }
                                currentWord = inputStream.EndOfStream ? null : WordModel.getWord(inputStream);
                            }
                            else
                            {
                                currentWord = inputStream.EndOfStream ? null : WordModel.getWord(inputStream);
                            }
                        }
                        outputStream.Write(sentenseStart);
                        outputStream.Write(true);
                        foreach (ulong b in buf)
                        {
                            outputStream.Write(b);
                            outputStream.Write(false);
                        }
                    }
                }
                catch (AbstractException e) { throw new AbstractException(e); }
                catch { throw new AbstractException("Ошибка сегментации текста.", AbstractExceptionKey.File); }
            }
            else
            {
                throw new AbstractException("Отсутствует файл после отчистки текста.", AbstractExceptionKey.File);
            }
        }

        private static bool isEnd(uint length, uint weight)
        {
            return (float)weight / (float)length > border;
        }


        public static void startFragment(string input, string binput, string output)
        {
            if (File.Exists(input))
            {
                try
                {
                    using (StreamWriter outputStream = new StreamWriter(output, false, Encoding.UTF8))//????????????
                    using (StreamReader inputStream = new StreamReader(input, Encoding.UTF8))
                    using (BinaryReader binputStream = new BinaryReader(new FileStream(binput, FileMode.Open), Encoding.UTF8))
                    {
                        WordModel.startRead();
                        WordModel currentWord;
                        Fragment currentFragment;
                        List<WordModel> segment = new List<WordModel>();
                        List<Fragment> sentense = new List<Fragment>();
                        ulong line = binputStream.ReadUInt64();
                        bool isSentenseStart = binputStream.ReadBoolean();
                        while (binputStream.BaseStream.Position != binputStream.BaseStream.Length)
                        {
                            line = binputStream.ReadUInt64();
                            isSentenseStart = binputStream.ReadBoolean();
                            segment.Clear();
                            while (WordModel.Line < line - 1)
                            {
                                currentWord = WordModel.getWord(inputStream);
                                segment.Add(currentWord);
                            }
                            currentFragment = new Fragment(segment);
                            sentense.Add(currentFragment);
                            if (isSentenseStart || binputStream.BaseStream.Position == binputStream.BaseStream.Length)
                            { 

                            }

                            
                        }


                        

                    }
                }
                catch (AbstractException e) { throw new AbstractException(e); }
                catch { throw new AbstractException("Ошибка фрагментации текста.", AbstractExceptionKey.File); }
            }
            else
            {
                throw new AbstractException("Отсутствует файл после отчистки текста.", AbstractExceptionKey.File);
            }
        }



        private static void createFragment(List<Fragment> sentense)
        {
            bool wasUpdate = true;
            List<Fragment> buf = new List<Fragment>();
            Fragment bufFragment = null;
            while (wasUpdate)
            {
                wasUpdate = false;
                for (int i = 0; i < sentense.Count - 1; ++i)
                {
                    if ((bufFragment = Fragment.Conection(sentense[i], sentense[i + 1]) )!= null)
                    {
                        buf.Add(bufFragment);
                        wasUpdate = true;
                    }
                    else if ((bufFragment = Fragment.Obey(sentense[i], sentense[i + 1])) != null)
                    {
                        buf.Add(bufFragment);
                        wasUpdate = true;
                    }
                    else if ((bufFragment = Fragment.Complication(sentense[i], sentense[i + 1])) != null)
                    {
                        buf.Add(bufFragment);
                        wasUpdate = true;
                    }
                    else
                    {
                        buf.Add(sentense[i]);
                    }
                }
                sentense = buf;
                buf.Clear();
                bufFragment = null;

            }
        }









        public static void writeSegment(string input, string binput, string output)
        {
            using (StreamWriter outputStream = new StreamWriter(output,false, Encoding.UTF8))
            using (StreamReader inputStream = new StreamReader(input, Encoding.UTF8))
            using (BinaryReader binputStream = new BinaryReader(new FileStream(binput, FileMode.Open), Encoding.UTF8))
            {
                WordModel.startRead();
                WordModel currentWord;
                ulong line = binputStream.ReadUInt64();
                bool isStart = binputStream.ReadBoolean();
                string sentense;
                while (binputStream.BaseStream.Position != binputStream.BaseStream.Length)
                {
                    line = binputStream.ReadUInt64();
                    isStart = binputStream.ReadBoolean();
                    sentense = String.Empty;
                    while (WordModel.Line < line - 1)
                    {
                        currentWord = WordModel.getWord(inputStream);
                        sentense += currentWord.word;
                    }
                    outputStream.WriteLine("@@@@@@@@@@@@@@@@@@@@");
                    outputStream.WriteLine(sentense);
                }
            }
        }
        
    }
}

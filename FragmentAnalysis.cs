using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ConsolCourse
{
    static class FragmentAnalysis
    {
        private const float percent = 0.2f;

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
                        bool openBracket = false;
                        bool closeBracket = false;
                        bool dash = false;
                        ulong sbuf = 0;
                        WordModel.startRead();
                        WordModel currentWord = inputStream.EndOfStream ? null : WordModel.getWord(inputStream);
                        while (!inputStream.EndOfStream)
                        {
                           if (currentWord.part == PartsOfLanguage.G1)
                            {
                                while ((currentWord.part == PartsOfLanguage.G1 || currentWord.part == PartsOfLanguage.SPACE)&& !inputStream.EndOfStream)
                                {
                                    if (currentWord.token == "»") { closeBracket = openBracket; }
                                    currentWord = WordModel.getWord(inputStream);
                                }
                                if (openBracket && !closeBracket)
                                {
                                    currentWord = WordModel.getWord(inputStream);
                                }
                                else
                                {
                                    if (currentWord.part == PartsOfLanguage.G2 && currentWord.token != ")") { sbuf = WordModel.Line; }
                                    else { sbuf = 0; }
                                    while ((currentWord.part == PartsOfLanguage.G2 || currentWord.part == PartsOfLanguage.SPACE) && !inputStream.EndOfStream)
                                    {
                                        if (currentWord.token == "—" || currentWord.token == "-") { dash = true; }
                                        currentWord = WordModel.getWord(inputStream);
                                    }
                                    if (dash && openBracket && closeBracket)
                                    {
                                        dash = openBracket = closeBracket = false;
                                        currentWord = inputStream.EndOfStream ? null : WordModel.getWord(inputStream);
                                    }
                                    else if (dash && currentWord.part == PartsOfLanguage.NON && Char.IsLower(currentWord.word[0]))
                                    {
                                        dash = false;
                                        currentWord = inputStream.EndOfStream ? null : WordModel.getWord(inputStream);
                                    }
                                    else if (currentWord.part == PartsOfLanguage.R1 || isEnd(sentenseLength, sentenseWeight) || border)
                                    {
                                        outputStream.Write(sentenseStart);
                                        outputStream.Write(true);
                                        sentenseLength = 0;
                                        sentenseWeight = 0;
                                        if (sbuf != 0) { sentenseStart = sbuf; }
                                        else { sentenseStart = WordModel.Line; }
                                    }
                                    if (currentWord.part == PartsOfLanguage.R1) { border = false; }
                                    else { border = true; }
                                }
                            }
                            else if (currentWord.part != PartsOfLanguage.SPACE && currentWord.part != PartsOfLanguage.R1 && currentWord.part != PartsOfLanguage.G2)
                            {
                                sentenseLength++;
                                if (currentWord.important) { sentenseWeight++; }
                                currentWord = inputStream.EndOfStream ? null : WordModel.getWord(inputStream);
                            }
                            else
                            {
                                if (currentWord.token == "«")
                                {
                                    openBracket = true;
                                }
                                currentWord = inputStream.EndOfStream ? null : WordModel.getWord(inputStream);
                            }
                        }
                        outputStream.Write(sentenseStart);
                        outputStream.Write(true);
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
            return (float)weight / (float)length > percent;
        }


        public static void startFragment(string input, string binput, string winput, string output)
        {
            if (File.Exists(input))
            {
                try
                {
                    using (StreamWriter outputStream = new StreamWriter(output, false, Encoding.UTF8))
                    using (StreamReader inputStream = new StreamReader(input, Encoding.UTF8))
                    using (BinaryReader binputStream = new BinaryReader(new FileStream(binput, FileMode.Open), Encoding.UTF8))
                    using (BinaryReader winputStream = new BinaryReader(new FileStream(winput, FileMode.Open), Encoding.UTF8))
                    {
                        WordModel.startRead();
                        WordModel currentWord;
                        string sentence = String.Empty;
                        uint sentenseLength = 0;
                        float sentenseWeight = 0f;
                        uint wordNumber = winputStream.ReadUInt32();
                        float wordWeight = winputStream.ReadSingle();
                        ulong line = binputStream.ReadUInt64();
                        bool isSentenseStart = binputStream.ReadBoolean();
                        while (binputStream.BaseStream.Position != binputStream.BaseStream.Length)
                        {
                            line = binputStream.ReadUInt64();
                            isSentenseStart = binputStream.ReadBoolean();
                            sentence = String.Empty;
                            sentenseLength = 0;
                            sentenseWeight = 0f;
                            while (WordModel.Line < line - 1 && !inputStream.EndOfStream)
                            {
                                currentWord = WordModel.getWord(inputStream);
                                if (currentWord.important && currentWord.wordNumber == wordNumber)
                                {
                                    if (winputStream.BaseStream.Position != winputStream.BaseStream.Length)
                                    {
                                        sentenseLength++;
                                        sentenseWeight += wordWeight * wordWeight;
                                        wordNumber = winputStream.ReadUInt32();
                                        wordWeight = winputStream.ReadSingle();
                                    }
                                }
                                sentence += currentWord.word;
                            }
                            outputStream.WriteLine(sentence + " - " + sentenseWeight / sentenseLength);

                        }
                    }
                }
                catch (AbstractException e) { throw new AbstractException(e); }
                catch (Exception e){ throw new AbstractException("Ошибка фрагментации текста.", AbstractExceptionKey.File); }
            }
            else
            {
                throw new AbstractException("Отсутствует файл после отчистки текста.", AbstractExceptionKey.File);
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

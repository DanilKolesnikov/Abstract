using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System;
using System.Threading.Tasks;

namespace ConsolCourse
{
    static class MorphAnalysis
    {
        private const uint MaxWordCount = 10000;
        
        private static List<ItemMorphAnalysis> files;

        public static void startAnalysis(string inputpath, string input, string output)
        {
            if (File.Exists(inputpath))
            {
                Encoding inputpathEncoding = detectTextEncoding(inputpath);
                File.WriteAllText(input,"~");
                File.AppendAllText(input, File.ReadAllText(inputpath, inputpathEncoding), Encoding.UTF8);
            }
            else
            {
                throw new AbstractException("Не удалось найти файл.", AbstractExceptionKey.File);
            }
            files = createtItemAnalysis(input, output);
            Parallel.ForEach<ItemMorphAnalysis>(files, x => x.startItemAnalysis());
            sewItemAnalysis(files, output);
            deleteItemAnalysis(files);
            files = null;
        }

        private static void deleteItemAnalysis(List<ItemMorphAnalysis> outfiles)
        {
            foreach (ItemMorphAnalysis item in outfiles)
            {
                File.Delete(item.output);
                File.Delete(item.input);
            }
        }

        private static List<ItemMorphAnalysis> createtItemAnalysis(string input, string output)
        {
            List <ItemMorphAnalysis> answer  = new List<ItemMorphAnalysis>();
            int index = 0;
            uint wordNumber;
            bool border;
            StringBuilder itemText = new StringBuilder();
            try
            {
                using (StreamReader inputFile = new StreamReader(input))
                {
                    while (!inputFile.EndOfStream)
                    {
                        wordNumber = 0;
                        border = true;
                        itemText.Clear();
                        char currentSymbol;
                        while (!inputFile.EndOfStream)
                        {
                            currentSymbol = (char)inputFile.Peek();
                            if (isLetter(currentSymbol))
                            {
                                border = false;
                                itemText.Append((char)inputFile.Read());
                            }
                            else
                            {
                                if (!border) { wordNumber++; }
                                border = true;
                                if (wordNumber < MaxWordCount) { itemText.Append((char)inputFile.Read()); }
                                else { break; }
                            }
                        }
                        File.WriteAllText("." + index.ToString() + input, itemText.ToString(), Encoding.UTF8);
                        answer.Add(new ItemMorphAnalysis("." + index.ToString() + input, "." + index.ToString() + output));
                        index++;
                    }
                }
            }
            catch
            {
                throw new AbstractException("Ошибка деления файлов.", AbstractExceptionKey.File);
            }
            return answer;
        }

        private static void sewItemAnalysis(List<ItemMorphAnalysis> outfiles, string output)
        {
            if (File.Exists(output)) { File.Delete(output); }
            foreach (ItemMorphAnalysis item in outfiles)
            {
                if (File.Exists(item.output))
                {
                    File.AppendAllText(output, File.ReadAllText(item.output));
                }
                else
                {
                    throw new AbstractException("Потерян файл после морфологического анализа.", AbstractExceptionKey.File);
                }
            }
        }

        private static bool isLetter(char c)
        {
            return ((c <= 'я') && (c >= 'а')) || ((c <= 'Я') && (c >= 'А'));
        }

        public static Encoding detectTextEncoding(string filename)
        {
            byte[] byteFile = new byte[4];
            try { 
                using (FileStream file = new FileStream(filename, FileMode.Open))
                {
                    file.Read(byteFile, 0, 4);
                }
            }
            catch
            {
                throw new AbstractException("Не удалось установить кодировку файла.", AbstractExceptionKey.File);
            }
            
            if (byteFile.Length >= 4 && byteFile[0] == 0x00 && byteFile[1] == 0x00 && byteFile[2] == 0xFE && byteFile[3] == 0xFF)
            {
                return Encoding.GetEncoding("utf-32BE");
            } // UTF-32, big-endian 
            else if (byteFile.Length >= 4 && byteFile[0] == 0xFF && byteFile[1] == 0xFE && byteFile[2] == 0x00 && byteFile[3] == 0x00)
            {
                return Encoding.UTF32;
            } // UTF-32, little-endian
            else if (byteFile.Length >= 2 && byteFile[0] == 0xFE && byteFile[1] == 0xFF)
            {
                return Encoding.BigEndianUnicode;
            } // UTF-16, big-endian
            else if (byteFile.Length >= 2 && byteFile[0] == 0xFF && byteFile[1] == 0xFE)
            {
                return Encoding.Unicode;
            } // UTF-16, little-endian
            else if (byteFile.Length >= 3 && byteFile[0] == 0xEF && byteFile[1] == 0xBB && byteFile[2] == 0xBF)
            {
                return Encoding.UTF8;
            } // UTF-8
            else if (byteFile.Length >= 3 && byteFile[0] == 0x2b && byteFile[1] == 0x2f && byteFile[2] == 0x76)
            {
                return Encoding.UTF7;
            } // UTF-7
            
            throw new AbstractException("Не удалось установить кодировку файла.", AbstractExceptionKey.File);
        }
        
        public class ItemMorphAnalysis
        {
            private const string PROCNAME = "mystem.exe";
            private const string PROCPARAMS = "-ncgdis -eutf-8 --eng-gr --format json";
            public readonly string input;
            public readonly string output;

            public StreamReader errorStream;

            public ItemMorphAnalysis(string input, string output)
            {
                this.input = input;
                this.output = output;
            }

            public void startItemAnalysis()
            {
                Process analysis = new Process();
                analysis.StartInfo.UseShellExecute = false;
                analysis.StartInfo.RedirectStandardOutput = true;
                analysis.StartInfo.CreateNoWindow = true;
                analysis.StartInfo.ErrorDialog = false;
                analysis.StartInfo.RedirectStandardError = true;

                analysis.StartInfo.FileName = PROCNAME;
                analysis.StartInfo.Arguments = PROCPARAMS + " " + input + " " + output;

                analysis.Start();
                errorStream = analysis.StandardError;
                analysis.WaitForExit();
                analysis.Close();
                while (!errorStream.EndOfStream)
                {
                    Console.WriteLine(errorStream.ReadLine());
                    throw new AbstractException("Ошибка морфологического анализа.", AbstractExceptionKey.MorphAnalysis);
                }
            }
        }
    }
}

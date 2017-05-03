using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConsolCourse
{
    static class TextCleaner
    {
        private const float eps = 0.55f;
        private const string DirectoryName = "Vectors";
        private static uint wordNumber = 0;
        private static BorTree tree;
        private static DSU sets;

        public static void startClear(string input, string frgmentoutput, string output, bool isHard )
        {
            tree = new BorTree();
            sets = new DSU();
            startEasyClear(input, frgmentoutput, output);
            if (isHard)
            {
                startHardClear(output);
            }
            writeClearWords(output);
        }

        public static void startEasyClear(string input, string fragmentoutput, string output)
        {
            List<WordModel> currentWords;
            uint treeNumber;
           
            if (File.Exists(input))
            {
                try
                {
                    using (BinaryWriter outputStream = new BinaryWriter(new FileStream(output, FileMode.OpenOrCreate),Encoding.UTF8))
                    using (StreamReader inputStream = new StreamReader(input, Encoding.UTF8))
                    using (StreamWriter fragmentoutputStream = new StreamWriter(fragmentoutput,false, Encoding.UTF8))
                    {
                        while (!inputStream.EndOfStream)
                        {
                            currentWords = Convertor.convertToWord(inputStream.ReadLine());
                            foreach (WordModel currentWord in currentWords)
                            {
                                if (currentWord.important)
                                {
                                    AbstractModel.Model.words.Add(currentWord.token); ///////
                                    sets.add(wordNumber);
                                    treeNumber = tree.getWordNumber(currentWord.token, wordNumber);
                                    if(treeNumber != wordNumber)
                                    {
                                        sets.union(treeNumber, wordNumber);
                                    }
                                    else
                                    {
                                        outputStream.Write(wordNumber);
                                        outputStream.Write(currentWord.token);
                                    }
                                    wordNumber++;
                                    currentWord.wordNumber = wordNumber;
                                }
                                else
                                {
                                    currentWord.wordNumber = WordModel.NotImportant;
                                }
                                WordModel.setWord(fragmentoutputStream, currentWord);
                            }
                        }
                    }
                }
                catch (AbstractException e) { throw new AbstractException(e); }
                catch (Exception e) { Console.WriteLine(e.Message); throw new AbstractException("Ошибка отчистки текста.", AbstractExceptionKey.File); }
            }
            else
            {
                throw new AbstractException("Отсутствует файл после морфологического анализа.", AbstractExceptionKey.File);
            }
        }
        
        
        private static void initWord2VecModel(List<Tuple<string, uint>> hashGroup, int hash, float[][] vectors, SortedList<uint, List<Edge>> cayleyTable)
        {
            string path = System.IO.Path.Combine(DirectoryName, "hashsort" + hash.ToString() + ".bin");
            if (File.Exists(path) && hashGroup != null)
            {
                Word2VecModel currentWord;
                Tuple<string, uint> findedWord;
                int lenght;
                int maxlenght;
                long delta, startPosition, position;
                int borderLeft, borderRight, mid;
                int comp;
                hashGroup.Sort();
                try
                {
                    using (BinaryReader reader = new BinaryReader(new FileStream(path, FileMode.Open), Encoding.UTF8))
                    {
                        lenght = reader.ReadInt32();
                        maxlenght = reader.ReadInt32();
                        delta = maxlenght * 2 + 2000;
                        borderLeft = 0;
                        startPosition = reader.BaseStream.Position;
                        foreach (Tuple<string, uint> word in hashGroup)
                        {
                            borderRight = lenght;
                            while (borderLeft + 1 != borderRight)
                            {
                                mid = (borderRight + borderLeft) / 2;
                                position = startPosition + delta * mid;
                                reader.BaseStream.Seek(position, SeekOrigin.Begin);
                                currentWord = Word2VecModel.getWord(reader, maxlenght);
                                comp = String.Compare(word.Item1, currentWord.word);
                                if (comp == 0)
                                {
                                    vectors[word.Item2] = currentWord.vector;
                                    cayleyTable.Add(word.Item2, new List<Edge>());
                                    findedWord = word;
                                    borderLeft = mid;
                                    break;
                                }
                                else if (comp > 0)
                                {
                                    borderLeft = mid;
                                }
                                else
                                {
                                    borderRight = mid;
                                }
                            } 
                        }
                    }
                }
                catch { }
            }
        }

        public static void startHardClear(string input)
        {
            float[][] vectors = new float[wordNumber] [];
            SortedList<uint,List<Edge>> cayleyTable = new SortedList<uint, List<Edge>>();
            List<Edge> edges = new List<Edge>();
            List<Tuple<string, uint>>[] hashGroups = new List<Tuple<string, uint>>[1009];
            try
            {
                using (BinaryReader reader = new BinaryReader(new FileStream(input, FileMode.Open), Encoding.UTF8))
                {
                    int currentHash;
                    string currentWord;
                    uint currentNumber;
                    while(reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        currentNumber = reader.ReadUInt32();
                        currentWord = reader.ReadString();
                        currentHash = Hash.hashFunc(currentWord);
                        if(hashGroups[currentHash] == null) { hashGroups[currentHash] = new List<Tuple<string, uint>>(); }
                        hashGroups[currentHash].Add(new Tuple<string, uint>(currentWord, currentNumber));
                    }
                }

                for (int i = 0; i < hashGroups.Length; ++i)
                {
                    initWord2VecModel(hashGroups[i], i, vectors, cayleyTable);
                }

                float currentMult = 0f;
                Edge currentEdge;
                for (uint i = 0; i < wordNumber; ++i)
                {
                    for (uint j = i + 1 ; j < wordNumber; ++j)
                    {
                        currentMult = vecMult(vectors[i], vectors[j]);
                        if (currentMult >= eps) {
                            currentEdge = new Edge(currentMult, i, j);
                            edges.Add(currentEdge);
                            cayleyTable[i].Add(currentEdge);
                            cayleyTable[j].Add(currentEdge);
                        }
                    }
                }

                edges.Sort(delegate (Edge x, Edge y)
                {
                    if (x == null && y == null) { return 0; }
                    else if (x == null) { return -1; }
                    else if (y == null) { return 1; }
                    else return x.CompareTo(y);
                });


                while (edges.Count > 0)
                {
                    currentEdge = edges[edges.Count - 1];
                    edges.RemoveAt(edges.Count - 1);
                    uint i = currentEdge.i, j = currentEdge.j;
                    uint leader = sets.union(i, j);
                    int ci = 0, cj = 0;
                    List<Edge> iEdges = cayleyTable[i];
                    List<Edge> jEdges = cayleyTable[j];
                    List<Edge> leaderEdges = new List<Edge>();
                    Edge currentIEdge;
                    Edge currentJEdge;
                    uint kI, kJ;
                    while ((ci < iEdges.Count) && (cj < jEdges.Count))
                    {
                        currentIEdge = iEdges[ci];
                        currentJEdge = jEdges[cj];
                        kI = currentIEdge.getSecond(i);
                        kJ = currentJEdge.getSecond(j);
                        if ( kI == kJ)
                        {
                            if (currentIEdge.weight > currentJEdge.weight)
                            {
                                iEdges.Remove(currentIEdge);
                                cayleyTable[kI].Remove(currentIEdge);
                                edges.Remove(currentIEdge);
                                if (leader == i) { currentJEdge.setSecond(kJ, leader); }
                                leaderEdges.Add(currentJEdge);
                                cj++;
                            }
                            else if (currentIEdge.weight < currentJEdge.weight)
                            {
                                jEdges.Remove(currentJEdge);
                                cayleyTable[kJ].Remove(currentJEdge);
                                edges.Remove(currentJEdge);
                                if (leader == j) { currentIEdge.setSecond(kI, leader); }
                                leaderEdges.Add(currentIEdge);
                                ci++;
                            }
                            else
                            {
                                if(leader == i)
                                {
                                    jEdges.Remove(currentJEdge);
                                    cayleyTable[kJ].Remove(currentJEdge);
                                    edges.Remove(currentJEdge);
                                    leaderEdges.Add(currentIEdge);
                                    ci++;
                                }
                                else
                                {
                                    iEdges.Remove(currentIEdge);
                                    cayleyTable[kI].Remove(currentIEdge);
                                    edges.Remove(currentIEdge);
                                    leaderEdges.Add(currentJEdge);
                                    cj++;
                                }
                            }
                        }
                        else if (kI > kJ)
                        {
                            jEdges.Remove(currentJEdge);
                            cayleyTable[kJ].Remove(currentJEdge);
                            edges.Remove(currentJEdge);
                        }
                        else
                        {
                            iEdges.Remove(currentIEdge);
                            cayleyTable[kI].Remove(currentIEdge);
                            edges.Remove(currentIEdge);
                        }
                    }
                    while (ci < iEdges.Count)
                    {
                        currentIEdge = iEdges[ci];
                        kI = currentIEdge.getSecond(i);
                        iEdges.Remove(currentIEdge);
                        cayleyTable[kI].Remove(currentIEdge);
                        edges.Remove(currentIEdge);
                    }
                    while (cj < jEdges.Count)
                    {
                        currentJEdge = jEdges[cj];
                        kJ = currentJEdge.getSecond(j);
                        jEdges.Remove(currentJEdge);
                        cayleyTable[kJ].Remove(currentJEdge);
                        edges.Remove(currentJEdge);
                    }
                    if (leader == i) { cayleyTable[i] = leaderEdges; cayleyTable[j] = null; }
                    else { cayleyTable[j] = leaderEdges; cayleyTable[i] = null; }
                }
            }
            catch (Exception e) { throw new AbstractException("Ошибка конденсации слов.", AbstractExceptionKey.Clear); }
        }

        private static float vecMult(float[] a, float[] b)
        {
            float ans = 0;
            if ((a != null) && (b != null))
            { 
                int n = Math.Min(a.Length, b.Length); // должно быть 500 всегда
                for (int i = 0; i < n; ++i) { ans += a[i] * b[i] ; }
            }
            return ans;
        }

        private static void writeClearWords(string output)
        {
            using (BinaryWriter writer = new BinaryWriter(new FileStream (output, FileMode.OpenOrCreate), Encoding.UTF8))
            {
                writer.Write(wordNumber);
                for (uint i = 0; i < wordNumber; ++i)
                {
                    writer.Write(sets.get(i) + 1);
                }
            }
        }
    }
}

using System;
using Newtonsoft.Json;
using System.IO;

namespace ConsolCourse
{
    class WordModel
    {
        public static ulong Line { get; private set; }

        public const uint NotImportant = 0;
        [JsonProperty("wordNumber")]
        public uint wordNumber { get; set; }
        [JsonProperty("word")]
        public string word { get; set; }
        [JsonProperty("token")]
        public string token { get; set; }
        [JsonProperty("important")]
        public bool important { get; set; }
        [JsonProperty("part")]
        public PartsOfLanguage part { get; set; }

        public WordModel (string word , 
                          string token, 
                          bool important, 
                          PartsOfLanguage part)
        { 
            this.word = word;
            this.token = token;
            this.important = important;
            this.part = part;
        }

        public WordModel(string word, bool important, PartsOfLanguage part) :
            this(word, word, important, part) { }

        public WordModel() :
            this(String.Empty, String.Empty, false, PartsOfLanguage.NON)
        { }

        public static void startRead()
        {
            Line = 0;
        }

        public static WordModel getWord(StreamReader reader)
        {
            try
            {
                Line++;
                return JsonConvert.DeserializeObject<WordModel>(reader.ReadLine());
            }
            catch
            {
                throw new AbstractException("Ошибка json парсера при считывании слова.", AbstractExceptionKey.Json);
            }
        }

        public static void setWord(StreamWriter writer, WordModel word)
        {
            try
            {
                writer.WriteLine(JsonConvert.SerializeObject(word));
            }
            catch
            {
                throw new AbstractException("Ошибка json парсера при записи слова.", AbstractExceptionKey.Json);
            }
        }

    }
    public enum PartsOfLanguage
    {
        NON, //     пустое 
        G1, //      символ разделитель предложений
        G2, //      символ разделитель внутри предложений
        SPACE, //   пробел
        R1, //      100% символ разделитель предложений
    }
}

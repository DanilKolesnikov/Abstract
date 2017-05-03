using System;
using Newtonsoft.Json;
using System.Collections.Generic;
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
        [JsonProperty("genders")]
        private List <Gender> genders { get; set; }
        [JsonProperty("counts")]
        private List<Count> counts { get; set; }
        [JsonProperty("dies")]
        private List<Die> dies { get; set; }

        public WordModel (string word , 
                          string token, 
                          bool important, 
                          PartsOfLanguage part, 
                          List<Gender> genders, 
                          List<Count> counts, 
                          List<Die> dies){
            this.word = word;
            this.token = token;
            this.important = important;
            this.part = part;
            this.genders = genders;
            this.counts = counts;
            this.dies = dies;
        }

        public WordModel(string word, bool important, PartsOfLanguage part) :
            this(word, word, important, part, null, null, null){ }

        public WordModel(string word, string token, bool important, PartsOfLanguage part) :
            this(word, token, important, part, null, null, null){ }

        public WordModel() :
            this(String.Empty, String.Empty, false, PartsOfLanguage.NON, null, null, null)
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
        A, //       прилагательное
        CONJP, //   союз подчинительный 
        CONJS, //   союз сочинительный
        S, //       существительное
        SPRO, //    местоимение-существительное
        G1, //      символ разделитель предложений
        G2, //      символ разделитель внутри предложений
        SPACE, //   пробел
        R1, //      100% символ разделитель предложений
        PARENTH, // вводное слово
        INF, //     инфинитив
        GER, //     деепричастие
        PARTCP, //  причастие
        PRAED, //   прдикатив
        BREVA, //   краткое прилагательное
        BREVP, //   краткое причастие
        V  //       глагол
    }

    public enum Gender
    {
        M, F, N
    }

    public enum Die
    {
        NOM, //     именительный
        GEN, //     родительный
        DAT, //     дательный
        ACC, //     винительный
        INS, //     творительный
        ABL, //     предложный
        VOC, //     звательный
    }

    public enum Count
    {
        SG, PL
    }
}

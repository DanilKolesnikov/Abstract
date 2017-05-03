namespace ConsolCourse
{
    class BorTree
    {
        private const byte AlphabetLenght = 'я' - 'а' + 1;
        private const int FirstLetter = (int) 'а';

        private BorNode rootBor;

        public BorTree()
        {
            rootBor = new BorNode();
        }

        public uint getWordNumber(string word, uint number)
        {
            BorNode currentNode = rootBor;
            for(int i = 0; i < word.Length; i++)
            {
                currentNode = currentNode.checkout(word[i]);
            }
            if (currentNode.isNotEnd)
            {
                currentNode.wordNumber = number;
                currentNode.isNotEnd = false;
            }
            return currentNode.wordNumber;
        }

        private class BorNode
        {
            public uint wordNumber;
            public bool isNotEnd = true;
            private BorNode[] nextNodes = new BorNode[AlphabetLenght];

            public BorNode checkout(char nextSymbol)
            {
                int index;
                //if ('ё' == nextSymbol) { index = (int)'е' - FirstLetter; }
                index = (int)nextSymbol - FirstLetter;
                if (nextNodes[index] == null)
                {
                    nextNodes[index] = new BorNode();
                }
                return nextNodes[index];
            }
        }
    }
}

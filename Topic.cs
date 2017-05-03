using System.Collections.Generic;

namespace ConsolCourse
{
    class Topic
    {
        public uint word;
        private float l1;
        private float l2;
        private float r1;
        private float r2;
        private List<Neighbor> rightNeighbors;
        private List<Neighbor> leftNeighbors;
        private static bool toggle = true;
        private static float leftSum = 0f;
        private static float rightSum = 0f;
        public Topic(uint word)
        {
            this.word = word;
            leftNeighbors = new List<Neighbor>();
            rightNeighbors = new List<Neighbor>();
            r1 = 0f;
            r2 = 0f;
            l1 = 0f;
            l2 = 0f;
        }

        private float lastLeftIter {
            get { return toggle ? l1 : l2; }
            set
            {
                if (toggle) { l1 = value; }
                else { l2 = value; }
            }
        }
        private float nextLeftIter
        {
            get { return toggle ? l2 : l1; }
            set
            {
                if (toggle) { l2 = value; }
                else { l1 = value; }
            }
        }

        private float lastRightIter
        {
            get { return toggle ? r1 : r2; }
            set
            {
                if (toggle) { r1 = value; }
                else { r2 = value; }
            }
        }
        private float nextRightIter
        {
            get { return toggle ? r2 : r1; }
            set
            {
                if (toggle) { r2 = value; }
                else { r1 = value; }
            }
        }

        public float ptabability
        {
            get { return toggle ? (r1 + l1) / 2 : (r2 + l2) / 2; }
        }

        public void addNeighbor(Topic neighbor)
        {
            lastLeftIter++; leftSum++;
            lastRightIter++; rightSum++;
            if (neighbor == null || neighbor.word == word) return;
            bool haveNeighbor = false;
            for (int i = 0; i < leftNeighbors.Count; ++i)
            {
                if (leftNeighbors[i].neighbor.word == neighbor.word)
                {
                    leftNeighbors[i].leftProbability++;
                    leftNeighbors[i].rightProbability++;
                    haveNeighbor = true;
                    break;
                }
            }
            if (!haveNeighbor) {
                Neighbor currentNeighbor = new Neighbor(neighbor);
                neighbor.rightNeighbors.Add(currentNeighbor);
                leftNeighbors.Add(currentNeighbor);
            }
        }

        public void initProbability()
        {
            lastLeftIter /= leftSum;
            lastRightIter /= rightSum;
            float buf = 0f;
            int i;
            for ( i = 0; i < rightNeighbors.Count; ++i)
            {
                buf += rightNeighbors[i].rightProbability;
            }
            for ( i = 0; i < rightNeighbors.Count; ++i)
            {
                rightNeighbors[i].rightProbability /= buf;
            }
            buf = 0f;
            for ( i = 0; i < leftNeighbors.Count; ++i)
            {
                buf += leftNeighbors[i].leftProbability;
            }
            for ( i = 0; i < leftNeighbors.Count; ++i)
            {
                leftNeighbors[i].leftProbability /= buf;
            }
        }

        public void iteration()
        {
            nextRightIter = 0f;
            nextLeftIter = 0f;
            for (int i = 0; i < leftNeighbors.Count; ++i)
            {
                nextLeftIter += leftNeighbors[i].leftProbability * leftNeighbors[i].neighbor.lastLeftIter;
            }
            leftSum += nextLeftIter;
            for (int i = 0; i < rightNeighbors.Count; ++i)
            {
                nextRightIter += rightNeighbors[i].rightProbability * rightNeighbors[i].neighbor.lastRightIter;
            }
            rightSum += nextRightIter;

        }

        public void normProbability()
        {
            nextRightIter /= rightSum;
            nextLeftIter /= leftSum;
        }

        public static void switchToggle()
        {
            leftSum = 0f;
            rightSum = 0f;
            toggle = !toggle;
        }
        
        private class Neighbor
        {
            public float leftProbability;
            public float rightProbability;
            public Topic neighbor;

            public Neighbor(Topic neighbor)
            {
                this.leftProbability = 1f;
                this.rightProbability = 1f;
                this.neighbor = neighbor;
            }
        }
    }
}

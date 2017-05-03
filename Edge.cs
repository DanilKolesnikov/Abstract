using System;

namespace ConsolCourse
{
    class Edge : IComparable
    {
        public uint i;
        public uint j;
        public float weight;
        public Edge (float weight, uint i, uint j){
            this.weight = weight;
            this.i = i;
            this.j = j;
        }

        public uint getSecond(uint a)
        {
            return i == a ? j : i;
        }

        public void setSecond(uint a, uint newb)
        {
            if (i == a) { j = newb; }
            else { i = newb; }
        }

        public int CompareTo(object obj)
        {
            return weight.CompareTo(((Edge)obj).weight);
        }
    }
}

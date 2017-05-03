using System;
using System.Collections.Generic;

namespace ConsolCourse
{
    class DSU
    {
        private Dictionary<uint,Noode> items;
        
        public DSU()
        {
            items = new Dictionary<uint, Noode>();
        }

        public void add(uint i)
        {
            if (items.ContainsKey(i)) return;
            items.Add(i, new Noode(null, 0, i));
        }

        public uint get (uint i)
        {
            if (!items.ContainsKey(i)) { add(i); }
            Noode root = items[i];
            while (root.parent != null)
            {
                root = root.parent;
            }
            Noode x = items[i];
            Noode j;
            while (x.parent != null)
            {
                j = x.parent;
                x.parent = root;
                x = j;
            }
            return root.number;
        }

        public uint union (uint i , uint j)
        {
            i = get(i);
            j = get(j);
            Noode ni = items[i], nj = items[j];
            if (i == j) { return i; }
            else if (ni.rank == nj.rank)
            {
                ni.rank++;
                nj.parent = ni;
                return i;
            }
            else if (ni.rank < nj.rank)
            {
                ni.parent = nj; ;
                return j;
            }
            else
            {
                nj.parent = ni;
                return i;
            }
        }

        private class Noode
        {
            public Noode parent;
            public uint rank;
            public uint number;

            public Noode(Noode parent, uint rank, uint number)
            {
                this.parent = parent;
                this.rank = rank;
                this.number = number;
            }
        }
    }
}

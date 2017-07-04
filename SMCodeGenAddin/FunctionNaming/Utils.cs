using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARSourceGeneration
{
    public class ComparableSet<T> : SortedSet<T>, IComparable<SortedSet<T>> where T : IComparable
    {
        public int CompareTo(SortedSet<T> other)
        {
            int ret = 0;
            if (other.Count != this.Count)
            {
                ret = -1;
            }
            else
            {
                var et = other.GetEnumerator();
                var etThis = this.GetEnumerator();
                while (et.MoveNext() && etThis.MoveNext())
                {
                    if ((ret = etThis.Current.CompareTo(et.Current)) != 0)
                    {
                        break;
                    }
                }
            }
            return ret;
        }
    }

}

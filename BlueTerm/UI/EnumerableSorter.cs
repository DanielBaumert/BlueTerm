using System;
using System.Collections.Generic;

namespace BlueTerm.UI
{
    public abstract class EnumerableSorter<TElement>
    {
        internal abstract void ComputeKeys(TElement[] elements, int count);

        internal abstract int CompareKeys(int index1, int index2);

        internal int[] Sort(TElement[] elements, int count)
        {
            ComputeKeys(elements, count);
            int[] map = new int[count];
            for (int i = 0; i < count; i++) map[i] = i;
            QuickSort(map, 0, count - 1);
            return map;
        }

        void QuickSort(int[] map, int left, int right)
        {
            do
            {
                int i = left;
                int j = right;
                int x = map[i + ((j - i) >> 1)];
                do
                {
                    while (i < map.Length && CompareKeys(x, map[i]) > 0) i++;
                    while (j >= 0 && CompareKeys(x, map[j]) < 0) j--;
                    if (i > j) break;
                    if (i < j)
                    {
                        int temp = map[i];
                        map[i] = map[j];
                        map[j] = temp;
                    }
                    i++;
                    j--;
                } while (i <= j);
                if (j - left <= right - i)
                {
                    if (left < j) QuickSort(map, left, j);
                    left = i;
                }
                else
                {
                    if (i < right) QuickSort(map, i, right);
                    right = j;
                }
            } while (left < right);
        }
    }

    public class EnumerableSorter<TElement, TKey> : EnumerableSorter<TElement>
    {
        internal Func<TElement, TKey> keySelector;
        internal IComparer<TKey> comparer;
        internal bool descending;
        internal EnumerableSorter<TElement> next;
        internal TKey[] keys;

        internal EnumerableSorter(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending, EnumerableSorter<TElement> next)
        {
            this.keySelector = keySelector;
            this.comparer = comparer;
            this.descending = descending;
            this.next = next;
        }

        internal override void ComputeKeys(TElement[] elements, int count)
        {
            keys = new TKey[count];
            for (int i = 0; i < count; i++) keys[i] = keySelector(elements[i]);
            if (next != null) next.ComputeKeys(elements, count);
        }

        internal override int CompareKeys(int index1, int index2)
        {
            int c = comparer.Compare(keys[index1], keys[index2]);
            if (c == 0)
            {
                if (next == null) return index1 - index2;
                return next.CompareKeys(index1, index2);
            }
            return descending ? -c : c;
        }
    }

}

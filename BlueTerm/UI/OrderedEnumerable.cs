using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueTerm.UI
{
    public class OrderedEnumerable<TElement, TKey> : OrderedEnumerable<TElement>
    {
        internal OrderedEnumerable<TElement> parent;
        internal Func<TElement, TKey> keySelector;
        internal IComparer<TKey> comparer;
        internal bool descending;

        internal OrderedEnumerable(IEnumerable<TElement> source, Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending)
        {
            this.source = source ?? throw new ArgumentNullException("source");
            this.parent = null;
            this.keySelector = keySelector ?? throw new ArgumentNullException("keySelector");
            this.comparer = comparer != null ? comparer : Comparer<TKey>.Default;
            this.descending = descending;
        }

        internal override EnumerableSorter<TElement> GetEnumerableSorter(EnumerableSorter<TElement> next)
        {
            EnumerableSorter<TElement> sorter = new EnumerableSorter<TElement, TKey>(keySelector, comparer, descending, next);
            if (parent != null) sorter = parent.GetEnumerableSorter(sorter);
            return sorter;
        }
    }

    public abstract class OrderedEnumerable<TElement> : IOrderedEnumerable<TElement>
    {
        internal IEnumerable<TElement> source;

        public IEnumerator<TElement> GetEnumerator()
        {
            Buffer<TElement> buffer = new Buffer<TElement>(source);
            if (buffer.count > 0)
            {
                EnumerableSorter<TElement> sorter = GetEnumerableSorter(null);
                int[] map = sorter.Sort(buffer.items, buffer.count);
                sorter = null;
                for (int i = 0; i < buffer.count; i++) yield return buffer.items[map[i]];
            }
        }

        internal abstract EnumerableSorter<TElement> GetEnumerableSorter(EnumerableSorter<TElement> next);

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IOrderedEnumerable<TElement> IOrderedEnumerable<TElement>.CreateOrderedEnumerable<TKey>(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending)
        {
            OrderedEnumerable<TElement, TKey> result = new OrderedEnumerable<TElement, TKey>(source, keySelector, comparer, descending);
            result.parent = this;
            return result;
        }
    }

}

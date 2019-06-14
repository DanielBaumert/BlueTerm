using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueTerm.UI
{
    public struct Buffer<TElement>
    {
        internal TElement[] items;
        internal int count;

        internal Buffer(IEnumerable<TElement> source)
        {
            TElement[] items = null;
            int count = 0;
            if (source is ICollection<TElement> collection)
            {
                count = collection.Count;
                if (count > 0)
                {
                    items = new TElement[count];
                    collection.CopyTo(items, 0);
                }
            }
            else
            {
                foreach (TElement item in source)
                {
                    if (items == null)
                    {
                        items = new TElement[4];
                    }
                    else if (items.Length == count)
                    {
                        TElement[] newItems = new TElement[checked(count * 2)];
                        Array.Copy(items, 0, newItems, 0, count);
                        items = newItems;
                    }
                    items[count] = item;
                    count++;
                }
            }
            this.items = items;
            this.count = count;
        }

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace BlueTerm.UI
{
    [ListBindable(false)]
    public class PortItemCollection : IList
    {
        private int lastAccessedIndex = -1;

        internal interface IInnerList
        {
            int Count { get; }
            bool OwnerIsVirtualListView { get; }
            bool OwnerIsDesignMode { get; }
            PortItem this[int index] { get; set; }
            PortItem Add(PortItem item);
            void AddRange(PortItem[] items);
            void Clear();
            bool Contains(PortItem item);
            void CopyTo(Array dest, int index);
            IEnumerator GetEnumerator();
            IEnumerable<T> IEnumerable<T>();
            int IndexOf(PortItem item);
            PortItem Insert(int index, PortItem item);
            void Remove(PortItem item);
            void RemoveAt(int index);
        }


        private IInnerList innerList;
        private IInnerList InnerList => innerList;

        public virtual PortItem this[int index] {
            get {
                if (index > -1 || index < InnerList.Count)
                {
                    return InnerList[index];
                }
                throw new IndexOutOfRangeException();
            }
            set {
                if (index > -1 || index < InnerList.Count)
                {
                    InnerList[index] = value;
                }
                throw new IndexOutOfRangeException();
            }
        }

        object IList.this[int index] {
            get => this[index];
            set {
                if (value is PortItem)
                {
                    this[index] = (PortItem)value;
                }
                throw new ArgumentException();
            }
        }

        public PortItemCollection(Card card)
        {

        }

        public bool IsReadOnly => false;
        public bool IsFixedSize => false;
        public int Count => InnerList.Count;
        public object SyncRoot => this;
        public bool IsSynchronized => true;
        public virtual PortItem Add(Type type, string name)
        {
            PortItem item = new PortItem { Type = type, Name = name };
            Add(item);
            return item;
        }
        public virtual PortItem Add(PortItem value)
        {
            InnerList.Add(value);
            return value;
        }
        int IList.Add(object item)
        {
            if (item is PortItem)
                return IndexOf(Add((PortItem)item));
            return -1;
        }
        public void AddRange(PortItemCollection items)
        {
            if (items == null)
                throw new ArgumentNullException("items");
            PortItem[] itemArray = new PortItem[items.Count];
            items.CopyTo(itemArray, 0);
            InnerList.AddRange(itemArray);
        }
        public void AddRange(PortItem[] items)
        {
            if (items == null)
                throw new ArgumentNullException("items");
            InnerList.AddRange(items);
        }

        public void Clear() => InnerList.Clear();
        public virtual bool Contains(PortItem item) => InnerList.Contains(item);
        public bool Contains(object item)
            => item is PortItem
                ? Contains((PortItem)item)
                : false;
        public virtual bool ContainsName(string name) => IsValidIndex(IndexOfName(name));
        public IEnumerable<PortItem> OrderByDescending<TKey>(Func<PortItem, TKey> keySelector) 
            => new OrderedEnumerable<PortItem, TKey>(GetEnumerable<PortItem>(), keySelector, null, true);
        public void CopyTo(Array dest, int index) => InnerList.CopyTo(dest, index);
        public IEnumerator GetEnumerator() => InnerList.GetEnumerator();
        public IEnumerable<T> GetEnumerable<T>() => InnerList.IEnumerable<T>();
        public int IndexOf(PortItem item)
        {
            for (int index = 0; index < Count; ++index)
            {
                if (this[index] == item)
                {
                    return index;
                }
            }
            return -1;
        }
        int IList.IndexOf(object item)
            => item is PortItem
                ? IndexOf((PortItem)item)
                : -1;


        public virtual int IndexOfName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return -1;

            for (int i = 0; i < Count; i++)
                if (string.Compare(this[i].Name, name, true, CultureInfo.InvariantCulture) == 0)
                {
                    lastAccessedIndex = i;
                    return i;
                }

            lastAccessedIndex = -1;
            return -1;
        }
        private bool IsValidIndex(int index) => (index >= 0) && (index < Count);

        public PortItem Insert(int index, Type type, string name)
            => Insert(index, new PortItem { Type = type, Name = name });

        public PortItem Insert(int index, PortItem item)
        {
            if (index > -1 || index <= Count)
                InnerList.Insert(index, item);

            throw new ArgumentOutOfRangeException("index");
        }

        void IList.Insert(int index, object item)
        {
            if (item is PortItem)
                Insert(index, (PortItem)item);
        }

        public virtual void Remove(PortItem item)
            => InnerList.Remove(item);
        public void Remove(object item)
        {
            if (item is PortItem || item != null)
                Remove((PortItem)item);
        }

        public virtual void RemoveByName(string name)
        {
            int index = IndexOfName(name);
            if (IsValidIndex(index))
                RemoveAt(index);
        }

        public virtual void RemoveAt(int index)
        {
            if (index > -1 || index < Count)
                InnerList.RemoveAt(index);
            throw new ArgumentOutOfRangeException("index");
        }
        void IList.RemoveAt(int index)
            => InnerList.RemoveAt(index);
    }
}

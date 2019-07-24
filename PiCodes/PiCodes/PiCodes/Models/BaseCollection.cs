using System;
using System.Collections.Specialized;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PiCodes.Models
{
    public interface IElement
    {
        string GetInfo(bool shortInfo);
        double Price { get; }
    }

    public abstract class BaseCollection<T> : IList<T>, INotifyCollectionChanged, ICollection<T>, IEnumerator<T>
        where T : IEquatable<T>, IElement, INotifyPropertyChanged
    {
        protected T[] elements;

        private int buffer_size = 1;

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public T this[int index]
        {
            get { return elements[index]; }
            set
            {
                T oldValue = elements[index];
                elements[index] = value;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, oldValue));
            }
        }

        protected int position = -1;
        bool IEnumerator.MoveNext()
        {
            if (position < elements.Length - 1)
            { position++; return true; }
            return false;
        }
        object IEnumerator.Current => elements[position];
        IEnumerator IEnumerable.GetEnumerator() => this;
        void IEnumerator.Reset() => position = -1;
        public IEnumerator<T> GetEnumerator() => this;
        public T Current => elements[position];
        public void Dispose() => ((IEnumerator)this).Reset();

        private int cnt = 0;
        public int Count => cnt;
        public bool IsReadOnly => false;
        public void Add(T item)
        {
            if (IsReadOnly) throw new NotSupportedException();
            if (Contains(item)) return;

            cnt++;
            if(Count + 2 > buffer_size)
            {
                buffer_size *= 2;
                T[] newElements = new T[buffer_size];
                for (int i = 0; i < Count - 1; i++)
                    newElements[i] = elements[i];
                newElements[Count - 1] = item;
                elements = newElements;
            }
            else
            elements[Count - 1] = item;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }


        public bool Remove(T item)
        {
            if (IsReadOnly) throw new NotSupportedException();
            if (Contains(item))
            {
                cnt--;
                T[] newElements = new T[elements.Length - 1];
                int i = 0;
                while (!item.Equals(elements[i]))
                {
                    newElements[i] = elements[i];
                    i++;
                }
                i++;
                while (i < elements.Length)
                {
                    newElements[i - 1] = elements[i];
                    i++;
                }
                elements = newElements;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
                return true;
            }
            else return false;
        }
        public void Clear()
        {
            if (IsReadOnly) throw new NotSupportedException();
            elements = new T[] { };
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        public bool Contains(T item)
        {
            foreach (T cur in elements)
                if (item.Equals(cur)) return true;
            return false;
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException();
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException();
            if (Count > array.Length - arrayIndex) throw new ArgumentException();
            int i = arrayIndex;
            foreach (T cur in elements)
                array[i++] = cur;
        }

        public string[] GetInfoList()
        {
            string[] output = new string[Count];
            int i = 0;
            foreach (var cur in elements)
            {
                output[i++] = cur.GetInfo(true);
            }
            return output;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        public int IndexOf(T item)
        {
            for (int i = 0; i < Count; i++)
                if (elements[i].Equals(item)) return i;
            return -1;
        }

        public void Insert(int index, T item)
        {
            if (IsReadOnly) throw new NotSupportedException();
            if (item == null) throw new ArgumentNullException();
            if (index < 0) throw new ArgumentOutOfRangeException();
            if (index > Count) throw new ArgumentException();
            T[] newElements = new T[Count + 1];
            for (int i = 0; i < index; i++)
                newElements[i] = elements[i];
            newElements[index] = item;
            for (int i = index + 1; i < newElements.Length; i++)
                newElements[i] = elements[i - 1];
            elements = newElements;
        }

        public void RemoveAt(int index)
        {
            if (IsReadOnly) throw new NotSupportedException();
            if (index < 0) throw new ArgumentOutOfRangeException();
            if (index >= Count) throw new ArgumentException();
            T[] newElements = new T[Count - 1];
            for (int i = 0; i < index; i++)
                newElements[i] = elements[i];
            for (int i = index + 1; i < elements.Length; i++)
                newElements[i - 1] = elements[i];
            elements = newElements;
        }
    }
}

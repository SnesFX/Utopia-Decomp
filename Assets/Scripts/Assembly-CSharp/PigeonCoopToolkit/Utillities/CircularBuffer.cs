using System;
using System.Collections;
using System.Collections.Generic;

namespace PigeonCoopToolkit.Utillities
{
	public class CircularBuffer<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
	{
		private T[] _buffer;

		private int _position;

		private long _version;

		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public T this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new IndexOutOfRangeException();
				}
				int num = (_position - Count + index) % Capacity;
				return _buffer[num];
			}
			set
			{
				Insert(index, value);
			}
		}

		public int Capacity { get; private set; }

		public int Count { get; private set; }

		public CircularBuffer(int capacity)
		{
			if (capacity <= 0)
			{
				throw new ArgumentException("Must be greater than zero", "capacity");
			}
			Capacity = capacity;
			_buffer = new T[capacity];
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(T item)
		{
			_buffer[_position++ % Capacity] = item;
			if (Count < Capacity)
			{
				Count += 1;
			}
			_version++;
		}

		public void Clear()
		{
			for (int i = 0; i < Count; i++)
			{
				_buffer[i] = default(T);
			}
			_position = 0;
			Count = 0;
			_version++;
		}

		public bool Contains(T item)
		{
			int num = IndexOf(item);
			return num != -1;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			for (int i = 0; i < Count; i++)
			{
				array[i + arrayIndex] = _buffer[(_position - Count + i) % Capacity];
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			long version = _version;
			for (int i = 0; i < Count; i++)
			{
				if (version != _version)
				{
					throw new InvalidOperationException("Collection changed");
				}
				yield return this[i];
			}
		}

		public int IndexOf(T item)
		{
			for (int i = 0; i < Count; i++)
			{
				T val = _buffer[(_position - Count + i) % Capacity];
				if (item == null && val == null)
				{
					return i;
				}
				if (item != null && item.Equals(val))
				{
					return i;
				}
			}
			return -1;
		}

		public void Insert(int index, T item)
		{
			if (index < 0 || index > Count)
			{
				throw new IndexOutOfRangeException();
			}
			if (index == Count)
			{
				Add(item);
				return;
			}
			int num = Math.Min(Count, Capacity - 1) - index;
			int num2 = (_position - Count + index) % Capacity;
			for (int num3 = num2 + num; num3 > num2; num3--)
			{
				int num4 = num3 % Capacity;
				int num5 = (num3 - 1) % Capacity;
				_buffer[num4] = _buffer[num5];
			}
			_buffer[num2] = item;
			if (Count < Capacity)
			{
				Count += 1;
				_position++;
			}
			_version++;
		}

		public bool Remove(T item)
		{
			int num = IndexOf(item);
			if (num == -1)
			{
				return false;
			}
			RemoveAt(num);
			return true;
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index >= Count)
			{
				throw new IndexOutOfRangeException();
			}
			for (int i = index; i < Count - 1; i++)
			{
				int num = (_position - Count + i) % Capacity;
				int num2 = (_position - Count + i + 1) % Capacity;
				_buffer[num] = _buffer[num2];
			}
			int num3 = (_position - 1) % Capacity;
			_buffer[num3] = default(T);
			_position--;
			Count -= 1;
			_version++;
		}
	}
}

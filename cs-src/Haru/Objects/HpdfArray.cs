using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Haru.Streams;
using Haru.Types;

namespace Haru.Objects
{
    /// <summary>
    /// PDF array object [...]
    /// </summary>
    public class HpdfArray : HpdfObject, IList<HpdfObject>
    {
        private readonly List<HpdfObject> _items;

        /// <inheritdoc/>
        public override HpdfObjectClass ObjectClass => HpdfObjectClass.Array;

        /// <summary>
        /// Gets the number of items in the array
        /// </summary>
        public int Count => _items.Count;

        /// <summary>
        /// Gets whether the array is read-only
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets or sets the object at the specified index
        /// </summary>
        public HpdfObject this[int index]
        {
            get => _items[index];
            set => _items[index] = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Creates a new empty array
        /// </summary>
        public HpdfArray()
        {
            _items = new List<HpdfObject>();
        }

        /// <summary>
        /// Creates a new array from a rectangle/box
        /// </summary>
        public HpdfArray(HpdfRect rect)
            : this()
        {
            Add(new HpdfReal(rect.Left));
            Add(new HpdfReal(rect.Bottom));
            Add(new HpdfReal(rect.Right));
            Add(new HpdfReal(rect.Top));
        }

        /// <summary>
        /// Creates a new array from items
        /// </summary>
        public HpdfArray(params HpdfObject[] items)
            : this()
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    Add(item);
                }
            }
        }

        /// <summary>
        /// Adds an object to the array
        /// </summary>
        public void Add(HpdfObject item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            _items.Add(item);
        }

        /// <summary>
        /// Inserts an object at the specified index
        /// </summary>
        public void Insert(int index, HpdfObject item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            _items.Insert(index, item);
        }

        /// <summary>
        /// Removes an object from the array
        /// </summary>
        public bool Remove(HpdfObject item)
        {
            return _items.Remove(item);
        }

        /// <summary>
        /// Removes the object at the specified index
        /// </summary>
        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        /// <summary>
        /// Clears all items from the array
        /// </summary>
        public void Clear()
        {
            _items.Clear();
        }

        /// <summary>
        /// Determines whether the array contains a specific object
        /// </summary>
        public bool Contains(HpdfObject item)
        {
            return _items.Contains(item);
        }

        /// <summary>
        /// Determines the index of a specific object
        /// </summary>
        public int IndexOf(HpdfObject item)
        {
            return _items.IndexOf(item);
        }

        /// <summary>
        /// Copies the elements to an array
        /// </summary>
        public void CopyTo(HpdfObject[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator
        /// </summary>
        public IEnumerator<HpdfObject> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        public override void WriteValue(HpdfStream stream)
        {
            stream.WriteChar('[');

            for (int i = 0; i < _items.Count; i++)
            {
                if (i > 0)
                    stream.WriteChar(' ');
                _items[i].WriteValue(stream);
            }

            stream.WriteChar(']');
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[{string.Join(" ", _items.Select(o => o.ToString()))}]";
        }
    }
}

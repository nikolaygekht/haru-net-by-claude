using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Haru.Streams;

namespace Haru.Objects
{
    /// <summary>
    /// PDF dictionary object &lt;&lt;...&gt;&gt;
    /// </summary>
    public class HpdfDict : HpdfObject, IDictionary<string, HpdfObject>
    {
        private readonly Dictionary<string, HpdfObject> _items;

        /// <inheritdoc/>
        public override HpdfObjectClass ObjectClass => HpdfObjectClass.Dict;

        /// <summary>
        /// Gets the number of key-value pairs in the dictionary
        /// </summary>
        public int Count => _items.Count;

        /// <summary>
        /// Gets whether the dictionary is read-only
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets the collection of keys
        /// </summary>
        public ICollection<string> Keys => _items.Keys;

        /// <summary>
        /// Gets the collection of values
        /// </summary>
        public ICollection<HpdfObject> Values => _items.Values;

        /// <summary>
        /// Gets or sets the value associated with the specified key
        /// </summary>
        public HpdfObject this[string key]
        {
            get => _items[key];
            set
            {
                if (key is null)
                    throw new ArgumentNullException(nameof(key));
                if (value is null)
                    throw new ArgumentNullException(nameof(value));
                _items[key] = value;
            }
        }

        /// <summary>
        /// Creates a new empty dictionary
        /// </summary>
        public HpdfDict()
        {
            _items = new Dictionary<string, HpdfObject>();
        }

        /// <summary>
        /// Adds a key-value pair to the dictionary
        /// </summary>
        public void Add(string key, HpdfObject value)
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));
            if (value is null)
                throw new ArgumentNullException(nameof(value));
            _items.Add(key, value);
        }

        /// <summary>
        /// Adds a key-value pair (for collection initializer)
        /// </summary>
        public void Add(KeyValuePair<string, HpdfObject> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        /// Removes a key from the dictionary
        /// </summary>
        public bool Remove(string key)
        {
            return _items.Remove(key);
        }

        /// <summary>
        /// Removes a key-value pair
        /// </summary>
        public bool Remove(KeyValuePair<string, HpdfObject> item)
        {
            return ((IDictionary<string, HpdfObject>)_items).Remove(item);
        }

        /// <summary>
        /// Determines whether the dictionary contains the specified key
        /// </summary>
        public bool ContainsKey(string key)
        {
            return _items.ContainsKey(key);
        }

        /// <summary>
        /// Determines whether the dictionary contains a specific key-value pair
        /// </summary>
        public bool Contains(KeyValuePair<string, HpdfObject> item)
        {
            return ((IDictionary<string, HpdfObject>)_items).Contains(item);
        }

        /// <summary>
        /// Tries to get the value associated with the specified key
        /// </summary>
        public bool TryGetValue(string key, out HpdfObject? value)
        {
            return _items.TryGetValue(key, out value);
        }

        /// <summary>
        /// Clears all key-value pairs from the dictionary
        /// </summary>
        public void Clear()
        {
            _items.Clear();
        }

        /// <summary>
        /// Copies the elements to an array
        /// </summary>
        public void CopyTo(KeyValuePair<string, HpdfObject>[] array, int arrayIndex)
        {
            ((IDictionary<string, HpdfObject>)_items).CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator
        /// </summary>
        public IEnumerator<KeyValuePair<string, HpdfObject>> GetEnumerator()
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
            stream.WriteString("<<");
            stream.WriteLine();

            foreach (var kvp in _items)
            {
                stream.WriteEscapedName(kvp.Key);
                stream.WriteChar(' ');

                // Write indirect reference if object is indirect, otherwise write value
                if (kvp.Value.IsIndirect && kvp.Value.ObjectId != 0)
                {
                    stream.WriteUInt(kvp.Value.RealObjectId);
                    stream.WriteChar(' ');
                    stream.WriteUInt(kvp.Value.GenerationNumber);
                    stream.WriteString(" R");
                }
                else
                {
                    kvp.Value.WriteValue(stream);
                }
                stream.WriteLine();
            }

            stream.WriteString(">>");
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var items = string.Join(" ", _items.Select(kvp => $"/{kvp.Key} {kvp.Value}"));
            return $"<< {items} >>";
        }
    }
}

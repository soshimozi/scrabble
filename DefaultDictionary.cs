using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scrabble
{
    public class DefaultDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        new public TValue this[TKey key]
        {
            get
            {
                if (!TryGetValue(key, out TValue value))
                {
                    value = Activator.CreateInstance<TValue>();
                    Add(key, value);
                }

                return value;

            }

            set
            {
                Add(key, value);
            }
        }
    }
}

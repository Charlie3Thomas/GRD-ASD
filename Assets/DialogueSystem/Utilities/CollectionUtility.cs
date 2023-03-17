using System.Collections.Generic;

namespace CT.Utilities
{
    public static class CollectionUtility
    {
        public static void AddItem<_T, K>(this SerializableDictionary<_T, List<K>> serializableDictionary, _T _key, K _value)
        {
            if (serializableDictionary.ContainsKey(_key))
            {
                serializableDictionary[_key].Add(_value);

                return;
            }

            serializableDictionary.Add(_key, new List<K>() { _value });
        }
    }
}
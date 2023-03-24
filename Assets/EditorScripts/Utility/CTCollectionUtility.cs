using System.Collections.Generic;

namespace CT.Utils
{
    public static class CTCollectionUtility
    {
        public static void AddItem<_T, _K>(this SerializableDictionary<_T, List<_K>> serializableDictionary, _T _key, _K _value)
        {
            if (serializableDictionary.ContainsKey(_key))
            {
                serializableDictionary[_key].Add(_value);

                return;
            }

            serializableDictionary.Add(_key, new List<_K>() { _value });
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CT.Utilities
{
    public static class CollectionUtility
    {
        /*
            Dictionary value is not a list
            We need to check if the dicionary already has an element with this key
         */
        public static void AddItem<_T, _K>(this SerializableDictionary<_T, List<_K>> _serializable_dictionary, _T _key, _K _value)
        {
            if (_serializable_dictionary.ContainsKey(_key))
            {
                _serializable_dictionary[_key].Add(_value);

                return;
            }

            _serializable_dictionary.Add(_key, new List<_K>() { _value });
        }
    }
}

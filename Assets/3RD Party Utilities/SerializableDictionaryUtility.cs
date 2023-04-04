using System.Collections.Generic;

public static class SerializableDictionaryUtility
{
    public static void AddItem<_T, _K>(this SerializableDictionary<_T, List<_K>> _s_dictionary, _T _key, _K _value)
    {
        if (_s_dictionary.ContainsKey(_key))
        {
            _s_dictionary[_key].Add(_value);

            return;
        }

        _s_dictionary.Add(_key, new List<_K>() { _value });
    }
}
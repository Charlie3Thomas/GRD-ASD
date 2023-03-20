using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEventManager : MonoBehaviour
{
    public static GlobalEventManager instance;

    public static Action<Language> OnLanguageChange;

    private void Awake()
    {
        instance = this;
    }

    public void InvokeEnglish()
    {
        OnLanguageChange?.Invoke(Language.English);
    }

    public void InvokeFrench()
    {
        OnLanguageChange?.Invoke(Language.French);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Aspie.Language
{
    public enum Language
    {
        ENGLISH = 0,
        FRENCH = 1,
    }
    public class LocalizationManager : MonoBehaviour
    {
        private static LocalizationManager _instance;
        public static LocalizationManager Instance { get { return _instance; } }
        void Awake()
        {
            if (_instance == null || _instance != this)
                _instance = this;
        }
        public bool IsFrench => LocalizationSettings.SelectedLocale != LocalizationSettings.AvailableLocales.Locales [0];
        
        public void UpdateLanguage(Language language)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[(int)language];
        }
        public void UpdateLanguage(int id)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[id];
        }
    }
}

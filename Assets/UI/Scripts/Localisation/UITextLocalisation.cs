using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UITextLocalisation : MonoBehaviour
{
    public string englishText = "";
    public string frenchText = "";

    private TMPro.TextMeshProUGUI textMeshPro;

    void Start()
    {
        textMeshPro = GetComponent<TMPro.TextMeshProUGUI>();
        englishText = textMeshPro.text; // Get default value as English
        if (frenchText == "")
        {
            frenchText = englishText;
        }
        // Subscribe to global event for language change
        GlobalEventManager.OnLanguageChange += UpdateLanguage;
    }

    void UpdateLanguage(Language language)
    {
        switch (language)
        {
            case Language.English:
                textMeshPro.text = englishText;
                break;
            case Language.French:
                textMeshPro.text = frenchText;
                break;
        }       
    }

    // Unsubscribe from event to avoid memory leaks
    private void OnDestroy()
    {
        GlobalEventManager.OnLanguageChange -= UpdateLanguage;
    }
}

public enum Language 
{
    English,
    French
}


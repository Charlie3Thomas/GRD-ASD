using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
public class AddCustomScriptToTextObjects : EditorWindow
{
    [MenuItem("Custom Tools/Add Custom Script To Text Objects")]
    public static void AddScriptToTextObjects()
    {
        TMPro.TextMeshProUGUI[] textObjects = FindObjectsOfType<TMPro.TextMeshProUGUI>();

        foreach (TMPro.TextMeshProUGUI textObject in textObjects)
        {
            if (textObject.GetComponent<UITextLocalisation>() == null)
            {
                textObject.gameObject.AddComponent<UITextLocalisation>();
            }
        }
    }
}

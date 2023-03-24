using UnityEditor;
using UnityEngine.UIElements;

namespace CT.Utils
{
    public static class CTStyleUtility
    {
        public static VisualElement AddStyleSheets(this VisualElement _element, params string[] _style_sheet_names)
        {
            foreach (string style_sheet_name in _style_sheet_names)
            {
                StyleSheet style_sheet = (StyleSheet) EditorGUIUtility.Load(style_sheet_name);

                _element.styleSheets.Add(style_sheet);
            }

            return _element;
        }
    }
}
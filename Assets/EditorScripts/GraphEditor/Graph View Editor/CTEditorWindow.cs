using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace CT.Windows
{
    using Utils;

    public class CTEditorWindow : EditorWindow
    {
        private CTGraphView graph_view;

        private readonly string default_file_name = "TreeFileName";

        private static TextField tf_file_name;
        private Button btn_save;

        [MenuItem("Window/CT Dialogue Tree Editor/Dialogue Graph")]
        public static void Open()
        {
            GetWindow<CTEditorWindow>("Dialogue Graph");
        }

        private void OnEnable()
        {
            AddGraphView();
            AddToolbar();

            AddStyles();
        }

        private void AddGraphView()
        {
            graph_view = new CTGraphView(this);

            graph_view.StretchToParentSize();

            rootVisualElement.Add(graph_view);
        }

        private void AddToolbar()
        {
            Toolbar toolbar = new Toolbar();

            tf_file_name = CTElementUtility.CreateTextField(default_file_name, "File Name:", callback =>
            {
                tf_file_name.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
            });

            btn_save = CTElementUtility.CreateButton("Save", () => Save());

            Button btn_load = CTElementUtility.CreateButton("Load", () => Load());
            Button btn_clear = CTElementUtility.CreateButton("Clear", () => Clear());
            Button btn_new = CTElementUtility.CreateButton("New", () => ResetGraph());

            toolbar.Add(tf_file_name);
            toolbar.Add(btn_save);
            toolbar.Add(btn_load);
            toolbar.Add(btn_clear);
            toolbar.Add(btn_new);

            rootVisualElement.Add(toolbar);
        }

        private void AddStyles()
        {
            rootVisualElement.AddStyleSheets("Assets/EditorScripts/GraphEditor/StyleSheets/CTVariables.uss");
        }

        private void Save()
        {
            if (string.IsNullOrEmpty(tf_file_name.value))
            {
                EditorUtility.DisplayDialog("Invalid file name.", "Please provide a valid file name.", "Okay");

                return;
            }

            CTIOUtility.Initialise(graph_view, tf_file_name.value);
            CTIOUtility.Save();
        }

        private void Load()
        {
            string file_path = EditorUtility.OpenFilePanel("Dialogue Graphs", "Assets/Editor/DialogueSystem/Graphs", "asset");

            if (string.IsNullOrEmpty(file_path))
            {
                return;
            }

            Clear();

            CTIOUtility.Initialise(graph_view, Path.GetFileNameWithoutExtension(file_path));
            CTIOUtility.Load();
        }

        private void Clear()
        {
            graph_view.ClearGraph();
        }

        private void ResetGraph()
        {
            Clear();

            UpdateFileName(default_file_name);
        }

        public static void UpdateFileName(string _new_file_name)
        {
            tf_file_name.value = _new_file_name;
        }

        public void AllowSaving()
        {
            btn_save.SetEnabled(true);
        }

        public void DenySaving()
        {
            btn_save.SetEnabled(false);
        }
    }
}
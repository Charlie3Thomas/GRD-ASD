#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace CT.GraphView
{
    using System.Diagnostics;
    using UnityEditor.Experimental.GraphView;
    using Utils;

    public class CTGraphViewWindow : EditorWindow
    {
        private CTGraphView graph_view;

        private readonly string default_title = "TreeName";

        private static TextField file_name;
        private Button save;


        private void OnEnable()
        {
            graph_view = new CTGraphView(this);
            InitialiseGraphView();
            InitialiseToolbar();
        }

        #region Setup
        [MenuItem("Window/Dialogue Tree")]
        public static void Open()
        {
            GetWindow<CTGraphViewWindow>("Dialogue Tree");
        }

        private void InitialiseGraphView()
        {
            graph_view.StretchToParentSize();

            rootVisualElement.Add(graph_view);
        }

        private void InitialiseToolbar()
        {
            Toolbar toolbar = new Toolbar();

            file_name = CTComponentUtility.CreateTextField(default_title, "File Name:", callback =>
            {
                file_name.value = callback.newValue;
            });

            save = CTComponentUtility.CreateButton("Save", () => Save());
            Button load = CTComponentUtility.CreateButton("Load", () => Load());
            Button clear = CTComponentUtility.CreateButton("Clear", () => Clear());

            toolbar.Add(file_name);
            toolbar.Add(save);
            toolbar.Add(load);
            toolbar.Add(clear);

            rootVisualElement.Add(toolbar);
        }

        public static void ChangeTitle(string _title)
        {
            file_name.value = _title;
        }

        #endregion

        #region Utility
        public void AllowSaving()
        {
            save.SetEnabled(true);
        }

        public void DenySaving()
        {
            save.SetEnabled(false);
        }

        private void Save()
        {
            if (string.IsNullOrEmpty(file_name.value))
            {
                UnityEngine.Debug.LogError("Filename is required!");

                return;
            }

            CTIOUtility.Initialise(graph_view, file_name.value);
            CTIOUtility.Save();
        }

        private void Load()
        {
            string path = EditorUtility.OpenFilePanel("Dialogue Trees", "Assets/Editor/DialogueSystem/Graphs", "asset");

            if (string.IsNullOrEmpty(path))
                return;

            Clear();

            CTIOUtility.Initialise(graph_view, Path.GetFileNameWithoutExtension(path));
            CTIOUtility.Load();
        }

        private void Clear()
        {
            if (graph_view != null)
                graph_view.ClearTree();
        }

        private void Reset()
        {
            Clear();

            ChangeTitle(default_title);
        }

        #endregion
    }
}
#endif
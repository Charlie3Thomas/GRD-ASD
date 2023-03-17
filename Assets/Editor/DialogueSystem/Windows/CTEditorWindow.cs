using CT.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CT.Windows
{
    public class CTEditorWindow : EditorWindow
    {
        private CTGraphView graph_view; 

        private readonly string default_filename = "NewDialogueGraph";

        private static TextField filename_textfield;

        private Button save_button;

        [MenuItem("Window/CT/Dialogue Graph")]
        public static void Open()
        {
            //Debug.Log("Opening window!");

            CTEditorWindow editor_window = GetWindow<CTEditorWindow>();
            editor_window.titleContent = new GUIContent("CT Dialogue Graph");
        }

        private void OnEnable()
        {
            AddGraphView();
            AddToolBar();
            AddStyles();
        }

        #region Style
        private void AddGraphView()
        {
            graph_view = new CTGraphView(this);

            graph_view.StretchToParentSize();

            rootVisualElement.Add(graph_view);
        }

        private void AddToolBar()
        {
            Toolbar toolbar = new Toolbar();

            filename_textfield = CTElementUtility.CreateTextField(default_filename, "File Name:", callback =>
            {
                filename_textfield.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
            });

            save_button = CTElementUtility.CreateButton("Save", () => SaveGraph());

            Button clear_button = CTElementUtility.CreateButton("Clear", () => ClearGraph());
            Button new_button = CTElementUtility.CreateButton("New", () => NewGraph());

            toolbar.Add(filename_textfield);
            toolbar.Add(save_button);
            toolbar.Add(clear_button);
            toolbar.Add(new_button);

            rootVisualElement.Add(toolbar);
        }

        private void AddStyles()
        {
            StyleSheet style_sheet = (StyleSheet)EditorGUIUtility.Load("DialogueSystem/CTVariables.uss");

            rootVisualElement.styleSheets.Add(style_sheet);
        }
        #endregion

        #region Utility

        public static void UpdateFileName(string _new_file_name)
        {
            filename_textfield.value = _new_file_name;
        }

        public void AllowSave()
        {
            save_button.SetEnabled(true);

            Debug.Log("Saving enabled");
        }

        public void DenySave()
        {
            save_button.SetEnabled(false);

            Debug.Log("Saving disabled");
        }

        private void SaveGraph()
        {
            // Check file name not empty
            if (string.IsNullOrEmpty(filename_textfield.value))
            {
                // Display error to user and skip file saving with invalid file name
                EditorUtility.DisplayDialog
                (
                    "INVALID FILE NAME.",
                    "Please ensure file name is valid!",
                    "Ok."
                );

                return;
            }

            CTIOUtility.Initialise(graph_view, filename_textfield.value);
            CTIOUtility.Save();
        }

        private void ClearGraph()
        {
            graph_view.ClearGraph();
        }

        private void NewGraph()
        {
            ClearGraph();
            UpdateFileName(default_filename);
        }

        #endregion

    }
}

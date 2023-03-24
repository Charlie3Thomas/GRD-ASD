using CT.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;
using TMPro;

namespace CT
{
    using Data;

    public class TestSOScript : MonoBehaviour
    {
        [SerializeField] private CTDialogueSO starting_dlog;

        [SerializeField] private TextMeshProUGUI text_UI;

        [SerializeField] private Button test_button_1;
        [SerializeField] private Button test_button_2;
        [SerializeField] private Button test_button_3;

        [SerializeField] private CTDialogueSO current_dlog;

        private void Awake()
        {
            current_dlog = starting_dlog;
        }

        private void Start()
        {
            // Buttons
            test_button_1.onClick.AddListener(TestButton1);
            test_button_2.onClick.AddListener(TestButton2);
            test_button_3.onClick.AddListener(TestButton3);

            ShowText();
            //OnOptionChosen(0);
        }

        private void ShowText()
        {
            // Clear text from previous node
            test_button_1.GetComponentInChildren<TextMeshProUGUI>().text = "";
            test_button_2.GetComponentInChildren<TextMeshProUGUI>().text = "";
            test_button_3.GetComponentInChildren<TextMeshProUGUI>().text = "";


            //// Debug
            //Debug.Log("current_dlog.IsStartingDialogue : " + current_dlog.IsStartingDialogue);
            //Debug.Log("current_dlog.DialogueName : " + current_dlog.DialogueName);
            //Debug.Log("current_dlog.DialogueType : " + current_dlog.DialogueType);
            //for (int i = 0; i < current_dlog.Choices.Count; i++)
            //{
            //    Debug.Log("current_dlog.Choices " + i + " : " + current_dlog.Choices[i].Text);
            //}
            //Debug.Log("current_dlog.name : " + current_dlog.name);
            //Debug.Log("current_dlog.Text : " + current_dlog.Text);
            //Debug.Log("current_dlog.TipText : " + current_dlog.TipText);

            // UI Text
            text_UI.text = current_dlog.dlog_text;

            // UI Buttons
            // Need something more elegant but this is fine for testing purposes.
            if (current_dlog.dlog_type == Enumerations.CTDialogueType.MultipleChoice)
            {
                if (current_dlog.list_dlog_choices.Count > 0)
                    test_button_1.GetComponentInChildren<TextMeshProUGUI>().text = string.IsNullOrEmpty(current_dlog.list_dlog_choices[0].text) ? "" : current_dlog.list_dlog_choices[0].text;

                if (current_dlog.list_dlog_choices.Count > 1)
                    test_button_2.GetComponentInChildren<TextMeshProUGUI>().text = string.IsNullOrEmpty(current_dlog.list_dlog_choices[1].text) ? "" : current_dlog.list_dlog_choices[1].text;

                if (current_dlog.list_dlog_choices.Count > 2)
                    test_button_3.GetComponentInChildren<TextMeshProUGUI>().text = string.IsNullOrEmpty(current_dlog.list_dlog_choices[2].text) ? "" : current_dlog.list_dlog_choices[2].text;

            }
            //else
            //{
            //    test_button_1.GetComponentInChildren<TextMeshProUGUI>().text = "";
            //    test_button_2.GetComponentInChildren<TextMeshProUGUI>().text = "";
            //    test_button_3.GetComponentInChildren<TextMeshProUGUI>().text = "";
            //}
            
        }

        private void OnOptionChosen(int _choice_index)
        {
            CTDialogueSO next_dlog = current_dlog.list_dlog_choices[_choice_index].next_dlog_node;

            if  (next_dlog == null) 
            {
                // No more dialogue to show
                Debug.Log("Final dialogue");
                return;
            }

            current_dlog = next_dlog;

            ShowText();
        }

        private void TestButton1()
        {
            OnOptionChosen(0);
        }

        private void TestButton2()
        {
            OnOptionChosen(1);
        }

        private void TestButton3()
        {
            OnOptionChosen(2);
        }
    }
}

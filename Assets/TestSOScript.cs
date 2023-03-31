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
    using DG.Tweening;

    public class TestSOScript : MonoBehaviour
    {
        [SerializeField] private CTDialogueSO starting_dlog;

        [SerializeField] private TextMeshProUGUI text_UI;

        [SerializeField] private Button test_button_1;
        [SerializeField] private Button test_button_2;
        [SerializeField] private Button test_button_3;

        [SerializeField] private CTDialogueSO current_dlog;

        public GameObject[] selection;

        public int currentSelection = 0;

        private int currentmoveincount = 0;

        public RectTransform student, librarian;

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

            if (test_button_1.GetComponentInChildren<TextMeshProUGUI>().text == "")
            {
                test_button_1.gameObject.SetActive(false);
            }
            else
            {
                test_button_1.gameObject.SetActive(true);
                currentSelection = 0;
                UpdateSelection();
            }

            if (test_button_2.GetComponentInChildren<TextMeshProUGUI>().text == "")
            {
                test_button_2.gameObject.SetActive(false);
            }
            else
            {
                test_button_2.gameObject.SetActive(true);
                currentSelection = 0;
                UpdateSelection();
            }

            if (test_button_3.GetComponentInChildren<TextMeshProUGUI>().text == "")
            {
                test_button_3.gameObject.SetActive(false);
            }
            else
            {
                test_button_3.gameObject.SetActive(true);
                currentSelection = 0;
                UpdateSelection();
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

        public void ChooseEnter()
        {
            if(test_button_1.GetComponentInChildren<TextMeshProUGUI>().text == "" &&
               test_button_2.GetComponentInChildren<TextMeshProUGUI>().text == "" &&
               test_button_3.GetComponentInChildren<TextMeshProUGUI>().text == "")
            {
                OnOptionChosen(0);
                currentmoveincount++;

                if(currentmoveincount > 1)
                {
                    //move in student

                    student.DOAnchorPos(new Vector2(-502, 69), 0.5f)
                    .SetEase(Ease.Linear);
                        
                }


                if (currentmoveincount > 2)
                {
                    //move in librarian
                    librarian.DOAnchorPos(new Vector2(662, 31), 0.5f)
                    .SetEase(Ease.Linear);
                }

            }

            //if (currentSelection == 0)
            //{
            //    OnOptionChosen(0);
            //}
            //if (currentSelection == 1)
            //{
            //    OnOptionChosen(1);
            //}
            //if (currentSelection == 2)
            //{
            //    OnOptionChosen(2);
            //}

        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                ChooseEnter();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentSelection = currentSelection - 1;
                if (currentSelection < 0)
                {
                    currentSelection = 2;
                }
                UpdateSelection();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentSelection = currentSelection + 1;
                if (currentSelection > 2)
                {
                    currentSelection = 0;
                }
                UpdateSelection();
            }

        }

        private void UpdateSelection()
        {
            for(int i = 0; i < selection.Length; i++)
            {
                selection[i].SetActive(false);
            }

            if (test_button_1.GetComponentInChildren<TextMeshProUGUI>().text == "")
            {
                selection[0].SetActive(false);
            }
            if (test_button_2.GetComponentInChildren<TextMeshProUGUI>().text == "")
            {
                selection[1].SetActive(false);
            }
            if (test_button_3.GetComponentInChildren<TextMeshProUGUI>().text == "")
            {
                selection[2].SetActive(false);
            }

            selection[currentSelection].SetActive(true);
        }

    }
}

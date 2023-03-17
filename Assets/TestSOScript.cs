using CT.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CT
{
    using Data;
    using TMPro;
    using Unity.VisualScripting;

    public class TestSOScript : MonoBehaviour
    {
        [SerializeField] private CTDialogueSO starting_dlog;
        [SerializeField] private TextMeshProUGUI text_UI;

        private CTDialogueSO current_dlog;

        private void Awake()
        {
            current_dlog = starting_dlog;
        }

        private void Start()
        {
            ShowText();
            //OnOptionChosen(0);
        }

        private void ShowText()
        {
            Debug.Log("current_dlog.IsStartingDialogue : " + current_dlog.IsStartingDialogue);
            Debug.Log("current_dlog.DialogueName : " + current_dlog.DialogueName);
            Debug.Log("current_dlog.DialogueType : " + current_dlog.DialogueType);
            for (int i = 0; i < current_dlog.Choices.Count; i++)
            {
                Debug.Log("current_dlog.Choices " + i + " : " + current_dlog.Choices[i].Text);
            }
            Debug.Log("current_dlog.name : " + current_dlog.name);
            Debug.Log("current_dlog.Text : " + current_dlog.Text);
            Debug.Log("current_dlog.TipText : " + current_dlog.TipText);

            text_UI.text = current_dlog.Text;
        }

        private void OnOptionChosen(int _choice_index)
        {
            CTDialogueSO next_dlog = current_dlog.Choices[_choice_index].NextDialogue;

            if  (next_dlog == null) 
            {
                // No more dialogue to show
                Debug.Log("Final dialogue");
                return;
            }

            current_dlog = next_dlog;

            ShowText();
        }
    }
}

using CT.Data;
using CT.Enumerations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CT.ScriptableObjects
{
    public class CTDialogueSO : ScriptableObject
    {
        [field: SerializeField] public string DialogueName { get; set; }
        [field: SerializeField][field: TextArea()] public string Text { get; set; }
        [field: SerializeField] public List<CTDialogueChoiceData> Choices { get; set; }

        //string
        [field: SerializeField] public CTDialogueType DialogueType { get; set; }
        [field: SerializeField] public bool IsStartingDialogue { get; set; }

        public void Initialise(string _dialogue_name, string _text, List<CTDialogueChoiceData> _choices, CTDialogueType _dialogue_type, bool _is_starting_dialogue)
        {
            DialogueName= _dialogue_name;
            Text = _text;
            Choices = _choices;
            DialogueType = _dialogue_type;
            IsStartingDialogue = _is_starting_dialogue;
        }
    }
}

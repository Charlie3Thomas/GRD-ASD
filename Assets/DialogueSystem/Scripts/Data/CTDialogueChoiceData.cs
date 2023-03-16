using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CT.Data
{

    using ScriptableObjects;


    [Serializable]
    public class CTDialogueChoiceData
    {
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public CTDialogueSO NextDialogue { get; set; }
    }

    //using Data;
    //using Enumerations;

    //public class CTDialogueChoiceData
    //{
    //    [field: SerializeField] public string DialogueName { get; set; }
    //    [field: SerializeField] public string Text { get; set; }
    //    [field: SerializeField] public List<CTDialogueChoiceData> Choices { get; set; }
    //    [field: SerializeField] public CTDialogueType DialogueType { get; set; }
    //    [field: SerializeField] public bool IsStartingDialogue { get; set; }

    //    public void Initialise(string _dialogue_name, string _text, List<CTDialogueChoiceData> _choices, CTDialogueType _dialogue_type, bool _is_starting_dialogue)
    //    {
    //        DialogueName = _dialogue_name;
    //        Text = _text;
    //        Choices = _choices;
    //        DialogueType = _dialogue_type;
    //        IsStartingDialogue = _is_starting_dialogue;
    //    }

    //}
}

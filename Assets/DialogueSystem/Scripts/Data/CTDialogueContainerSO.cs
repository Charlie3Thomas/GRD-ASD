using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CT.ScriptableObjects
{
    public class CTDialogueContainerSO : ScriptableObject
    {
        [field: SerializeField] public string FileName { get; set; }
        [field: SerializeField] public SerializableDictionary<CTDialogueGroupSO, List<CTDialogueSO>> DialogueGroups { get; set; }
        [field: SerializeField] public List<CTDialogueSO> UngroupedDialogues { get; set; }

        public void Initialise(string _filename)
        {
            FileName = _filename;

            DialogueGroups = new SerializableDictionary<CTDialogueGroupSO, List<CTDialogueSO>>();

            UngroupedDialogues = new List<CTDialogueSO>();
        }
    }
}

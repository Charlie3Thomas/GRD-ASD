using UnityEngine;

namespace CT.ScriptableObjects
{
    public class CTDialogueGroupSO : ScriptableObject
    {
        [field: SerializeField] public string group_name { get; set; }

        public void Initialise(string _group_name)
        {
            group_name = _group_name;
        }
    }
}
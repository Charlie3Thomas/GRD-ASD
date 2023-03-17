using UnityEngine;

namespace CT.ScriptableObjects
{
    public class CTDialogueGroupSO : ScriptableObject
    {
        [field: SerializeField] public string GroupName { get; set; }

        public void Initialize(string _group_name)
        {
            GroupName = _group_name;
        }
    }
}
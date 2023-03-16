using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CT.ScriptableObjects
{
    public class CTDialogueGroupSO : ScriptableObject
    {
        [field: SerializeField] public string GroupName { get; set; }

        public void Initialise(string _groupname)
        {
            GroupName = _groupname;
        }
    }
}

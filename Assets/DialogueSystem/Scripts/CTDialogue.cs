using UnityEngine;

namespace CT
{
    using ScriptableObjects;

    public class CTDialogue : MonoBehaviour
    {
        // dlog SOs
        [SerializeField] private CTDialogueContainerSO dlog_container;
        [SerializeField] private CTDialogueGroupSO dlog_group;
        [SerializeField] private CTDialogueSO dialogue;

        // dlog type grops
        [SerializeField] private bool grouped_dlogs;
        [SerializeField] private bool start_dlogs_only;

        // Indexes
        [SerializeField] private int selected_dlog_group_index;
        [SerializeField] private int selected_dlog_index;
    }
}
using CT.Data;
using CT.SO;
using CT.UI.Engine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CT.Utilis
{
    public class CTNodeIOUtility : MonoBehaviour
    {
        [SerializeField] private CTUISetupUtility UI_setup;

        [SerializeField] private CTNodeDataSO starting_node;
        [SerializeField] private CTNodeDataSO current_node;
        [SerializeField] private CTNodeDataSO next_node;

        [SerializeField] private List<CTNodeOptionData> current_node_choices;
        [SerializeField] private string current_node_dlog;
        [SerializeField] private string current_node_tip;
        [SerializeField] private string current_character;

        public string current_character_data_name;
        private bool tip;

        private void OnEnable()
        {
            current_node = starting_node;
            ReadNodeInformation();
        }

        private void Start()
        {
            //current_node_dlog = "";
            //current_node_tip = "";
            //current_node_choices = new List<CTDialogueChoiceData>();
            //current_character = "";
            //tip = false;
            //is_end_node = false;
            
        }

        private void Update()
        {
            Test();

            if (current_node.type == Enums.CTNodeType.Narration)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    OnOptionChosen(0);
                }
            }
        }

        private void Test()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                OnOptionChosen(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                OnOptionChosen(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                OnOptionChosen(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                OnOptionChosen(3);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                OnOptionChosen(4);
            }
        }

        #region Node access methods
        public void ReadNodeInformation()
        {
            // Read current node information and sanitise
            current_node_dlog = current_node.text.Replace("\r", "");

            current_node_choices = current_node.options;

            current_character = current_node.character;

            TranslateCharacter();

            // If current node is a narration node
            if (current_node.type == Enums.CTNodeType.Narration)
                next_node = current_node.options[0].next_node; // There is only one choice in a narration node
            // If current node is a choice node
            // do not set next node yet as we need to wait for player to choose an option


            // Check if there is a tip
            if (current_node.tip_text != "" ) 
            {
                current_node_tip = current_node.tip_text;

                tip = true;
            }
            else
            {
                current_node_tip = "";

                tip = false;
            }
        }

        public void OnOptionChosen(int _choice_index)
        {
            // Do not proceed if current node is an end node
            if (CheckIfEndNode())
            {
                Debug.Log("End node is reached, no further dialogue");

                return;
            }

            // If node is connected to another node
            if (current_node.options.Count > 0)
            {
                // If current node is a narration node
                if (current_node.type == Enums.CTNodeType.Narration)
                {
                    // Set current node to next node at index
                    // Hard coded at 0 as there is only one choice in a narration node
                    next_node = current_node.options[0].next_node;
                }
                else // If current node is a choice node
                {
                    // Set current node to next node at index
                    next_node = current_node.options[_choice_index].next_node;
                }

                current_node = next_node;

                ReadNodeInformation();

                UI_setup.RefreshUI();
            }
        }

        private bool CheckIfEndNode()
        {
            if (next_node == null)
                return true;
            else
                return false;
        }


        #endregion

        #region Public Getter Methods
        // Get current node dialogue
        public string GetDlogText()
        {
            return current_node_dlog;
        }

        // Get current node tip
        public string GetTipText()
        {
            if (current_node_tip != "")
                return current_node_tip;
            else
                return "";
        }

        // Get current dialogue choices
        public List<CTNodeOptionData> GetDlogChoices()
        {
            return current_node_choices;
        }

        // Get choice text at index (index will be GetDlogChoices.Count -> range 0, ..., N)
        public string GetChoiceText(int _index)
        {
            return current_node_choices[_index].text;
        }

        public CT.Enums.CTNodeType GetNodeType()
        {
            return current_node.type;
        }

        private void TranslateCharacter()
        {
            current_character_data_name = current_character;
            // Remove carriage return(s) from string
            string s = current_character;
            s = s.Replace("\r", "");

            int i = 0;
            foreach (NodeCharacter cha in (NodeCharacter[])Enum.GetValues(typeof(NodeCharacter)))
            {
                if (cha.ToString().Replace("\r", "") == s)
                {
                    current_character = UI_setup.scene_characters[i].Replace("\r", "");
                }
                i++;
            }
        }

        // Get current character
        public string GetCharacter()
        {
            return current_character;
        }

        public bool IsThereATip()
        {
            return tip;
        }

        public bool IsThisEndNode()
        {
            return CheckIfEndNode();
        }

        public string GetBackground()
        {
            return current_node.background;
        }

        #endregion

    }

    #region Enumerations
    public enum NodeCharacter
    {
        Narrator,
        Character_0,
        Character_1,
        Character_2,
        Character_3,
        Character_4,
        Character_5,
        Character_6,
        Character_7,
        Character_8,
        Character_9
    }

    public enum Backgrounds
    { 
        Default,
        Background_0,
        Background_1,
        Background_2,
        Background_3
    }

    #endregion
}

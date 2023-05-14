
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


namespace CT.UI.Engine
{
    using Data;
    using UnityEngine.UI;
    using UnityEngine.UIElements;
    using Utilis;
    using static System.Net.Mime.MediaTypeNames;

    [RequireComponent(typeof(CTNodeIOUtility))]
    public class CTUISetupUtility : MonoBehaviour
    {
        [Header("Characters - Element 0 should always be 'Narrator'")]
        [SerializeField] public List<string> scene_characters;

        [SerializeField] private CTNodeIOUtility node_data;

        // Character sprites
        [SerializeField] private List<Texture2D> character_sprites;

        // Location sprites
        [SerializeField] private List<Texture2D> location_sprites;

        // Prefabs
        [Header("UI Prefabs")]
        [SerializeField] private GameObject image_prefab;
        //[SerializeField] private GameObject text_prefab;
        //[SerializeField] private GameObject tip_bttn_prefab;
        //[SerializeField] private GameObject tip_text_window;
        [SerializeField] private GameObject txt_bttn_prefab;
        [SerializeField] private TextMeshProUGUI txt_narration;
        [SerializeField] private TextTyper txt_typer;
        [SerializeField] private TextMeshProUGUI txt_character_name;

        // Anchor Points
        [Header("Anchor Points")]
        [SerializeField] private Transform background_anchor_point;
        [SerializeField] private Transform character_anchor_point;
        [SerializeField] private Transform tip_anchor_point;
        [SerializeField] private Transform dlog_choices_centre;
        [SerializeField] private Transform narration_anchor_point;

        // UI Elements
        private GameObject background;
        private GameObject character;
        private GameObject show_tip_button;
        private GameObject next_button;
        private GameObject narration;
        private List<GameObject> choices_buttons;
        public GameObject tip;

        private List<GameObject> UI_cleanup;

        private bool tip_status = false;

        private void Start()
        {
            background = new GameObject("Background");

            character = new GameObject("Character");

            show_tip_button = new GameObject("Show tip button");

            next_button = new GameObject("Next button");

            narration = new GameObject("Narration");

            choices_buttons = new List<GameObject>();
            UI_cleanup = new List<GameObject>();
            tip.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(() => ToggleTip());
            RefreshUI();
        }

        private void Update()
        {
            if (Input.GetKey("escape"))
            {
                UnityEngine.Application.Quit();
            }
        }



        #region Button Methods

        private void SetTipButtonStatus()
        {
            //tip.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(() => ToggleTip());

            //// Instantiate tip button and place at fixed position on the screen
            //show_tip_button = Instantiate(tip_bttn_prefab, tip_anchor_point);

            //show_tip_button.GetComponentInChildren<TextMeshProUGUI>().text = "";

            //ResizeButtonToTextureScale(show_tip_button.GetComponent<UnityEngine.UI.Button>(), 1);

            //tip = Instantiate(tip_text_window, tip_anchor_point);

            //// Set text to tip text and hide/show window
            //SetTipTextAndStatus();

            //show_tip_button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => tip.SetActive(!tip.activeSelf));
        }

        private void ToggleTip()
        {
            if (tip_status == false) 
            {
                // Tip text
                Debug.Log("Toggling to tip!");
                txt_narration.text = node_data.GetTipText();
                tip.GetComponentInChildren<TextMeshProUGUI>().text = "Cacher";
                txt_character_name.text = "Cacher";
                tip_status = true;
                return;
            }
            else
            {
                // Dialogue text
                Debug.Log("Toggling to narration!");
                txt_character_name.text = node_data.GetCharacterName();
                tip.GetComponentInChildren<TextMeshProUGUI>().text = "Conseil";
                txt_narration.text = node_data.GetDlogText();
                tip_status = false;
                return;
            }            
        }

        private void InstantiateNextDialogueButton()
        {
            // Instantiate next button and place at fixed position on the screen
            next_button = Instantiate(txt_bttn_prefab, dlog_choices_centre);

            ResizeButtonToTextureScale(next_button.GetComponent<UnityEngine.UI.Button>(), 1);
        }

        private void InstantiateChoiceButton(int _button_count)
        {
            if (_button_count < 0)
            {
                Debug.LogError("Cannot have negative buttons!");

                return;
            }

            List<GameObject> to_remove = new List<GameObject>(choices_buttons);

            // Clear old list
            foreach (GameObject go in to_remove)
            {
                Destroy(go);

                choices_buttons.Remove(go);
            }

            // Exit after clearnig choices if the node type is Narration
            if (node_data.GetNodeType() == Enums.CTNodeType.Narration)
                return;

            List<CTNodeOptionData> choices = node_data.GetDlogChoices();

            for (int i = 0; i < _button_count; i++)
            {
                int index = _button_count - i - 1;

                // Add offset to dlog_choices_centre
                GameObject button = Instantiate(txt_bttn_prefab, dlog_choices_centre);

                button.GetComponentInChildren<TextMeshProUGUI>().text = choices[index].text;

                button.name = index.ToString();

                choices_buttons.Add(button);

                button.transform.position += new Vector3(0.0f, i * 110.0f, 0.0f);

                //ResizeButtonToTextureScale(button.GetComponent<UnityEngine.UI.Button>(), 2);

                //ScaleButtonWithText(button.GetComponentInChildren<UnityEngine.UI.Button>());

                // Yes, this is a horrible hack. No, I am not sorry.
                button.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(() => OnChoiceButtonClick(Int32.Parse(button.name)));

                //button.name = index.ToString();
            }
        }

        private void UpdateNarrationWindow(string _dlog)
        {
            txt_typer.StartTypingText(_dlog);
            //txt_narration.text = _dlog;
            txt_character_name.text = node_data.GetCharacterName();

            //if (node_data.IsThereATip())
            //{
            //    tip.SetActive(true);
            //}
            //else
            //{
            //    tip.SetActive(false);
            //}

            // UPDATE TEXT SIZE BASED ON STRING LENGTH

            //Destroy(narration);

            //narration = Instantiate(text_prefab, narration_anchor_point);

            ////Debug.Log(node_data.GetCharacter());

            //narration.GetComponentInChildren<TextMeshProUGUI>().text = $"{node_data.GetCharacter()}: {_dlog}";

            //ResizeButtonToTextureScale(narration.GetComponent<UnityEngine.UI.Button>(), 4);

            //ScaleButtonWithText(narration.GetComponentInChildren<UnityEngine.UI.Button>());
        }

        private void OnChoiceButtonClick(int _index)
        {
            Debug.Log("Choice button clicked!");

            node_data.OnOptionChosen(_index);
        }

        #endregion


        #region Sprite Methods
        #region Character Methods
        private void InstantiateNewCharacter(int _index)
        {
            //if (_index < 0)
            //{

            //    return;
            //}

            // Ensure index is within range
            EnsureIndexWithinRange(_index, character_sprites);

            Destroy(character.gameObject);

            character = Instantiate(image_prefab, character_anchor_point); // Instantiate image object

            character.GetComponent<UnityEngine.UI.Image>().sprite = ConvertTexture2DToSprite(character_sprites[_index]); // Set sprite

            SetAnchors(character);
        }

        #endregion


        #region Background Methods
        private void InstantiateNewBackground(int _index)
        {
            // Ensure index is within range
            EnsureIndexWithinRange(_index, location_sprites);

            Destroy(background.gameObject);

            background = Instantiate(image_prefab, this.transform); // Instantiate image object

            background.GetComponent<UnityEngine.UI.Image>().sprite = ConvertTexture2DToSprite(location_sprites[_index]); // Set sprite

            SetAnchors(background);

            StretchToFillCanvas(background.GetComponent<UnityEngine.UI.Image>(), this.gameObject.GetComponent<Canvas>());

            //background.transform.parent = background_anchor_point;
            background.transform.SetParent(background_anchor_point);
        }

        private void StretchToFillCanvas(UnityEngine.UI.Image _image, Canvas _canvas)
        {
            RectTransform rt = _image.GetComponent<RectTransform>();

            rt.anchorMin = new Vector2(0, 0); // set anchorMin to the center of the parent

            rt.anchorMax = new Vector2(1, 1); // set anchorMax to the center of the parent

            rt.pivot = new Vector2(0.5f, 0.5f); // set pivot to the center of the element

            rt.sizeDelta = new Vector2(0, 0);

            rt.anchoredPosition = new Vector2(0, 0);
        }
        #endregion
        #endregion

        #region Button Methods


        #endregion


        #region Utility Methods

        public void RefreshUI()
        {
            CleanupUI();

            // Instantiate new background
            InstantiateNewBackground(TranslateBackground());

            // Instantiate new character
            InstantiateNewCharacter(TranslateCharacter());

            // Instantiate new choices
            InstantiateChoiceButton(node_data.GetDlogChoices().Count);

            // Instantiate new narration
            UpdateNarrationWindow(node_data.GetDlogText());

            if (node_data.IsThereATip())
            {
                // Show tip button
                tip.SetActive(true);
            }
            else
                // Hide tip button
                tip.SetActive(false);

            //if (node_data.IsThereATip())
            //{
            //    // Instantiate tip button
            //    Destroy(tip.gameObject);
            //    SetTipButtonStatus();
            //}
            //else
            //    // Destroy tip button
            //    Destroy(show_tip_button);


        }

        private void CleanupUI()
        {
            List<GameObject> cleanup = new List<GameObject>();

            foreach (GameObject go in UI_cleanup)
            {
                if (go != null)
                {
                    cleanup.Add(go);
                }
            }

            foreach (GameObject go in cleanup)
            {
                UI_cleanup.Remove(go);
                Destroy(go);
            }
        }

        private Sprite ConvertTexture2DToSprite(Texture2D _tex)
        {
            return Sprite.Create(_tex, new Rect(0, 0, _tex.width, _tex.height), new Vector2(0.5f, 0.5f), 100);
        }

        private void SetAnchors(GameObject _g)
        {
            RectTransform rt = _g.GetComponent<RectTransform>();

            rt.anchorMin = new Vector2(0.5f, 0.5f); // set anchorMin to the center of the parent

            rt.anchorMax = new Vector2(0.5f, 0.5f); // set anchorMax to the center of the parent

            rt.pivot = new Vector2(0.5f, 0.5f); // set pivot to the center of the element
        }

        private bool EnsureIndexWithinRange<_T>(int _index, List<_T> _list)
        {
            if (_index < 0 || _index > _list.Count)
            {
                Debug.LogError("Index out of range");

                return false;
            }

            return true;
        }

        private void ResizeButtonToTextureScale(UnityEngine.UI.Button _button, int _height)
        {
            // Get the size of button texture
            Texture2D texture = _button.GetComponent<UnityEngine.UI.Image>().sprite.texture;

            Vector2 textureSize = new Vector2(texture.width, texture.height * _height);

            // Set size of button to match texture
            RectTransform rectTransform = _button.GetComponent<RectTransform>();

            rectTransform.sizeDelta = textureSize;
        }

        private void ScaleButtonWithText(UnityEngine.UI.Button _button)
        {
            // Get the width of the text
            float text_count = _button.GetComponentInChildren<TextMeshProUGUI>().text.Count();

            // Set the width of the button based on the text width
            RectTransform rect_transform = _button.GetComponent<RectTransform>();

            rect_transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, text_count * 15f);
        }

        private int CompareCharacterStringWithEnum(string _name)
        {             
            foreach (Enums.NodeCharacter cha in (Enums.NodeCharacter[]) Enum.GetValues(typeof(Enums.NodeCharacter)))
            {
                if (cha.ToString() == _name)
                {
                    return (int)cha;
                }
            }

            return -1;
        }

        private int TranslateBackground()
        {
            //Debug.Log(node_data.GetBackground());
            switch(node_data.GetBackground())
            {
                case "Default":
                    return 0;
                case "Background_0":
                    return 1;
                case "Background_1":
                    return 2;
                case "Background_2":
                    return 3;
                case "Background_3":
                    return 4;
                case "Background_4":
                    return 5;
                default:
                    return -1;
            }
        }

        private int TranslateCharacter()
        {
            Debug.Log(node_data.current_character_data_name);
            switch(node_data.current_character_data_name) 
            { 
                case "None":
                    return -1;
                case "Narrator":
                    return 0;
                case "Character_0":
                    return 1;
                case "Character_1":
                    return 2;
                case "Character_2":
                    return 3;
                case "Character_3":
                    return 4;
                case "Character_4":
                    return 5;
                case "Character_5":
                    return 6;
                case "Character_6":
                    return 7;
                case "Character_7":
                    return 8;
                case "Character_8":
                    return 9;
                case "Character_9":
                    return 10;
                default:
                    return -1;
            }
        }

        #endregion
    }
}

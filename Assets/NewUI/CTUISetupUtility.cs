using CT.Data;
using CT.Utilis;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


namespace CT.UI.Engine
{
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
        [SerializeField] private GameObject text_prefab;
        [SerializeField] private GameObject tip_bttn_prefab;
        [SerializeField] private GameObject txt_bttn_prefab;

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

        private void Start()
        {
            background = new GameObject();

            character = new GameObject();

            show_tip_button = new GameObject();

            next_button = new GameObject();

            narration = new GameObject();

            choices_buttons = new List<GameObject>();

            RefreshUI();
        }

        private void Update()
        {
        }

        public void RefreshUI()
        {
            // Instantiate new background
            InstantiateNewBackground(0); // Use index from node

            // Instantiate new character
            InstantiateNewCharacter(0); // Use index from node

            // Instantiate new choices
            InstantiateChoiceButton(node_data.GetDlogChoices().Count); // Use index from node

            // Instantiate new narration
            InstantiateNarrationWindow(node_data.GetDlogText()); // Use text from node
        }


        #region Button Methods
        private void InstantiateRevealTipButton()
        {
            // Instantiate tip button and place at fixed position on the screen
            show_tip_button = Instantiate(txt_bttn_prefab, tip_anchor_point);

            show_tip_button.GetComponentInChildren<TextMeshProUGUI>().text = "";

            ResizeButtonToTextureScale(show_tip_button.GetComponent<UnityEngine.UI.Button>(), 1);
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

                button.GetComponent<TextMeshProUGUI>().text = choices[index].text;

                button.name = index.ToString();

                choices_buttons.Add(button);

                button.transform.position += new Vector3(0.0f, i * 35.0f, 0.0f);

                //ResizeButtonToTextureScale(button.GetComponent<UnityEngine.UI.Button>(), 2);

                //ScaleButtonWithText(button.GetComponentInChildren<UnityEngine.UI.Button>());

                // This is an absolute hack and a half ngl
                button.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(() => OnChoiceButtonClick(Int32.Parse(button.name)));

                //button.name = index.ToString();
            }
        }

        private void InstantiateNarrationWindow(string _dlog)
        {
            Destroy(narration);

            narration = Instantiate(text_prefab, narration_anchor_point);

            //Debug.Log(node_data.GetCharacter());

            narration.GetComponentInChildren<TextMeshProUGUI>().text = $"{node_data.GetCharacter()}: {_dlog}";

            //ResizeButtonToTextureScale(narration.GetComponent<UnityEngine.UI.Button>(), 4);

            //ScaleButtonWithText(narration.GetComponentInChildren<UnityEngine.UI.Button>());
        }

        #endregion


        #region Sprite Methods
        #region Character Methods
        private void InstantiateNewCharacter(int _index)
        {
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
        private void OnChoiceButtonClick(int _index)
        {
            // Yes, this is a hack.
            Debug.Log("Choice button clicked!");
            
            node_data.OnOptionChosen(_index);
        }

        #endregion


        #region Utility Methods
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
            foreach (NodeCharacter cha in (NodeCharacter[]) Enum.GetValues(typeof(NodeCharacter)))
            {
                if (cha.ToString() == _name)
                {
                    return (int)cha;
                }
            }

            return -1;
        }

        #endregion
    }
}

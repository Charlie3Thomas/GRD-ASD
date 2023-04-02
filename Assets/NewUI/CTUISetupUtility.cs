using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace CT.UI.Engine
{
    public class CTUISetupUtility : MonoBehaviour
    {
        /*
         
        REFERENCE TO ASSETS FOR TEN DIFFERETN CHARACTERS
        narrator (no asset required)
        Character_0 (asset - unique to each scenario)
        Character_1 (asset - unique to each scenario)
        Character_2 (asset - unique to each scenario)
        Character_3 (asset - unique to each scenario)
        Character_4 (asset - unique to each scenario)
        Character_5 (asset - unique to each scenario)
        Character_6 (asset - unique to each scenario)
        Character_7 (asset - unique to each scenario)
        Character_8 (asset - unique to each scenario)
        Character_9 (asset - unique to each scenario)

        REERENCE TO ASSETS FOR FIVE DIFFERENT LOCATION BACKGROUNDS
        Location_0 (asset - unique to each scenario)
        Location_1 (asset - unique to each scenario)
        Location_2 (asset - unique to each scenario)
        Location_3 (asset - unique to each scenario)
        Location_4 (asset - unique to each scenario)

        REFERENCE TO ASSETS FOR UI ELEMENTS
        text_box
        tip_text_box         
        show_tip_ui
        choice_button
        next_button
         */

        // Character sprites
        [SerializeField] private List<Texture2D> character_sprites;

        // Location sprites
        [SerializeField] private List<Texture2D> location_sprites;

        // UI Prefab Elements
        //[SerializeField] TMPro.TextMeshProUGUI text_box;        
        //[SerializeField] TMPro.TextMeshProUGUI tip_text_box;    
        //[SerializeField] UnityEngine.UI.Button next_button;     
        //[SerializeField] UnityEngine.UI.Button choice_button;   
        //[SerializeField] UnityEngine.UI.Button show_tip_ui;

        // Prefabs
        [Header("UI Prefabs")]
        [SerializeField] private GameObject image_prefab;
        [SerializeField] private GameObject text_prefab;
        [SerializeField] private GameObject button_prefab;

        // Anchor Points
        [Header("Anchor Points")]
        [SerializeField] private Transform background_anchor_point;
        [SerializeField] private Transform character_anchor_point;
        [SerializeField] private Transform tip_anchor_point;
        [SerializeField] private Transform dlog_choices_centre;

        // UI Elements
        private GameObject background;
        private GameObject character;
        private GameObject show_tip_button;
        private GameObject next_button;
        private List<GameObject> choices_buttons;

        private void Start()
        {
            background = new GameObject();
            character = new GameObject();
            show_tip_button = new GameObject();
            next_button = new GameObject();
            choices_buttons = new List<GameObject>();

            InstantiateNewBackground(0);
            InstantiateNewCharacter(0);
            InstantiateRevealTipButton();
            InstantiateChoiceButton(5);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                int index = Random.Range(0, location_sprites.Count);
                InstantiateNewBackground(index);
                index = Random.Range(0, character_sprites.Count);
                InstantiateNewCharacter(index);
                index = Random.Range(1, 6);
                InstantiateChoiceButton(index);
            }
        }

        #region Button Methods
        private void InstantiateRevealTipButton()
        {
            // Instantiate tip button and place at fixed position on the screen
            show_tip_button = Instantiate(button_prefab, tip_anchor_point);
            show_tip_button.GetComponentInChildren<TextMeshProUGUI>().text = "";
            ResizeButtonToTextureScale(show_tip_button.GetComponent<UnityEngine.UI.Button>());
        }

        private void InstantiateNextDialogueButton()
        {
            // Instantiate next button and place at fixed position on the screen
            next_button = Instantiate(button_prefab, dlog_choices_centre);

            ResizeButtonToTextureScale(next_button.GetComponent<UnityEngine.UI.Button>());
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

            for (int i = 0; i < _button_count; i++)
            {
                // Add offset to dlog_choices_centre
                GameObject button = Instantiate(button_prefab, dlog_choices_centre);
                button.GetComponentInChildren<TextMeshProUGUI>().text = "";
                choices_buttons.Add(button);
                button.transform.position += new Vector3(0.0f, i * 35.0f, 0.0f);
                ResizeButtonToTextureScale(button.GetComponent<UnityEngine.UI.Button>());
            }
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
            background.transform.parent = background_anchor_point;
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

        private void ResizeButtonToTextureScale(UnityEngine.UI.Button _button)
        {
            // Get the size of button texture
            Texture2D texture = _button.GetComponent<UnityEngine.UI.Image>().sprite.texture;
            Vector2 textureSize = new Vector2(texture.width, texture.height);

            // Set size of button to match texture
            RectTransform rectTransform = _button.GetComponent<RectTransform>();
            rectTransform.sizeDelta = textureSize;
        }

        #endregion
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CT
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
        //
        [SerializeField] GameObject character_image;
        [SerializeField] GameObject location_image;

        private GameObject background;


        private void Start()
        {
            background = new GameObject();
            InstantiateNewBackground(0);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                int index = Random.Range(0, location_sprites.Count);
                InstantiateNewBackground(index);
            }
        }

        private void InstantiateNewBackground(int _index)
        {
            // Ensure index is within range
            if (_index < 0 || _index > location_sprites.Count)
            {
                Debug.LogError("Index out of range");
                return;
            }

            Destroy(background.gameObject);

            background = Instantiate(character_image, this.transform); // Instantiate image object
            background.GetComponent<Image>().sprite = ConvertTexture2DToSprite(location_sprites[_index]); // Set sprite
            SetAnchors(background);
            StretchToFillCanvas(background.GetComponent<Image>(), this.gameObject.GetComponent<Canvas>());
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

        private void StretchToFillCanvas(Image _image, Canvas _canvas)
        {
            RectTransform rt = _image.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 0); // set anchorMin to the center of the parent
            rt.anchorMax = new Vector2(1, 1); // set anchorMax to the center of the parent
            rt.pivot = new Vector2(0.5f, 0.5f); // set pivot to the center of the element
            rt.sizeDelta = new Vector2(0, 0);
            rt.anchoredPosition = new Vector2(0, 0);
        }
    }
}

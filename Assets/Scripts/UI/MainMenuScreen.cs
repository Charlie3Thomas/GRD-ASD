using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Aspie.Sound;

namespace Aspie.UI
{
    public class MainMenuScreen : MonoBehaviour
    {
        [SerializeField]
        private Image bgImage;
        [SerializeField]
        private Sprite[] bgSprites;
        [SerializeField]
        private Animator bgAnimator;
        [SerializeField]
        private Button playStoryButton;
        [SerializeField]
        private Button settingsButton;
        [SerializeField]
        private Button helpButton;
        [SerializeField]
        private Button exitButton;
        [SerializeField]
        private StorySelectorScreen storySelectorScreen;
        [SerializeField]
        private SettingsScreen settingsScreen;

        private int currentSprite = 0;
        private bool fade = false;
        private float alpha = 0f;
        public Action OnPlayStoryClicked;
        public Action OnSettingsClicked;
        public Action OnHelpClicked;
        public Action<Story> OnStorySelected;

        void Start()
        {
            playStoryButton.onClick.AddListener(playStoryButtonClicked);
            settingsButton.onClick.AddListener(settingsButtonClicked);
            helpButton.onClick.AddListener(helpButtonClicked);
            exitButton.onClick.AddListener(exitButtonClicked);

            storySelectorScreen.OnStorySelected += playStory;

            InvokeRepeating(nameof(changeBGSprite), 0f, 5f);
        }

        private void Update()
        {
            if(fade)
            {
                alpha += Time.deltaTime;
                bgImage.color = new Color(1f, 1f, 1f, alpha);
                if (alpha >= 1f)
                {
                    alpha = 0f;
                    fade = false;
                }
            }
        }

        private void playStoryButtonClicked()
        {
            AudioService.Instance.PlayButtonSound();
            storySelectorScreen.gameObject.SetActive(true);
        }

        private void settingsButtonClicked()
        {
            AudioService.Instance.PlayButtonSound();
            settingsScreen.gameObject.SetActive(true);
        }

        private void helpButtonClicked()
        {
            AudioService.Instance.PlayButtonSound();
            OnHelpClicked?.Invoke();
        }
        private void exitButtonClicked()
        {
            AudioService.Instance.PlayButtonSound();
            Application.Quit();
        }

        private void playStory(Story s)
        {
            storySelectorScreen.gameObject.SetActive(false);
            Debug.Log("Playing story : " + s.SceneName);
            OnStorySelected?.Invoke(s);
            SceneManager.LoadScene(s.SceneName, LoadSceneMode.Additive);
        }

        private void changeBGSprite()
        {
            alpha = 0f;
            bgImage.color = new Color(1f, 1f, 1f, alpha);
            fade = true;
            bgImage.sprite = bgSprites[currentSprite];
            currentSprite++;
            if (currentSprite == bgSprites.Length)
            {
                currentSprite = 0;
            }
        }
    }
}

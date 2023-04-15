using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Aspie.UI
{
    public class MainMenuScreen : MonoBehaviour
    {
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

        public Action OnPlayStoryClicked;
        public Action OnSettingsClicked;
        public Action OnHelpClicked;
        public Action<StoryView> OnStorySelected;

        void Start()
        {
            playStoryButton.onClick.AddListener(playStoryButtonClicked);
            settingsButton.onClick.AddListener(settingsButtonClicked);
            helpButton.onClick.AddListener(helpButtonClicked);
            exitButton.onClick.AddListener(exitButtonClicked);

            storySelectorScreen.OnStorySelected += playStory;
        }

        private void playStoryButtonClicked()
        {
            storySelectorScreen.gameObject.SetActive(true);
        }

        private void settingsButtonClicked()
        {
            settingsScreen.gameObject.SetActive(true);
        }

        private void helpButtonClicked()
        {
            OnHelpClicked?.Invoke();
        }
        private void exitButtonClicked()
        {
            Debug.Log("EXIT CLICK");
        }

        private void playStory(StoryView sv)
        {
            storySelectorScreen.gameObject.SetActive(false);
            Debug.Log("Playing story : " + sv.name);
            OnStorySelected?.Invoke(sv);
        }
    }
}

using Aspie.Sound;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Aspie.UI
{
    public class GameOverScreen : MonoBehaviour
    {
        [SerializeField]
        private Button mainMenuButton;
        [SerializeField]
        private Button quitButton;

        public Action OnMainMenuRequest;
        public Action OnGameQuitRequest;

        void Start()
        {
            mainMenuButton.onClick.AddListener(mainMenuClicked);
            quitButton.onClick.AddListener(quitClicked);
        }

        private void mainMenuClicked()
        {
            AudioService.Instance.PlayButtonSound();
            OnMainMenuRequest?.Invoke();
        }
        private void quitClicked()
        {
            AudioService.Instance.PlayButtonSound();
            OnGameQuitRequest?.Invoke();
        }
    }
}

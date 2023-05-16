using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aspie.UI
{
    public class InGameScreen : MonoBehaviour
    {
        private bool isPaused = false;
        [SerializeField]
        private PauseMenuScreen pauseMenu;
        public Action OnMainMenuRequest;
        void Start()
        {
            pauseMenu.OnMainMenuClicked += gotoMainMenu;
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                togglePauseMenu();
            }
        }

        private void togglePauseMenu()
        {
            isPaused = !pauseMenu.gameObject.activeSelf;
            pauseMenu.gameObject.SetActive(isPaused);
        }

        private void gotoMainMenu()
        {
            pauseMenu.gameObject.SetActive(false);
            OnMainMenuRequest?.Invoke();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aspie.UI
{
    public class UIController : MonoBehaviour
    {
        public MainMenuScreen MainMenu;
        public InGameScreen InGame;
        
        public GameOverScreen GameOver;

        void Start()
        {
            showMainMenu();
        }

        private void showMainMenu()
        {
            MainMenu.gameObject.SetActive(true);
        }
        private void showHelp()
        {
            //MainMenu.gameObject.SetActive(false);
        }
    }
}

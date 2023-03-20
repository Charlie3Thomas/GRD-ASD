using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue.UI
{
    public class SplashScreenManager : MonoBehaviour
    {
        [Header("RESOURCES")]
        public GameObject splashScreen;
        public GameObject mainPanels;
        public GameObject homePanel;
        private Animator splashScreenAnimator;
        private Animator mainPanelsAnimator;
        private Animator homePanelAnimator;

        [Header("SETTINGS")]
        public bool disableSplashScreen;

        [Header("Background")]
        public Sprite[] backgroundImages;
        public Image backgroundImage;

        private int currentSprite;


        void Start()
        {
            if (disableSplashScreen == true)
            {
                splashScreen.SetActive(true);
                splashScreenAnimator = splashScreen.GetComponent<Animator>();
                splashScreenAnimator.Play("Splash Out");
                mainPanels.SetActive(true);

                mainPanelsAnimator = mainPanels.GetComponent<Animator>();
                mainPanelsAnimator.Play("Splash Disabled");
                homePanelAnimator = homePanel.GetComponent<Animator>();
                homePanelAnimator.Play("Panel In");
            }

            else
            {
                splashScreen.SetActive(true);
                // mainPanels.SetActive(false);
            }

            backgroundImage.sprite = backgroundImages[0];
            currentSprite = 0;
            StartCoroutine(ChangeImageRoutine());
        }

        private IEnumerator ChangeImageRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(7f);
                currentSprite++;
                if (currentSprite >= backgroundImages.Length)
                {
                    currentSprite = 0;
                }
                backgroundImage.sprite = backgroundImages[currentSprite];

                StartCoroutine(ChangeImageFade());
            }
        }

        IEnumerator ChangeImageFade()
        {
            float alpha = 0f;
            
            while (alpha < 1f)
            {
                backgroundImage.sprite = backgroundImages[currentSprite];
                backgroundImage.color = new Color(1f, 1f, 1f, alpha);
                alpha += 0.45f * Time.deltaTime;
                yield return new WaitForSeconds(0.45f * Time.deltaTime);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Aspie.UI
{
    public class UIController : MonoBehaviour
    {
        public MainMenuScreen MainMenu;
        public InGameScreen InGame;
        
        public GameOverScreen GameOver;
        private AsyncOperation sceneAsync;

        private Scene mainScene;
        private Scene inGameScene;

        private void Awake()
        {
            //DontDestroyOnLoad(this);
            mainScene = SceneManager.GetActiveScene();
        }
        void Start()
        {
            inGameScene = mainScene;
            showMainMenu();
            MainMenu.OnStorySelected += loadSceneStory;
            InGame.OnMainMenuRequest += showMainMenu;
            CT.Utilis.CTNodeIOUtility.OnEndNoteReached += showGameOver;
            GameOver.OnMainMenuRequest += showMainMenu;
        }

        private void showMainMenu()
        {
            SceneManager.MoveGameObjectToScene(this.transform.root.gameObject, mainScene);
            GameOver.gameObject.SetActive(false);
            InGame.gameObject.SetActive(false);
            MainMenu.gameObject.SetActive(true);
            if(inGameScene != mainScene)
            {
                SceneManager.UnloadSceneAsync(inGameScene);
            }
        }
        private void showIngame()
        {
            MainMenu.gameObject.SetActive(false);
            InGame.gameObject.SetActive(true);
        }
        private void showGameOver()
        {
            InGame.gameObject.SetActive(false);
            GameOver.gameObject.SetActive(true);
        }
        private void loadSceneStory(Story story)
        {
            StartCoroutine(loadScene(story.SceneName));
            showIngame();
        }
        IEnumerator loadScene(string sceneName)
        {
            AsyncOperation scene = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            scene.allowSceneActivation = false;
            sceneAsync = scene;

            //Wait until we are done loading the scene
            while (scene.progress < 0.9f)
            {
                yield return null;
            }
            OnFinishedLoadingAllScene(sceneName);
        }

        private void enableScene(string sceneName)
        {
            //Activate the Scene
            inGameScene = mainScene;
            sceneAsync.allowSceneActivation = true;
            inGameScene = SceneManager.GetSceneByName(sceneName);
            if (inGameScene.IsValid())
            {
                SceneManager.MoveGameObjectToScene(this.transform.root.gameObject, inGameScene);
                SceneManager.SetActiveScene(inGameScene);
            }
        }

        void OnFinishedLoadingAllScene(string sceneName)
        {
            Debug.Log("Done Loading Scene");
            enableScene(sceneName);
        }
    }
}

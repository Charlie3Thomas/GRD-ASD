using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using Aspie.Sound;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Aspie.Language;

namespace Aspie.UI
{
    
    public class StorySelectorScreen : MonoBehaviour
    {
        [SerializeField]
        private Button closeButton;
        [SerializeField]
        private GameObject storyContainer;
        [SerializeField]
        private StoryView storyPrefab;
        private List<StoryView> storyViewList = new List<StoryView>();

        public Action<Story> OnStorySelected;
        void Start()
        {
            closeButton.onClick.AddListener(closeClicked);
        }

        private void OnEnable()
        {
            instantiateStories();
        }

        private void closeClicked()
        {
            AudioService.Instance.PlayButtonSound();
            gameObject.SetActive(false);
        }
        private void selectStory(Story s)
        {
            Debug.Log("Story Selected :: " + s.SceneName);
            AudioService.Instance.PlayButtonSound();
            OnStorySelected?.Invoke(s);
        }
        private void instantiateStories()
        {
            List<Story> currentStory = LocalizationManager.Instance.IsFrench ? GameDataManager.Instance.FrStoryList : GameDataManager.Instance.EnStoryList;
            for(int i=0; i<currentStory.Count; i++)
            {
                StoryView sv;
                if (i >= storyViewList.Count)
                {
                    sv = Instantiate(storyPrefab, storyContainer.transform);
                    storyViewList.Add(sv);
                }
                else
                {
                    sv = storyViewList[i];
                }
                sv.OnStorySelected += selectStory;
                sv.UdpateStory(currentStory[i]);
            }
        }
    }
}

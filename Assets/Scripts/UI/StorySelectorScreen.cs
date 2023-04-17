using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

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
        private List<StoryView> storyViewList;

        public Action<StoryView> OnStorySelected;
        void Start()
        {
            storyViewList = new List<StoryView>();
            instantiateStories();
            closeButton.onClick.AddListener(closeClicked);
        }

        private void closeClicked()
        {
            gameObject.SetActive(false);
        }
        private void selectStory(StoryView sv)
        {
            Debug.Log("Story Selected :: " + sv.name);
            OnStorySelected?.Invoke(sv);
        }
        private void instantiateStories()
        {
            foreach(Story s in GameDataManager.Instance.StoryList)
            {
                StoryView sv = Instantiate(storyPrefab, storyContainer.transform);
                storyViewList.Add(sv);
                sv.OnStorySelected += selectStory;
                sv.UdpateStory(s);
            }
        }
    }
}
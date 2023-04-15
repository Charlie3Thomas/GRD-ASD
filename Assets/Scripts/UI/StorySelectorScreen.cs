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
        private List<StoryView> storyList;

        public Action<StoryView> OnStorySelected;

        void Start()
        {
            storyList = new List<StoryView>();
            storyList = storyContainer.GetComponentsInChildren<StoryView>().ToList();
            foreach(StoryView sv in storyList)
            {
                sv.OnStorySelected += selectStory;
            }
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
    }
}

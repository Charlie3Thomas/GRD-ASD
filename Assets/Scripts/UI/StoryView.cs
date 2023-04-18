using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Aspie.UI
{
    public class StoryView : MonoBehaviour
    {
        [SerializeField]
        private Button selectButton;
        [SerializeField]
        private TextMeshProUGUI titleText;
        [SerializeField]
        private TextMeshProUGUI contentText;
        [SerializeField]
        private Image storyImage;

        public Action<StoryView> OnStorySelected;

        void Start()
        {
            selectButton.onClick.AddListener(selectClicked);
        }

        private void selectClicked()
        {
            OnStorySelected?.Invoke(this);
        }
        public void UdpateStory(Story story)
        {
            gameObject.name = story.Title;
            titleText.text = story.Title;
            contentText.text = story.Context;
            storyImage.sprite = story.StorySprite;
        }
    }
}

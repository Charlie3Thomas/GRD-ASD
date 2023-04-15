using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Aspie.UI
{
    public class StoryView : MonoBehaviour
    {
        [SerializeField]
        private Button selectButton;

        public Action<StoryView> OnStorySelected;

        void Start()
        {
            selectButton.onClick.AddListener(selectClicked);
        }

        private void selectClicked()
        {
            OnStorySelected?.Invoke(this);
        }
    }
}

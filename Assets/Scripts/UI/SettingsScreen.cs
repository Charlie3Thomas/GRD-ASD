using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Aspie.UI
{
    public class SettingsScreen : MonoBehaviour
    {
        [SerializeField]
        private Button closeButton;
        void Start()
        {
            closeButton.onClick.AddListener(closeClicked);
        }

        private void closeClicked()
        {
            gameObject.SetActive(false);
        }
    }
}

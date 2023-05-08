using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Aspie.Sound;

namespace Aspie.UI
{
    public class SettingsScreen : MonoBehaviour
    {
        [SerializeField]
        private Button closeButton;
        [SerializeField]
        private Slider masterVolSlider;
        [SerializeField]
        private Slider musicVolSlider;
        [SerializeField]
        private Slider effectsVolSlider;

        void Start()
        {
            closeButton.onClick.AddListener(closeClicked);
            masterVolSlider.onValueChanged.AddListener(onMasterVolUpdate);
            musicVolSlider.onValueChanged.AddListener(onMusicVolUpdate);
            effectsVolSlider.onValueChanged.AddListener(onEffectsVolUpdate);

            musicVolSlider.value = AudioService.Instance.BGVolLevel;
            effectsVolSlider.value = AudioService.Instance.SFXVolLevel;

            masterVolSlider.value = Mathf.Max(AudioService.Instance.BGVolLevel, AudioService.Instance.SFXVolLevel);
        }

        private void closeClicked()
        {
            AudioService.Instance.PlayButtonSound();
            gameObject.SetActive(false);
        }

        private void onMasterVolUpdate(float val)
        {
            AudioService.Instance.SetBGVolume(val);
            AudioService.Instance.SetSFXVolume(val);

            musicVolSlider.value = AudioService.Instance.BGVolLevel;
            effectsVolSlider.value = AudioService.Instance.SFXVolLevel;
        }
        private void onMusicVolUpdate(float val)
        {
            AudioService.Instance.SetBGVolume(val);
        }
        private void onEffectsVolUpdate(float val)
        {
            AudioService.Instance.SetSFXVolume(val);
        }
    }
}

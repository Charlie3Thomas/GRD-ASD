using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Aspie.Sound;
using Aspie.Language;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Aspie.UI
{
    public class SettingsScreen : MonoBehaviour
    {
        [SerializeField]
        private TMP_Dropdown languageDropdown;
        [SerializeField]
        private Button closeButton;
        [SerializeField]
        private Slider masterVolSlider;
        [SerializeField]
        private Slider musicVolSlider;
        [SerializeField]
        private Slider effectsVolSlider;

        [SerializeField]
        private TMP_Dropdown audioSelector;

        void Start()
        {
            closeButton.onClick.AddListener(closeClicked);
            masterVolSlider.onValueChanged.AddListener(onMasterVolUpdate);
            musicVolSlider.onValueChanged.AddListener(onMusicVolUpdate);
            effectsVolSlider.onValueChanged.AddListener(onEffectsVolUpdate);

            musicVolSlider.value = AudioService.Instance.BGVolLevel;
            effectsVolSlider.value = AudioService.Instance.SFXVolLevel;

            masterVolSlider.value = Mathf.Max(AudioService.Instance.BGVolLevel, AudioService.Instance.SFXVolLevel);
            setUpAudioClipDropDown();
            audioSelector.onValueChanged.AddListener(onBgAudioUpdate);
            languageDropdown.onValueChanged.AddListener(onLanguageUpdate);
        }

        private void setUpAudioClipDropDown()
        {
            List<string> clips = new List<string>();
            foreach (var c in AudioService.Instance.BGSounds)
                clips.Add(c.name);
            audioSelector.AddOptions(clips);
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
        private void onBgAudioUpdate(int id)
        {
            AudioService.Instance.SetBGClip(id);
        }
        private void onLanguageUpdate(int id)
        {
            LocalizationManager.Instance.UpdateLanguage(id);
        }
    }
}

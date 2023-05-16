using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Aspie.Sound;
using System;

namespace Aspie.UI
{
    public class PauseMenuScreen : MonoBehaviour
    {
        [SerializeField]
        private Button closeButton;
        [SerializeField]
        private Slider masterVolSlider;
        [SerializeField]
        private Slider musicVolSlider;
        [SerializeField]
        private Slider effectsVolSlider;
        [SerializeField]
        private Button mainMenuButton;

        [SerializeField]
        private TMP_Dropdown audioSelector;

        public Action OnMainMenuClicked;

        void Start()
        {
            closeButton.onClick.AddListener(closeClicked);
            mainMenuButton.onClick.AddListener(onMainMenuButtonClicked);
            masterVolSlider.onValueChanged.AddListener(onMasterVolUpdate);
            musicVolSlider.onValueChanged.AddListener(onMusicVolUpdate);
            effectsVolSlider.onValueChanged.AddListener(onEffectsVolUpdate);

            musicVolSlider.value = AudioService.Instance.BGVolLevel;
            effectsVolSlider.value = AudioService.Instance.SFXVolLevel;

            masterVolSlider.value = Mathf.Max(AudioService.Instance.BGVolLevel, AudioService.Instance.SFXVolLevel);
            setUpAudioClipDropDown();
            audioSelector.onValueChanged.AddListener(onBgAudioUpdate);
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
        private void onMainMenuButtonClicked()
        {
            OnMainMenuClicked?.Invoke();
        }
    }
}

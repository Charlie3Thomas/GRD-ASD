using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aspie.Sound
{
    public class AudioService : MonoBehaviour
    {

        public AudioSource BgSource;
        public AudioSource SfxSource;

        public AudioClip ButtonSound;
        public AudioClip[] BGSounds;
        public static AudioService Instance;

        private const string bgVolKey = "BG_VOL";
        private const string sfxVolKey = "SFX_VOL";

        public float BGVolLevel => PlayerPrefs.GetFloat(bgVolKey);
        public float SFXVolLevel => PlayerPrefs.GetFloat(sfxVolKey);

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            SetBGVolume(BGVolLevel);
            SetSFXVolume(SFXVolLevel);
            BgSource.Play();
        }

        public void PlayButtonSound()
        {
            SfxSource.PlayOneShot(ButtonSound);
        }

        public void SetBGClip(int id)
        {
            BgSource.clip = BGSounds[id];
            BgSource.Play();
        }
        public void StartBGMusic(bool flag)
        {
            if (flag)
                BgSource.Play();
            else
                BgSource.Stop();
        }

        public void SetBGVolume(float val)
        {
            BgSource.volume = val;
            PlayerPrefs.SetFloat(bgVolKey, val);
        }
        public void SetSFXVolume(float val)
        {
            SfxSource.volume = val;
            PlayerPrefs.SetFloat(sfxVolKey, val);
        }
    }
}


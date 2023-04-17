using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aspie
{
    [Serializable]
    public class Story
    {
        public string Title;
        public string Context;
        public Sprite StorySprite;
    }
    public class GameDataManager : MonoBehaviour
    {
        private static GameDataManager _instance;
        public static GameDataManager Instance { get { return _instance; } }
        public List<Story> StoryList;
        void Awake()
        {
            if (_instance == null || _instance != this)
                _instance = this;
        }

    }
}

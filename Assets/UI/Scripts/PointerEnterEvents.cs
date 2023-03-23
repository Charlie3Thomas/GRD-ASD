﻿using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Dialogue.UI
{
    public class PointerEnterEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("EVENTS")]
        public UnityEvent enterEvent;
        public UnityEvent exitEvent;

        public void OnPointerEnter(PointerEventData eventData)
        {
            enterEvent.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            exitEvent.Invoke();
        }
    }
}
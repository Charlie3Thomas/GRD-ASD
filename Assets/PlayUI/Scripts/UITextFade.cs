using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CT
{
    public class UITextFade : MonoBehaviour
    {
        public float fade_time = 0.5f; 
        private TextMeshProUGUI tmp_text; 

        void Start()
        {
            tmp_text = GetComponent<TextMeshProUGUI>();
            Color c = tmp_text.color;
            c.a = 0f;
            tmp_text.color = c;
            StartCoroutine(Fade());
        }

        IEnumerator Fade()
        {
            yield return new WaitForSeconds(Int32.Parse(this.transform.parent.gameObject.name) * 0.25f);

            yield return new WaitForSeconds(0.5f);

            float interval = fade_time / 100f;

            for (float alpha = 0f; alpha <= 1f; alpha += 0.01f)
            {
                Color c = tmp_text.color;
                c.a = alpha;
                tmp_text.color = c;
                yield return new WaitForSeconds(interval);
            }
        }
    }
}

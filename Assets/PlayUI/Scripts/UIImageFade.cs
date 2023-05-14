using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CT
{
    public class UIImageFade : MonoBehaviour
    {
        public float fade_time = 0.25f; 
        private Image image; 

        void Start()
        {
            image = GetComponent<Image>();
            Color c = image.color;
            c.a = 0f;
            image.color = c;
            StartCoroutine(Fade());
        }

        IEnumerator Fade()
        {
            yield return new WaitForSeconds(Int32.Parse(this.transform.parent.gameObject.name) * 0.25f);

            yield return new WaitForSeconds(0.5f);

            float interval = fade_time / 100f;

            for (float alpha = 0f; alpha <= 1f; alpha += 0.01f)
            {
                Color c = image.color;
                c.a = alpha;
                image.color = c;
                yield return new WaitForSeconds(interval);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CT
{
    public class SetParentChildStatus : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(GetLineCount());
        }

        private IEnumerator GetLineCount()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform.parent.GetComponent<TextMeshProUGUI>().rectTransform);

            yield return null;

            int line_count = this.transform.parent.GetComponent<TextMeshProUGUI>().textInfo.lineCount;

            Debug.Log(line_count);

            this.transform.SetParent(this.transform.parent.parent, true);
            this.transform.SetAsFirstSibling();

            this.GetComponent<RectTransform>().sizeDelta = new Vector2(600, 20 * line_count);

            Debug.Log($"{this.gameObject.name} + GetLineCount");
        }
    }
}

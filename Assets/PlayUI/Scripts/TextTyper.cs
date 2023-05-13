using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CT
{
    public class TextTyper : MonoBehaviour
    {
        [SerializeField] private TMP_Text tmp_object;
        [SerializeField] private string text_to_type;
        [SerializeField] private float type_speed = 0.05f;
        private int index = 0;
        private bool is_typing = false;

        void Start()
        {
            
        }

        public void StartTypingText(string _input)
        {
            //Debug.Log(_input);
            text_to_type = _input;
            index = 0;
            tmp_object.text = "";
            is_typing = true;
            StartCoroutine(Type());
        }

        IEnumerator Type()
        {
            while (is_typing)
            {
                if (index < text_to_type.Length)
                {
                    tmp_object.text += text_to_type[index];
                    index++;
                    yield return new WaitForSeconds(type_speed);
                }
                else
                {
                    is_typing = false;
                }
            }
        }
    }
}

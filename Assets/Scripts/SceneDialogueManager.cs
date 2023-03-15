using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneDialogueManager : MonoBehaviour
{
    [SerializeField]
    private List<ResponseObjectView> responseList;
    [SerializeField]
    private Text questionText;
    public Dialogue dailogue;
    void Start()
    {
        LoadQuestion(1);
    }

    void Update()
    {
        
    }
    public void GotoTarget(int id)
    {
        LoadQuestion(id);
    }

    private async void LoadQuestion(int id)
    {
        if(id == 0)
        {
            questionText.text = "END OF CONVERSATION";
            return;
        }
        dailogue = DialogueManager.Instance.DialogueList.Find(x => x.Id == id);
        questionText.text = dailogue.Question;
        for(int i=0; i<dailogue.Options.Count; i++)
        {
            if(!string.IsNullOrEmpty(dailogue.Options[i]))
            {
                responseList[i].gameObject.SetActive(true);
                responseList[i].SetData(dailogue.Options[i], dailogue.Goto[i], this);
            }
            else
            {
                responseList[i].gameObject.SetActive(false);
            }
        }
        if(dailogue.JumpTo > 0)
        {
            await System.Threading.Tasks.Task.Delay(3000);
            LoadQuestion(dailogue.JumpTo);
        }
    }
}

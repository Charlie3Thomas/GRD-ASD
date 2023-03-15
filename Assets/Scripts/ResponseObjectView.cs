using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResponseObjectView : MonoBehaviour
{
    private Button submitButton;
    private Text resposeText;
    private int targetId;
    private SceneDialogueManager sceneDialogueManager;

    void Start()
    {
        submitButton = this.GetComponent<Button>();
        resposeText = this.GetComponentInChildren<Text>();
        submitButton.onClick.AddListener(onSubmitButtonClicked);
    }
    public void SetData(string response, int target, SceneDialogueManager manager)
    {
        resposeText.text = response;
        targetId = target;
        sceneDialogueManager = manager;
    }
    void onSubmitButtonClicked()
    {
        sceneDialogueManager.GotoTarget(targetId);
    }
}

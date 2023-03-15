using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    private string jsonString = "";
    private StreamReader sr;

    public List<Dialogue> DialogueList;

    public static DialogueManager Instance;

    private void Awake()
    {
        Instance = this;
        string path = (Application.streamingAssetsPath + "/scene1.json");
        sr = new StreamReader(path);
        jsonString = sr.ReadToEnd();
        sr.Close();
        jsonString = "{\"DialogueList\":" + jsonString + "}";
        Debug.Log("==>" + jsonString);
        SceneDialogues sceneDialogues = JsonUtility.FromJson<SceneDialogues>(jsonString);
        Debug.Log("Counts :: " + sceneDialogues.DialogueList.Count);
        DialogueList = sceneDialogues.DialogueList;
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}

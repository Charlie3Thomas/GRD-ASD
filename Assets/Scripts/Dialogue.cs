using System.Collections.Generic;
using System;
[Serializable]
public class Dialogue
{
    public int Id;
    public string Question;
    public int JumpTo;
    public List<string> Options;
    public List<int> Goto;
}
[Serializable]
public class SceneDialogues
{
    public List<Dialogue> DialogueList;
}

using UnityEngine;

[System.Serializable]
public class DialogueEnd
{
    public string type;     // dialogue / topdown / none
    public string target;   // id диалога или сцены
    
    public bool IsValid =>
        !string.IsNullOrEmpty(type);
}

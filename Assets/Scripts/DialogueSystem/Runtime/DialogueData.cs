using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueData
{
    public string id;
    public string startNode;
    public List<DialogueNode> nodes;
}

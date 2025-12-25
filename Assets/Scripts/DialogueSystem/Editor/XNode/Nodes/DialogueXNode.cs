using UnityEngine;
using XNode;

namespace DialogueSystem.Editor.XNode
{
    public class DialogueXNode : Node
    {
        [Input] public int input;
        [Output] public int output;
    }
}
using UnityEngine;
using XNode;
using DialogueSystem.Runtime;

namespace DialogueSystem.Editor.XNode
{
    public class DialogueXNode : Node
    {
        public string nodeId;
        public DialogueInputType inputType;
        public bool isEnd;

        [Input] public DialogueXNode input;
        [Output] public DialogueXNode output;

        public override object GetValue(NodePort port)
        {
            return null;
        }
    }
}

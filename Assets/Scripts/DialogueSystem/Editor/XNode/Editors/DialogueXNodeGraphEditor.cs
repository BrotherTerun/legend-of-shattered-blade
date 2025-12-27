using UnityEngine;
using XNode;
using XNodeEditor;

namespace DialogueSystem.Editor.XNode.Editors
{
    [CustomNodeGraphEditor(typeof(DialogueXNodeGraph))]
    public class DialogueXNodeGraphEditor : NodeGraphEditor
    {
        public override Color GetPortColor(NodePort port)
        {
            return Color.white;
        }
    }
}

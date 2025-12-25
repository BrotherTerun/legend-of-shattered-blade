using UnityEngine;
using XNodeEditor;

namespace DialogueSystem.Editor.XNode
{
    [CustomNodeEditor(typeof(DialogueXNode))]
    public class DialogueXNodeEditor : NodeEditor
    {
        public override Color GetTint()
        {
            var node = target as DialogueXNode;

            if (node == null)
                return base.GetTint();

            if (node.isEnd)
                return new Color(0.7f, 0.3f, 0.3f); // финальный Ч красный

            return new Color(0.3f, 0.6f, 0.9f); // обычный Ч синий
        }
    }
}


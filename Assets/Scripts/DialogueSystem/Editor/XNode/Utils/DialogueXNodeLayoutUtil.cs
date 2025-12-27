using DialogueSystem.Runtime;
using UnityEngine;

namespace DialogueSystem.Editor.XNode.Utils
{
    public static class DialogueXNodeLayoutUtil
    {
        public static float EstimateNodeHeight(DialogueXNode node)
        {
            const float baseHeight = 120f;
            const float lineHeight = 18f;
            const int charsPerLine = 60;

            int textLength = string.IsNullOrEmpty(node.text) ? 0 : node.text.Length;
            int lines = Mathf.Max(1, textLength / charsPerLine);

            if (node.inputType == DialogueInputType.WaitingForChoice && node.choices != null)
            {
                lines += node.choices.Count * 2;
            }

            return baseHeight + lines * lineHeight;
        }
    }
}

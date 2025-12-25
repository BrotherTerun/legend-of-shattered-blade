using System.Collections.Generic;

namespace DialogueSystem.Editor.GraphModel
{
    public class EditorDialogueGraph
    {
        public string dialogueId;

        // ¬се ноды диалога по id
        public Dictionary<string, EditorNode> nodes
            = new Dictionary<string, EditorNode>();

        // id стартовой ноды (из JSON)
        public string startNodeId;
    }
}
using System.Collections.Generic;

namespace DialogueSystem.Editor.GraphModel
{
    public class EditorEdge
    {
        public string fromNodeId;
        public string toNodeId;

        public EditorEdgeType type;

        // Для choices
        public string label;

        // На будущее (пока не используем)
        //public List<EditorEffect> effects;
    }
}
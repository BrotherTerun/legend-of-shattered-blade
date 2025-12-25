using System.Collections.Generic;
using DialogueSystem.Editor.GraphModel;

namespace DialogueSystem.Editor.Parsing
{
    public class EditorGraphParseResult
    {
        public EditorDialogueGraph graph;

        public List<EditorGraphParseError> errors
            = new List<EditorGraphParseError>();

        public bool HasErrors => errors.Count > 0;
    }
}

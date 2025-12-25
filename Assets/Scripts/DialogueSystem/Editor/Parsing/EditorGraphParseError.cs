namespace DialogueSystem.Editor.Parsing
{
    public class EditorGraphParseError
    {
        public EditorGraphParseErrorType type;
        public string message;

        public string nodeId;
        public string targetId;
    }
}
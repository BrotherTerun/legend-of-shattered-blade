namespace DialogueSystem.Editor.Parsing
{
    public enum EditorGraphParseErrorType
    {
        DuplicateNodeId,
        MissingNode,
        MissingNextNode,
        MissingChoiceTarget,
        InvalidInputType,
        InvalidEndDefinition,
        MissingStartNode
    }
}

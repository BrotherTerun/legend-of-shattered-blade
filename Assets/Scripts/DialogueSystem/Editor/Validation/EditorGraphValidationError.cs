namespace DialogueSystem.Editor.Validation
{
    public enum EditorGraphValidationErrorType
    {
        GraphIsEmpty,
        NoStartNode,
        NodeHasNoOutgoingEdges,
        EndNodeHasOutgoingEdges,
        UnreachableNode,
        TransitionTargetNotFound,
        EndTargetMissing,
        ConflictingEndTransitions,
        InvalidEndType
    }

    public class EditorGraphValidationError
    {
        public EditorGraphValidationErrorType type;
        public string nodeId;
        public string message;
    }
}
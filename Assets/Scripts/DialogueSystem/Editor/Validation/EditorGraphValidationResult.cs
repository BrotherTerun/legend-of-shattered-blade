using System.Collections.Generic;

namespace DialogueSystem.Editor.Validation
{
    public class EditorGraphValidationResult
    {
        public readonly List<EditorGraphValidationError> errors
            = new List<EditorGraphValidationError>();

        public bool IsValid => errors.Count == 0;

        public void AddError(
            EditorGraphValidationErrorType type,
            string message,
            string nodeId = null)
        {
            errors.Add(new EditorGraphValidationError
            {
                type = type,
                nodeId = nodeId,
                message = message
            });
        }
    }
}
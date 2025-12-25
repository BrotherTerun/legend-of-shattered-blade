using DialogueSystem.Editor.GraphModel;
using System.Collections.Generic;

namespace DialogueSystem.Editor.Validation
{
    public interface IEditorGraphValidator
    {
        IEnumerable<EditorGraphValidationError> Validate(EditorDialogueGraph graph);
    }
}

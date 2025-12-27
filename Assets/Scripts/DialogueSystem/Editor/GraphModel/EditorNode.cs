using System.Collections.Generic;
using DialogueSystem.Runtime;

namespace DialogueSystem.Editor.GraphModel
{
    public class EditorNode
    {
        public string id;

        // Контент
        public string speaker;     // может быть null
        public string text;

        // Управление вводом
        public DialogueInputType input;

        // Структура
        public bool isEnd;
        public DialogueEnd end;

        public List<EditorChoiceData> choices;

        // Исходящие связи
        public List<EditorEdge> edges = new List<EditorEdge>();
    }
}

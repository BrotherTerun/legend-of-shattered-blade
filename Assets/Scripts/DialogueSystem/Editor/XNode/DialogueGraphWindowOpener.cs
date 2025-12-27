using UnityEngine;
using UnityEditor;
using XNodeEditor;

namespace DialogueSystem.Editor.XNode
{
    public static class DialogueGraphWindowOpener
    {
        [MenuItem("Dialogue/Open Dialogue Graph Viewer")]
        public static void OpenGraphWindow()
        {
            // ÂÀÆÍÎ: GetWindow, à íå CreateWindow
            NodeEditorWindow window = EditorWindow.GetWindow<NodeEditorWindow>();

            window.titleContent = new UnityEngine.GUIContent("Dialogue Graph");
            window.Show();
        }
    }
}

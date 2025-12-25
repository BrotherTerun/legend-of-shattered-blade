using DialogueSystem.Editor.Parsing;
using DialogueSystem.Editor.XNode.Builder;
using UnityEditor;
using UnityEngine;

public static class DialogueXNodeBuildTest
{
    [MenuItem("Dialogue/Test/Build XNode Graph")]
    public static void BuildTest()
    {
        var graph = DialogueParserTestRunner.LoadTestGraph();
        if (graph == null)
        {
            Debug.LogError("EditorDialogueGraph not loaded");
            return;
        }

        var xGraph = DialogueXNodeBuilder.Build(graph);
        if (xGraph == null)
            return;

        AssetDatabase.CreateAsset(
            xGraph,
            "Assets/DialogueXNode_Test.asset"
        );

        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = xGraph;
    }
}

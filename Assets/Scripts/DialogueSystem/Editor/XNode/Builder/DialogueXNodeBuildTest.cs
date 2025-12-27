using DialogueSystem.Editor.Parsing;
using DialogueSystem.Editor.XNode;
using DialogueSystem.Editor.XNode.Builder;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class DialogueXNodeBuildTest
{
    private const string GraphsRootFolder = "Assets/DialogueGraphs";

    private static void EnsureGraphsFolderExists()
    {
        if (!AssetDatabase.IsValidFolder(GraphsRootFolder))
        {
            AssetDatabase.CreateFolder("Assets", "DialogueGraphs");
            AssetDatabase.Refresh();
        }
    }

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

        xGraph.name = Path.GetFileNameWithoutExtension(graph.dialogueId);
        string assetPath = $"{GraphsRootFolder}/{xGraph.name}.asset";

        EnsureGraphsFolderExists();

        DialogueXNodeGraph existingGraph = AssetDatabase.LoadAssetAtPath<DialogueXNodeGraph>(assetPath);

        if (existingGraph != null)
        {
            Debug.LogWarning($"Dialogue graph already exists: {assetPath}");
            return;
        }

        AssetDatabase.CreateAsset(xGraph, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = xGraph;
    }
}

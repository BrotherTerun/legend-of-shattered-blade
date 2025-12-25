using UnityEditor;
using UnityEngine;
using DialogueSystem.Editor.Parsing;
using DialogueSystem.Runtime;
using DialogueSystem.Editor.Validation;
using DialogueSystem.Editor.GraphModel;

namespace DialogueSystem.Editor.Parsing
{
    public static class DialogueParserTestRunner
    {
        [MenuItem("Dialogue/Test/Parse Test Dialogue")]
        public static void RunParserTest()
        {
            var graph = LoadTestGraph();
            if (graph == null)
                return;

            Debug.Log($"Dialogue parsed. Nodes: {graph.nodes.Count}");

            var validation = EditorGraphValidator.Validate(graph);

            if (!validation.IsValid)
            {
                Debug.LogWarning($"Validation failed: {validation.errors.Count} issues found");

                foreach (var error in validation.errors)
                {
                    Debug.LogWarning(
                        $"[Validation:{error.type}] Node: {error.nodeId} — {error.message}"
                    );
                }
            }
            else
            {
                Debug.Log("Graph validation passed");
            }
        }

        /// <summary>
        /// 🔑 ЕДИНАЯ точка получения EditorDialogueGraph для editor-инструментов
        /// </summary>
        public static EditorDialogueGraph LoadTestGraph()
        {
            DialogueData testData = LoadTestDialogue("dialogue_02_awakening.json");

            if (testData == null)
            {
                Debug.LogError("Test DialogueData is null");
                return null;
            }

            var result = DialogueJsonToEditorGraphParser.Parse(testData);

            if (result.HasErrors)
            {
                Debug.LogWarning($"Parse finished with {result.errors.Count} errors");

                foreach (var error in result.errors)
                {
                    Debug.LogWarning(
                        $"[{error.type}] Node: {error.nodeId}, Target: {error.targetId} — {error.message}"
                    );
                }
            }

            return result.graph;
        }

        // ⚠️ ВРЕМЕННАЯ заглушка
        private static DialogueData LoadTestDialogue(string dialogueFile)
        {
            if (dialogueFile == null)
            {
                Debug.LogError("LoadTestDialogue() not implemented");
                return null;
            }

            return DialogueLoader.Load(dialogueFile);
        }
    }
}

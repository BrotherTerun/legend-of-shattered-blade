using UnityEditor;
using UnityEngine;
using DialogueSystem.Editor.Parsing;
using DialogueSystem.Runtime;
using DialogueSystem.Editor.Validation;

namespace DialogueSystem.Editor.Parsing
{
    public static class DialogueParserTestRunner
    {
        [MenuItem("Dialogue/Test/Parse Test Dialogue")]
        public static void RunParserTest()
        {
            // ⚠️ ВАЖНО:
            // Здесь ты должен подставить РЕАЛЬНУЮ загрузку JSON,
            // так же, как это делает твой runtime DialogueLoader.

            DialogueData testData = LoadTestDialogue("dialogue_02_awakening.json");

            if (testData == null)
            {
                Debug.LogError("Test DialogueData is null");
                return;
            }

            var result = DialogueJsonToEditorGraphParser.Parse(testData);

            Debug.Log($"Dialogue parsed. Nodes: {result.graph.nodes.Count}");

            if (result.HasErrors)
            {
                Debug.LogWarning($"Parse finished with {result.errors.Count} errors:");

                foreach (var error in result.errors)
                {
                    Debug.LogWarning(
                        $"[{error.type}] Node: {error.nodeId}, Target: {error.targetId} — {error.message}"
                    );
                }
            }
            else
            {
                Debug.Log("Parse finished without errors");
            }

            // Дополнительная проверка рёбер
            foreach (var node in result.graph.nodes.Values)
            {
                Debug.Log($"Node '{node.id}' has {node.edges.Count} edges");
            }

            var validation = EditorGraphValidator.Validate(result.graph);

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

        // ⚠️ ВРЕМЕННАЯ заглушка
        private static DialogueData LoadTestDialogue(string dialogueFile)
        {
            if (dialogueFile == null)
            {
                Debug.LogError("LoadTestDialogue() not implemented");
                return null;
            }
            
            // Тут ты должен вызвать свой существующий DialogueLoader
            // Например:
            return DialogueLoader.Load(dialogueFile);
        }
    }
}

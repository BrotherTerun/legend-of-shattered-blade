using UnityEngine;
using DialogueSystem.Editor.GraphModel;
using DialogueSystem.Editor.Validation;
using DialogueSystem.Editor.XNode;
using UnityEditor;
using XNode;
using System.Collections.Generic;

namespace DialogueSystem.Editor.XNode.Builder
{
    public static class DialogueXNodeBuilder
    {
        public static DialogueXNodeGraph Build(EditorDialogueGraph graph)
        {
            // 1️⃣ Валидация
            var validationResult = EditorGraphValidator.Validate(graph);
            if (!validationResult.IsValid)
            {
                Debug.LogError("Cannot build XNode graph: validation failed");
                foreach (var error in validationResult.errors)
                    Debug.LogError(error.ToString());

                return null;
            }

            // 2️⃣ Создание Graph Asset
            var xGraph = ScriptableObject.CreateInstance<DialogueXNodeGraph>();
            xGraph.name = $"DialogueGraph_{graph.startNodeId}";

            // 3️⃣ Создание XNode-нод
            foreach (var editorNode in graph.nodes.Values)
            {
                var xNode = xGraph.AddNode<DialogueXNode>();
                xNode.nodeId = editorNode.id;
                xNode.inputType = editorNode.input;
                xNode.isEnd = editorNode.isEnd;

                // Позиции пока рандом / по сетке
                xNode.position = new Vector2(
                    Random.Range(0, 600),
                    Random.Range(0, 400)
                );
            }

            // 4️⃣ Соединение связей
            foreach (var editorNode in graph.nodes.Values)
            {
                var fromXNode = FindXNode(xGraph, editorNode.id);
                if (fromXNode == null)
                    continue;

                if (editorNode.edges == null)
                    continue;

                foreach (var edge in editorNode.edges)
                {
                    var toXNode = FindXNode(xGraph, edge.toNodeId);
                    if (toXNode == null)
                        continue;

                    fromXNode.GetOutputPort("output")
                             .Connect(toXNode.GetInputPort("input"));
                }
            }

            ApplyLayout(xGraph, graph);
            return xGraph;
        }

        private static void ApplyLayout(
                DialogueXNodeGraph xGraph,
                EditorDialogueGraph source)
        {
            const float xSpacing = 350f;
            const float ySpacing = 160f;

            var visited = new HashSet<string>();
            var queue = new Queue<(string nodeId, int depth)>();
            var layerCounters = new Dictionary<int, int>();

            queue.Enqueue((source.startNodeId, 0));
            visited.Add(source.startNodeId);

            while (queue.Count > 0)
            {
                var (nodeId, depth) = queue.Dequeue();
                var node = FindXNode(xGraph, nodeId);
                if (node == null)
                    continue;

                if (!layerCounters.ContainsKey(depth))
                    layerCounters[depth] = 0;

                int index = layerCounters[depth]++;
                node.position = new Vector2(
                    depth * xSpacing,
                    index * ySpacing
                );

                var editorNode = source.nodes[nodeId];
                if (editorNode.edges == null)
                    continue;

                foreach (var edge in editorNode.edges)
                {
                    if (visited.Contains(edge.toNodeId))
                        continue;

                    visited.Add(edge.toNodeId);
                    queue.Enqueue((edge.toNodeId, depth + 1));
                }
            }
        }


        private static DialogueXNode FindXNode(
            DialogueXNodeGraph graph,
            string nodeId)
        {
            foreach (var node in graph.nodes)
            {
                if (node is DialogueXNode dx && dx.nodeId == nodeId)
                    return dx;
            }

            return null;
        }
    }
}

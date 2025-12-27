using UnityEngine;
using DialogueSystem.Editor.GraphModel;
using DialogueSystem.Editor.Validation;
using DialogueSystem.Editor.XNode;
using DialogueSystem.Editor.XNode.Utils;
using UnityEditor;
using XNode;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using DialogueSystem.Runtime;
using System.Linq;
using static DialogueSystem.Editor.XNode.DialogueXNode;
using System.Xml.Linq;

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
            xGraph.name = Path.GetFileNameWithoutExtension(graph.dialogueId);


            // 3️⃣ Создание XNode-нод
            foreach (var editorNode in graph.nodes.Values)
            {
                var xNode = xGraph.AddNode<DialogueXNode>();
                xNode.nodeId = editorNode.id;
                xNode.speaker = editorNode.speaker;
                xNode.text = editorNode.text;
                xNode.inputType = editorNode.input;
                xNode.isEnd = editorNode.isEnd;
                //xNode.end = editorNode.end;
                xNode.isStartNode = editorNode.id == graph.startNodeId;

                // Позиции пока рандом / по сетке
                xNode.position = new Vector2(
                    Random.Range(0, 600),
                    Random.Range(0, 400)
                );
            }

            foreach (var editorNode in graph.nodes.Values)
            {
                var xNode = FindXNode(xGraph, editorNode.id);
                if (xNode == null)
                    continue;

                if (editorNode.input == DialogueInputType.WaitingForChoice)
                {
                    xNode.choices = editorNode.choices
                        .Select(c => new ChoiceData
                        {
                            label = c.label,
                            targetNodeId = c.targetNodeId
                        })
                        .ToList();

                    xNode.SetupPorts();
                }
            }


            // 4️⃣ Соединение связей
            foreach (var editorNode in graph.nodes.Values)
            {
                var fromXNode = FindXNode(xGraph, editorNode.id);
                if (fromXNode == null || editorNode.edges == null)
                    continue;

                // ================================
                // CASE 1: CHOICE-NODE
                // ================================
                if (editorNode.input == DialogueInputType.WaitingForChoice)
                {
                    for (int i = 0; i < editorNode.choices.Count; i++)
                    {
                        var choice = editorNode.choices[i];

                        // Находим целевой нод
                        if (!graph.nodes.ContainsKey(choice.targetNodeId))
                            continue;

                        var toXNode = FindXNode(xGraph, choice.targetNodeId);
                        if (toXNode == null)
                            continue;

                        // Получаем конкретный choice-порт
                        var choicePort = fromXNode.GetOutputPort($"choice_{i}");
                        var inputPort = toXNode.GetInputPort("input");

                        if (choicePort == null || inputPort == null)
                            continue;

                        choicePort.Connect(inputPort);
                    }

                    // 🔒 ВАЖНО: для choice-ноды больше ничего не делаем
                    continue;
                }

                // ================================
                // CASE 2: NORMAL (LINEAR) NODE
                // ================================
                foreach (var edge in editorNode.edges)
                {
                    // Защита от мусора
                    if (edge.type != EditorEdgeType.Next)
                        continue;

                    if (!graph.nodes.ContainsKey(edge.toNodeId))
                        continue;

                    var toXNode = FindXNode(xGraph, edge.toNodeId);
                    if (toXNode == null)
                        continue;

                    var outputPort = fromXNode.GetOutputPort("output");
                    var inputPort = toXNode.GetInputPort("input");

                    if (outputPort == null || inputPort == null)
                        continue;

                    outputPort.Connect(inputPort);
                }
            }

            ApplyLayout(xGraph, graph);
            return xGraph;
        }

        //private static float EstimateNodeHeight(DialogueXNode node)
        //{
        //    const float baseHeight = 120f;
        //    const float lineHeight = 18f;
        //    const int charsPerLine = 60;

        //    int textLength = string.IsNullOrEmpty(node.text) ? 0 : node.text.Length;
        //    int lines = Mathf.Max(1, textLength / charsPerLine);

        //    return baseHeight + lines * lineHeight;
        //}

        private static void ApplyLayout(
            DialogueXNodeGraph xGraph,
            EditorDialogueGraph source)
        {
            const float baseXSpacing = 80f;
            const float baseYSpacing = 120f;
            const float nodeWidth = 420f;

            var visited = new HashSet<string>();
            var queue = new Queue<(string nodeId, int depth)>();
            var layerHeights = new Dictionary<int, float>();
            var layerNodes = new Dictionary<int, List<DialogueXNode>>();

            queue.Enqueue((source.startNodeId, 0));
            visited.Add(source.startNodeId);

            while (queue.Count > 0)
            {
                var (nodeId, depth) = queue.Dequeue();
                var node = FindXNode(xGraph, nodeId);
                if (node == null)
                    continue;

                float nodeHeight = DialogueXNodeLayoutUtil.EstimateNodeHeight(node);

                if (!layerHeights.ContainsKey(depth))
                    layerHeights[depth] = 0f;

                float x = depth * (nodeWidth + baseXSpacing);
                float y = layerHeights[depth];

                node.position = new Vector2(x, y);

                layerHeights[depth] += nodeHeight + baseYSpacing;

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

                if (!layerNodes.ContainsKey(depth))
                    layerNodes[depth] = new List<DialogueXNode>();

                layerNodes[depth].Add(node);
            }

            foreach (var pair in layerNodes)
            {
                int depth = pair.Key;
                var nodes = pair.Value;

                float totalHeight = 0f;
                foreach (var n in nodes)
                    totalHeight += DialogueXNodeLayoutUtil.EstimateNodeHeight(n) + baseYSpacing;

                float offsetY = -totalHeight * 0.5f;

                foreach (var n in nodes)
                    n.position += new Vector2(0, offsetY);
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

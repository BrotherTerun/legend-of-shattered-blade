using UnityEditor;
using UnityEngine;
using DialogueSystem.Editor.GraphModel;
using DialogueSystem.Runtime;

namespace DialogueSystem.Editor.Parsing
{
    public static class DialogueJsonToEditorGraphParser
    {
        private static bool TryParseInputType(
            string rawInput,
            out DialogueInputType inputType)
        {
            inputType = DialogueInputType.None;

            if (string.IsNullOrEmpty(rawInput))
                return true;

            switch (rawInput.ToLowerInvariant())
            {
                case "click":
                    inputType = DialogueInputType.ClickToContinue;
                    return true;

                case "choices":
                    inputType = DialogueInputType.WaitingForChoice;
                    return true;

                case "auto":
                    inputType = DialogueInputType.Auto;
                    return true;

                default:
                    return false;
            }
        }

        public static EditorGraphParseResult Parse(DialogueData data)
        {
            var result = new EditorGraphParseResult();

            // 1 Проверка входных данных
            if (data == null)
            {
                result.errors.Add(new EditorGraphParseError
                {
                    type = EditorGraphParseErrorType.MissingNode,
                    message = "DialogueData is null"
                });

                return result;
            }

            // 2 Создание графа
            var graph = new EditorDialogueGraph
            {
                dialogueId = data.id,
                startNodeId = data.startNode
            };

            result.graph = graph;

            // 3 Создание всех нод (List<DialogueNode>)
            foreach (var nodeData in data.nodes)
            {
                if (nodeData == null)
                    continue;

                var nodeId = nodeData.id;

                // 3.1 Проверка дубликатов id
                if (graph.nodes.ContainsKey(nodeId))
                {
                    result.errors.Add(new EditorGraphParseError
                    {
                        type = EditorGraphParseErrorType.DuplicateNodeId,
                        nodeId = nodeId,
                        message = $"Duplicate node id: {nodeId}"
                    });

                    continue;
                }

                // 3.2 Создание EditorNode
                DialogueInputType parsedInput = DialogueInputType.None;

                if (!string.IsNullOrEmpty(nodeData.input))
                {
                    if (!TryParseInputType(nodeData.input, out parsedInput))
                    {
                        result.errors.Add(new EditorGraphParseError
                        {
                            type = EditorGraphParseErrorType.InvalidInputType,
                            nodeId = nodeId,
                            message = $"Invalid input type '{nodeData.input}' on node '{nodeId}'"
                        });
                    }
                }

                var editorNode = new EditorNode
                {
                    id = nodeId,
                    speaker = nodeData.speaker,
                    text = nodeData.text,
                    input = parsedInput,
                    isEnd = nodeData.end != null && nodeData.end.IsValid,
                    end = nodeData.end,
                };

                graph.nodes.Add(nodeId, editorNode);
            }

            // 4 Создание рёбер (next / choices / end)
            foreach (var nodeData in data.nodes)
            {
                if (nodeData == null)
                    continue;

                var nodeId = nodeData.id;

                // Если по какой-то причине нода не была создана — пропускаем
                if (!graph.nodes.TryGetValue(nodeId, out var editorNode))
                    continue;

                // ─────────────────────
                // NEXT
                // ─────────────────────
                if (!string.IsNullOrEmpty(nodeData.next))
                {
                    if (!graph.nodes.ContainsKey(nodeData.next))
                    {
                        result.errors.Add(new EditorGraphParseError
                        {
                            type = EditorGraphParseErrorType.MissingNextNode,
                            nodeId = nodeId,
                            targetId = nodeData.next,
                            message = $"Next node '{nodeData.next}' not found (from '{nodeId}')"
                        });
                    }
                    else
                    {
                        editorNode.edges.Add(new EditorEdge
                        {
                            fromNodeId = nodeId,
                            toNodeId = nodeData.next,
                            type = EditorEdgeType.Next
                        });
                    }
                }

                // ─────────────────────
                // CHOICES
                // ─────────────────────
                if (nodeData.choices != null)
                {
                    foreach (var choice in nodeData.choices)
                    {
                        if (choice == null)
                            continue;

                        if (string.IsNullOrEmpty(choice.next))
                        {
                            result.errors.Add(new EditorGraphParseError
                            {
                                type = EditorGraphParseErrorType.MissingChoiceTarget,
                                nodeId = nodeId,
                                message = $"Choice has empty next target (from '{nodeId}')"
                            });

                            continue;
                        }

                        if (!graph.nodes.ContainsKey(choice.next))
                        {
                            result.errors.Add(new EditorGraphParseError
                            {
                                type = EditorGraphParseErrorType.MissingChoiceTarget,
                                nodeId = nodeId,
                                targetId = choice.next,
                                message = $"Choice target '{choice.next}' not found (from '{nodeId}')"
                            });

                            continue;
                        }

                        editorNode.edges.Add(new EditorEdge
                        {
                            fromNodeId = nodeId,
                            toNodeId = choice.next,
                            type = EditorEdgeType.Choice,
                            label = choice.text
                        });
                    }
                }

                // ─────────────────────
                // END
                // ─────────────────────
                if (nodeData.end != null)
                {
                    editorNode.edges.Add(new EditorEdge
                    {
                        fromNodeId = nodeId,
                        toNodeId = null,
                        type = EditorEdgeType.End
                    });
                }
            }

            return result;
        }
    }
}

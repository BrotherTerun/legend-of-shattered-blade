using DialogueSystem.Editor.GraphModel;
using System.Linq;
using System.Collections.Generic;
using DialogueSystem.Runtime;

namespace DialogueSystem.Editor.Validation
{
    public static class EditorGraphValidator
    {
        public static EditorGraphValidationResult Validate(EditorDialogueGraph graph)
        {
            var result = new EditorGraphValidationResult();

            if (graph == null || graph.nodes.Count == 0)
            {
                result.AddError(
                    EditorGraphValidationErrorType.GraphIsEmpty,
                    "Dialogue graph contains no nodes"
                );
                return result;
            }

            ValidateStartNode(graph, result);
            ValidateNodeEdges(graph, result);
            ValidateUnreachableNodes(graph, result);
            ValidateTransitions(graph, result);

            return result;
        }

        private static void ValidateStartNode(
            EditorDialogueGraph graph,
            EditorGraphValidationResult result)
        {
            if (string.IsNullOrEmpty(graph.startNodeId) ||
                !graph.nodes.ContainsKey(graph.startNodeId))
            {
                result.AddError(
                    EditorGraphValidationErrorType.NoStartNode,
                    "Dialogue graph has no valid start node"
                );
            }
        }

        private static void ValidateNodeEdges(
            EditorDialogueGraph graph,
            EditorGraphValidationResult result)
        {
            foreach (var node in graph.nodes.Values)
            {
                bool hasOutgoing = node.edges != null && node.edges.Count > 0;

                if (!hasOutgoing && !node.isEnd)
                {
                    result.AddError(
                        EditorGraphValidationErrorType.NodeHasNoOutgoingEdges,
                        $"Node '{node.id}' has no outgoing edges and is not an end node",
                        node.id
                    );
                }

                if (node.isEnd)
                {
                    bool hasInvalidEdges = node.edges.Any(e => e.type != EditorEdgeType.End);

                    if (hasInvalidEdges)
                    {
                        result.AddError(
                            EditorGraphValidationErrorType.EndNodeHasOutgoingEdges,
                            $"End node '{node.id}' has non-end outgoing edges",
                            node.id
                        );
                    }
                }
            }
        }

        private static void ValidateUnreachableNodes(
            EditorDialogueGraph graph,
            EditorGraphValidationResult result)
        {
            if (string.IsNullOrEmpty(graph.startNodeId) ||
                !graph.nodes.ContainsKey(graph.startNodeId))
                return;

            var visited = new HashSet<string>();
            var stack = new Stack<string>();

            stack.Push(graph.startNodeId);
            visited.Add(graph.startNodeId);

            while (stack.Count > 0)
            {
                var currentId = stack.Pop();
                var node = graph.nodes[currentId];

                if (node.edges == null)
                    continue;

                foreach (var edge in node.edges)
                {
                    if (string.IsNullOrEmpty(edge.toNodeId))
                        continue;

                    if (!graph.nodes.ContainsKey(edge.toNodeId))
                        continue;

                    if (visited.Add(edge.toNodeId))
                    {
                        stack.Push(edge.toNodeId);
                    }
                }
            }

            foreach (var nodeId in graph.nodes.Keys)
            {
                if (!visited.Contains(nodeId))
                {
                    result.AddError(
                        EditorGraphValidationErrorType.UnreachableNode,
                        $"Node '{nodeId}' is unreachable from start node",
                        nodeId
                    );
                }
            }
        }

        private static void ValidateTransitions(
            EditorDialogueGraph graph,
            EditorGraphValidationResult result)
        {
            foreach (var node in graph.nodes.Values)
            {
                // 1️⃣ Проверка переходов
                if (node.edges != null)
                {
                    foreach (var edge in node.edges)
                    {
                        if (edge.type == EditorEdgeType.End)
                            continue;

                        if (!graph.nodes.ContainsKey(edge.toNodeId))
                        {
                            result.AddError(
                                EditorGraphValidationErrorType.TransitionTargetNotFound,
                                $"Node '{node.id}' has transition to non-existent node '{edge.toNodeId}'",
                                node.id
                            );
                        }
                    }
                }

                // 2️⃣ Проверка end.target и end.type
                if (node.isEnd)
                {
                    if (node.end == null)
                    {
                        result.AddError(
                            EditorGraphValidationErrorType.EndTargetMissing,
                            $"End node '{node.id}' is marked as end but has no end data",
                            node.id
                        );
                        continue;
                    }

                    var endType = node.end.type;
                    var endTarget = node.end.target;

                    switch (endType)
                    {
                        case "dialogue":
                        case "topdown":
                            if (string.IsNullOrEmpty(endTarget))
                            {
                                result.AddError(
                                    EditorGraphValidationErrorType.EndTargetMissing,
                                    $"End node '{node.id}' has end type '{endType}' but empty target",
                                    node.id
                                );
                            }
                            break;

                        case "none":
                        case "":
                        case null:
                            // допустимое завершение без target
                            break;

                        default:
                            result.AddError(
                                EditorGraphValidationErrorType.InvalidEndType,
                                $"End node '{node.id}' has unknown end type '{endType}'",
                                node.id
                            );
                            break;
                    }
                }


                // 3️⃣ Конфликт типов завершения
                if (node.isEnd && node.edges != null)
                {
                    bool hasNonEndEdges = node.edges.Any(e => e.type != EditorEdgeType.End);

                    if (hasNonEndEdges)
                    {
                        result.AddError(
                            EditorGraphValidationErrorType.ConflictingEndTransitions,
                            $"End node '{node.id}' has conflicting outgoing transitions",
                            node.id
                        );
                    }
                }
            }
        }

    }
}

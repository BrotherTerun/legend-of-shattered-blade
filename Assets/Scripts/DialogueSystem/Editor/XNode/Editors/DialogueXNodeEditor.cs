using DialogueSystem.Editor.XNode.Utils;
using DialogueSystem.Runtime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using XNode;
using XNodeEditor;


namespace DialogueSystem.Editor.XNode
{
    [CustomNodeEditor(typeof(DialogueXNode))]
    public class DialogueXNodeEditor : NodeEditor
    {
        private static GUIStyle _whiteLabel;
        private static GUIStyle _whiteTextArea;

        private void EnsureStyles()
        {
            if (_whiteLabel == null)
            {
                _whiteLabel = new GUIStyle(EditorStyles.label);
                _whiteLabel.normal.textColor = Color.white;
                _whiteLabel.wordWrap = true;
            }

            if (_whiteTextArea == null)
            {
                _whiteTextArea = new GUIStyle(EditorStyles.textArea);
                _whiteTextArea.normal.textColor = Color.white;
                _whiteTextArea.wordWrap = true;
            }
        }

        public override Color GetTint()
        {
            var node = target as DialogueXNode;

            if (node == null)
                return base.GetTint();

            if (node.isStartNode)
                return new Color(0.3f, 0.8f, 0.3f); // стартовый - зелЄный

            if (node.isEnd)
                return new Color(0.7f, 0.3f, 0.3f); // финальный Ч красный

            return new Color(0.3f, 0.6f, 0.9f); // обычный Ч синий
        }

        private void DrawChoiceNodeLayout(DialogueXNode node)
        {
            EnsureStyles();

            EditorGUILayout.BeginHorizontal();

            // Ћева€ колонка Ч основной контент
            EditorGUILayout.BeginVertical(GUILayout.Width(260));
            EditorGUILayout.LabelField(node.speaker, _whiteLabel);
            EditorGUILayout.TextArea(node.text, _whiteTextArea);
            EditorGUILayout.EndVertical();

            // ѕрава€ колонка Ч choices
            EditorGUILayout.BeginVertical(GUILayout.Width(140));

            float totalHeight = node.choices.Count * 24f;
            GUILayout.Space(Mathf.Max(0, (DialogueXNodeLayoutUtil.EstimateNodeHeight(node) - totalHeight) * 0.5f));

            for (int i = 0; i < node.choices.Count; i++)
            {
                var port = node.GetOutputPort($"choice_{i}");
                NodeEditorGUILayout.PortField(
                    new GUIContent(node.choices[i].label),
                    port,
                    GUILayout.Height(22)
                );
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }


        public override void OnBodyGUI()
        {
            serializedObject.Update();


            EnsureStyles();

            DialogueXNode node = target as DialogueXNode;

            // ID
            EditorGUILayout.LabelField($"ID: {node.nodeId}", EditorStyles.boldLabel);

            // —пикер
            EditorGUILayout.LabelField("Speaker", _whiteLabel);
            EditorGUILayout.TextArea(node.speaker, _whiteTextArea);

            //ѕортрет
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Portrait");

            Rect portraitRect = EditorGUILayout.GetControlRect();

            EditorGUI.LabelField(
                portraitRect,
                string.IsNullOrEmpty(node.portraitPath)
                    ? "<none>"
                    : node.portraitPath,
                EditorStyles.linkLabel
            );

            Event e = Event.current;
            if (e.type == EventType.MouseDown && portraitRect.Contains(e.mousePosition))
            {
                if (!string.IsNullOrEmpty(node.portraitPath))
                {
                    var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(node.portraitPath);
                    if (asset != null)
                    {
                        Selection.activeObject = asset;

                        if (e.clickCount == 2)
                        {
                            AssetDatabase.OpenAsset(asset);
                        }
                    }
                }
                e.Use();
            }

            // “екст
            EditorGUILayout.TextArea(node.text, _whiteTextArea);

            EditorGUILayout.Space();

            // INPUT Ч только если Ќ≈ стартовый нод
            if (!node.isStartNode)
            {
                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("inputType"),
                    new GUIContent("Ouput Type")
                );
            }
            else
            {
                
            }

            foreach (NodePort port in target.Ports)
            {
                NodeEditorGUILayout.PortField(port);
            }

            //if (node.isEnd)
            //{
            //    EditorGUILayout.Space(4);
            //    EditorGUILayout.LabelField("End Type", node.end.type.ToString(), _whiteLabel);

            //    if (!string.IsNullOrEmpty(node.end.target))
            //    {
            //        EditorGUILayout.LabelField("End Target", node.end.target, _whiteLabel);
            //    }
            //}

            if (node.inputType == DialogueInputType.WaitingForChoice)
            {
                DrawChoiceNodeLayout(node);
                return;
            }

            serializedObject.ApplyModifiedProperties();
        }



        public override int GetWidth()
        {
            return 420; // вместо стандартных ~200
        }

    }
}


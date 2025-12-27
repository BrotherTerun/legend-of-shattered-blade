using UnityEngine;
using XNode;
using DialogueSystem.Runtime;
using System.Collections.Generic;
using System.Linq;

namespace DialogueSystem.Editor.XNode
{
    public class DialogueXNode : Node
    {
        public string nodeId;

        public string speaker;
        public string portraitPath;
        [TextArea]
        public string text;

        public DialogueInputType inputType;

        [Input] public DialogueXNode input;

        [HideInInspector]
        public bool isEnd;

        //public class end
        //{
        //    public string type;
        //    public string target;
        //}

        [HideInInspector]
        public bool isStartNode; 

        [System.Serializable]
        public class ChoiceData
        {
            public string label;
            public string targetNodeId;
        }

        public List<ChoiceData> choices;

        protected override void Init()
        {
            base.Init();
            SetupPorts();
        }

        public void SetupPorts()
        {
            // 0. Удаляем ВСЕ динамические порты без исключений
            ClearDynamicPorts();

            ////1. Start-node
            //if (isStartNode)
            //{
            //    // Только output
            //    AddDynamicOutput(
            //        typeof(DialogueXNode),
            //        Node.ConnectionType.Override,
            //        Node.TypeConstraint.Inherited,
            //        "output"
            //    );
            //    return;
            //}

            //// ВСЕ остальные ноды имеют input
            //AddDynamicInput(
            //    typeof(DialogueXNode),
            //    ConnectionType.Override,
            //    TypeConstraint.Inherited,
            //    "input"
            //);

            //if (isEnd)
            //{
            //    // END: вход есть, выхода нет
            //    return;
            //}

            // 2. Choice-нода
            if (inputType == DialogueInputType.WaitingForChoice)
            {
                if (choices == null)
                    return;

                for (int i = 0; i < choices.Count; i++)
                {
                    AddDynamicOutput(
                        typeof(DialogueXNode),
                        Node.ConnectionType.Override,
                        Node.TypeConstraint.Inherited,
                        $"choice_{i}"
                    );
                }

                // 🔒 ВАЖНО: здесь НЕ создаём output
                return;
            }

            // 3. Линейный нод (click / auto)
            AddDynamicOutput(
                typeof(DialogueXNode),
                Node.ConnectionType.Override,
                Node.TypeConstraint.Inherited,
                "output"
            );
        }

        public int GetChoiceIndexFromPort(NodePort port)
        {
            if (port == null)
                return -1;

            if (!port.fieldName.StartsWith("choice_"))
                return -1;

            var indexStr = port.fieldName.Replace("choice_", "");
            if (int.TryParse(indexStr, out int index))
                return index;

            return -1;
        }

        public override object GetValue(NodePort port)
        {
            return null;
        }
    }
}
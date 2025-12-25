using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueNode
{
    public string id;
    public string speaker;
    public string text;

    // click / choices / auto
    public string input;

    // линейный переход
    public string next;

    // варианты выбора
    public List<DialogueChoiceData> choices;

    // портретные инструкции
    public List<PortraitCommand> portraits;

    // финал диалога
    public DialogueEnd end;
}
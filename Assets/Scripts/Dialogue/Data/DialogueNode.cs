using System;
using System.Collections.Generic;

[Serializable]
public class DialogueNode
{
    public string id;
    public string speaker;
    public string text;
    public string portrait; // путь к спрайту

    public List<DialogueChoiceData> choices;
}
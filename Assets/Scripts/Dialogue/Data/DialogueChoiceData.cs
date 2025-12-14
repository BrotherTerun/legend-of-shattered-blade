using System;

[Serializable]
public class DialogueChoiceData
{
    public string text;
    public string nextNodeId;

    public int light;
    public int darkness;

    public PathEffect[] pathEffects;
}
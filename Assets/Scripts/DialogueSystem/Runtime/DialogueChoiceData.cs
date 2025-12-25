using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueChoiceData
{
    public string text;
    public string next;

    // эффекты выбора
    public ChoiceEffects effects;
}

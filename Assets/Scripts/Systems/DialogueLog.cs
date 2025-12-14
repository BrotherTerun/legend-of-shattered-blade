using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "VN/Dialogue Log")]
public class DialogueLog : ScriptableObject
{
    public List<LogEntry> entries = new List<LogEntry>();

    public void Add(string speaker, string text)
    {
        entries.Add(new LogEntry(speaker, text));
    }

    public void Clear()
    {
        entries.Clear();
    }
}
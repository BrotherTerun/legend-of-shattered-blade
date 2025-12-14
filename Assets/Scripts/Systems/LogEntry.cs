[System.Serializable]
public class LogEntry
{
    public string speaker;
    public string text;

    public LogEntry(string speaker, string text)
    {
        this.speaker = speaker;
        this.text = text;
    }
}
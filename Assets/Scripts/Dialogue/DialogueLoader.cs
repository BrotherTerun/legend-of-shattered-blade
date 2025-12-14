using System.IO;
using UnityEngine;

public static class DialogueLoader
{
    public static DialogueData Load(string fileName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Dialogues", fileName);

        if (!File.Exists(path))
        {
            Debug.LogError($"Dialogue file not found: {path}");
            return null;
        }

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<DialogueData>(json);
    }
}
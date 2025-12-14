using UnityEngine;
using TMPro;

public class LogUIController : MonoBehaviour
{
    public DialogueLog dialogueLog;
    public GameObject historyPanel;
    public Transform contentRoot;
    public TMP_Text entryPrefab;

    public void ToggleHistory()
    {
        historyPanel.SetActive(!historyPanel.activeSelf);

        if (historyPanel.activeSelf)
            Refresh();
    }

    void Refresh()
    {
        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);

        foreach (var entry in dialogueLog.entries)
        {
            var text = Instantiate(entryPrefab, contentRoot);
            text.text = $"{entry.speaker}: {entry.text}";
        }
    }
}
using TMPro;
using UnityEngine;

public class DialogueLayoutController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform dialogueRoot;
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private RectTransform textRect;
    [SerializeField] private TMP_Text text;

    [Header("Layout")]
    [SerializeField] private Vector2 padding = new Vector2(48f, 32f);
    [SerializeField, Range(0.5f, 1f)] private float maxWidthPercent = 0.9f;
    [SerializeField, Range(0.1f, 0.5f)] private float maxHeightPercent = 0.33f;
    [SerializeField] private float bottomOffset = 40f;

    void Awake()
    {
    }

    public void SetText(string value)
    {
        Debug.Log("SetText called: " + value); 
        text.text = value;
        RebuildLayout();
    }

    public void RebuildLayout()
    {
        Debug.Log("RebuildLayout called"); 
        float maxWidth = Screen.width * maxWidthPercent;
        float maxHeight = Screen.height * maxHeightPercent;

        Vector2 preferred = text.GetPreferredValues(text.text, maxWidth, maxHeight);

        Vector2 textSize = new Vector2(
            Mathf.Min(preferred.x, maxWidth),
            Mathf.Min(preferred.y, maxHeight)
        );

        textRect.sizeDelta = textSize;
        panelRect.sizeDelta = textSize + padding;

        // позиция текста ВНУТРИ панели
        textRect.anchoredPosition = new Vector2(
            padding.x * 0.5f,
           -padding.y * 0.5f
        );


        dialogueRoot.anchoredPosition = new Vector2(0f, bottomOffset);
    }
}

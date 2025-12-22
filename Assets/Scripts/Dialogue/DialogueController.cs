using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueController : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text dialogueText;
    public Image portraitImage;
    public Transform choicesRoot;
    public GameObject choiceButtonPrefab;
    [SerializeField] private DialogueLayoutController layout;

    [Header("Game State")]
    public PlayerStats playerStats;

    [Header("Logs")]
    public DialogueLog dialogueLog;

    [Header("DialogueData")]
    private DialogueData data;
    private DialogueNode currentNode;

    public void StartDialogue(string jsonFile)
    {
        data = DialogueLoader.Load(jsonFile);
        GoToNode("start");
    }

    // временный стартер диалога
    [Header("Startup")]
    public string startDialogueFile;

    void Start()
    {
        if (!string.IsNullOrEmpty(startDialogueFile))
            StartDialogue(startDialogueFile);
    }
    
    void GoToNode(string nodeId)
    {
        currentNode = data.nodes.Find(n => n.id == nodeId);
        RenderNode();
    }

    void RenderNode()
    {
        if (currentNode.speaker != "")
            dialogueText.text = $"<style=\"Speaker\">{currentNode.speaker}:</style> {currentNode.text}";
        else
            dialogueText.text = $"{currentNode.text}";
        layout.RebuildLayout();

        dialogueLog?.Add(currentNode.speaker, currentNode.text);

        Debug.Log($"Choices count: {(currentNode.choices == null ? "NULL" : currentNode.choices.Count.ToString())}"); 
        
        if (!string.IsNullOrEmpty(currentNode.portrait))
            portraitImage.sprite = Resources.Load<Sprite>(currentNode.portrait);

        foreach (Transform child in choicesRoot)
            Destroy(child.gameObject);

        if (currentNode.choices == null) return;

        foreach (var choice in currentNode.choices)
        {
            var btn = Instantiate(choiceButtonPrefab, choicesRoot);
            btn.GetComponentInChildren<TMP_Text>().text = choice.text;
            btn.GetComponent<Button>().onClick.AddListener(() => OnChoice(choice));
        }
    }

    void OnChoice(DialogueChoiceData choice)
    {
        ApplyEffects(choice);
        GoToNode(choice.nextNodeId);
        dialogueLog?.Add("Выбор", choice.text);
    }

    void ApplyEffects(DialogueChoiceData choice)
    {
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats is not assigned in DialogueController!");
            return;
        }

        // Свет / Тьма
        playerStats.light += choice.light;
        playerStats.darkness += choice.darkness;

        // Пути
        if (choice.pathEffects != null)
        {
            foreach (var effect in choice.pathEffects)
            {
                ApplyPathEffect(effect);
            }
        }
    }
    void ApplyPathEffect(PathEffect effect)
    {
        switch (effect.path)
        {
            case "Fire":
                playerStats.fire += effect.value;
                break;

            case "Water":
                playerStats.water += effect.value;
                break;

            case "Earth":
                playerStats.earth += effect.value;
                break;

            case "Air":
                playerStats.air += effect.value;
                break;

            case "Blood":
                playerStats.blood += effect.value;
                break;

            case "Bone":
                playerStats.bone += effect.value;
                break;

            case "Flesh":
                playerStats.flesh += effect.value;
                break;

            case "Metal":
                playerStats.metal += effect.value;
                break;

            default:
                Debug.LogWarning($"Unknown path: {effect.path}");
                break;
        }
    }
}
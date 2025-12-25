using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DialogueSystem.Runtime;

public class DialogueController : MonoBehaviour
{

    [Header("UI")]
    public TMP_Text dialogueText;
    public Transform choicesRoot;
    public GameObject choiceButtonPrefab;
    [SerializeField] private DialogueLayoutController layout;
    [SerializeField] private PortraitController portraitController;


    [Header("Game State")]
    public PlayerStats playerStats;

    [Header("Logs")]
    public DialogueLog dialogueLog;

    [Header("Dialogue Data")]
    private DialogueData data;
    private DialogueNode currentNode;
    private DialogueInputType currentInputMode = DialogueInputType.None;

    // временный автозапуск
    [Header("Startup")]
    public string startDialogueFile;

    void Start()
    {
        if (!string.IsNullOrEmpty(startDialogueFile))
            StartDialogue(startDialogueFile);
    }

    public void StartDialogue(string jsonFile)
    {
        data = DialogueLoader.Load(jsonFile);

        if (data == null)
        {
            Debug.LogError("Dialogue data not loaded!");
            return;
        }

        GoToNode(data.startNode);
    }

    void GoToNode(string nodeId)
    {
        currentNode = data.nodes.Find(n => n.id == nodeId);

        if (currentNode == null)
        {
            Debug.LogError($"Node not found: {nodeId}");
            return;
        }

        RenderNode();
    }

    void RenderNode()
    {
        currentInputMode = DialogueInputType.None;

        // 1. Текст
        if (!string.IsNullOrEmpty(currentNode.speaker))
            dialogueText.text = $"<style=\"Speaker\">{currentNode.speaker}:</style> {currentNode.text}";
        else
            dialogueText.text = currentNode.text;

        layout.RebuildLayout();
        dialogueLog?.Add(currentNode.speaker, currentNode.text);

        // 2. Очистка выборов
        foreach (Transform child in choicesRoot)
            Destroy(child.gameObject);

        // 3. Портреты (пока заглушка)
        portraitController?.Apply(currentNode.portraits);

        // 4. Обработка input
        switch (currentNode.input)
        {
            case "click":
                currentInputMode = DialogueInputType.ClickToContinue;
                break;

            case "choices":
                currentInputMode = DialogueInputType.WaitingForChoice;
                RenderChoices();
                break;

            case "auto":
                currentInputMode = DialogueInputType.Auto;
                GoNext();
                break;
        }
    }

    void RenderChoices()
    {
        if (currentNode.choices == null) return;

        foreach (var choice in currentNode.choices)
        {
            var btn = Instantiate(choiceButtonPrefab, choicesRoot);
            btn.GetComponentInChildren<TMP_Text>().text = choice.text;

            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                ApplyEffects(choice.effects);
                dialogueLog?.Add("Выбор", choice.text);
                GoToNode(choice.next);
            });
        }
    }

    public void OnBackgroundClick()
    {
        if (currentInputMode != DialogueInputType.ClickToContinue)
            return;

        GoNext();
    }

    void GoNext()
    {
        if (!string.IsNullOrEmpty(currentNode.next))
        {
            GoToNode(currentNode.next);
            return;
        }

        if (currentNode.end != null)
        {
            HandleDialogueEnd(currentNode.end);
        }
    }

    void HandleDialogueEnd(DialogueEnd end)
    {
        switch (end.type)
        {
            case "dialogue":
                StartDialogue(end.target);
                break;

            case "topdown":
                Debug.Log($"Switch to topdown scene: {end.target}");
                // SceneManager.LoadScene(end.target);
                break;

            case "none":
            default:
                Debug.Log("Dialogue finished.");
                break;
        }
    }

    void ApplyEffects(ChoiceEffects effects)
    {
        if (effects == null || playerStats == null) return;

        playerStats.light += effects.light;
        playerStats.darkness += effects.darkness;

        if (effects.paths == null) return;

        foreach (var effect in effects.paths)
            ApplyPathEffect(effect);

        if (effects.tenets == null) return;

        foreach (var effect in effects.tenets)
            ApplyTenetEffect(effect);
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

    void ApplyTenetEffect(TenetEffect effect)
    {
        switch (effect.tenet)
        {
            case "Honor":
                playerStats.honor += effect.value;
                break;

            case "Duty":
                playerStats.duty += effect.value;
                break;

            case "Sincerity":
                playerStats.sincerity += effect.value;
                break;

            case "Resolve":
                playerStats.resolve += effect.value;
                break;

            case "Will":
                playerStats.will += effect.value;
                break;

            case "Insight":
                playerStats.insight += effect.value;
                break;

            default:
                Debug.LogWarning($"Unknown tenet: {effect.tenet}");
                break;

        }
    }
}
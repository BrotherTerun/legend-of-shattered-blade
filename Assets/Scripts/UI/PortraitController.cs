using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortraitController : MonoBehaviour
{
    [System.Serializable]
    public class Slot
    {
        public string id;
        public Transform root;
    }

    public List<Slot> slots;

    private Dictionary<string, Image> activePortraits = new();
    private Dictionary<string, Transform> slotMap = new();

    void Awake()
    {
        foreach (var slot in slots)
            slotMap[slot.id] = slot.root;
    }

    public void Apply(List<PortraitCommand> commands)
    {
        if (commands == null) return;

        foreach (var cmd in commands)
            ApplyCommand(cmd);
    }

    void ApplyCommand(PortraitCommand cmd)
    {
        switch (cmd.action)
        {
            case "show":
                Show(cmd);
                break;

            case "hide":
                Hide(cmd.character);
                break;

            case "change":
                Change(cmd);
                break;

            case "move":
                Move(cmd);
                break;

            default:
                Debug.LogWarning($"Unknown portrait action: {cmd.action}");
                break;
        }
    }

    void Show(PortraitCommand cmd)
    {
        if (!slotMap.ContainsKey(cmd.slot))
        {
            Debug.LogError($"Slot not found: {cmd.slot}");
            return;
        }

        Image img;

        if (!activePortraits.TryGetValue(cmd.character, out img))
        {
            img = new GameObject(cmd.character).AddComponent<Image>();
            activePortraits[cmd.character] = img;
        }

        img.transform.SetParent(slotMap[cmd.slot], false);
        img.sprite = Resources.Load<Sprite>(cmd.sprite);
        img.gameObject.SetActive(true);
    }

    void Hide(string character)
    {
        if (activePortraits.TryGetValue(character, out var img))
            img.gameObject.SetActive(false);
    }

    void Change(PortraitCommand cmd)
    {
        if (activePortraits.TryGetValue(cmd.character, out var img))
            img.sprite = Resources.Load<Sprite>(cmd.sprite);
    }

    void Move(PortraitCommand cmd)
    {
        if (activePortraits.TryGetValue(cmd.character, out var img) &&
            slotMap.ContainsKey(cmd.slot))
        {
            img.transform.SetParent(slotMap[cmd.slot], false);
        }
    }
}

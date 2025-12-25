using UnityEngine;
using UnityEngine.EventSystems;

public class ClickCatcher : MonoBehaviour, IPointerClickHandler
{
    public DialogueController dialogue;

    public void OnPointerClick(PointerEventData eventData)
    {
        dialogue.OnBackgroundClick();
    }
}
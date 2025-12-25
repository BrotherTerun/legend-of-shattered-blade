using UnityEngine;

[System.Serializable]
public class PortraitCommand
{
    public string character;   // ID персонажа
    public string action;      // show / hide / change
    public string slot;    // left / right / center
    public string sprite;      // путь к спрайту
}


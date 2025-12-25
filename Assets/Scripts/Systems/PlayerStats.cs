using UnityEngine;

[CreateAssetMenu(menuName = "VN/Player Stats")]
public class PlayerStats : ScriptableObject
{
    public int light;
    public int darkness;

    public int fire, water, earth, air;
    public int blood, bone, flesh, metal;

    public int honor, duty, will, insight, resolve, sincerity;
}

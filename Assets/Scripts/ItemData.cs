using UnityEngine;

[CreateAssetMenu(menuName = "Cult of the Chef/Item")]
public class ItemData : ScriptableObject
{
    public string displayName;
    public Sprite sprite;            // leave empty to keep the placeholder shape
    public Color tint = Color.white;

    [Header("Plating")]
    [Tooltip("Can this go on a plate? Off for raw ingredients that still need work.")]
    public bool canPlate;
    [Tooltip("Draw order on the plate: lower sits underneath (noodles 0, sauce 1, cheese 2, meatballs 3).")]
    public int plateLayer;
}
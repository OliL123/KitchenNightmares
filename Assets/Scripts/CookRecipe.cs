using UnityEngine;

// One thing a cooking station can do: input → output, then output → ruined if it's left too long.
[CreateAssetMenu(menuName = "Cult of the Chef/Cook Recipe")]
public class CookRecipe : ScriptableObject
{
    public ItemData input;
    public ItemData output;
    public ItemData ruined;          // e.g. Overcooked
    public float cookTime = 6f;
    public float burnTime = 5f;      // grace period after it's done
}
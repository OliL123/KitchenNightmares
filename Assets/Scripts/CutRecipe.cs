using UnityEngine;

// One thing the butcher block can cut: input → result, plus an optional second part.
[CreateAssetMenu(menuName = "Cult of the Chef/Cut Recipe")]
public class CutRecipe : ScriptableObject
{
    public ItemData input;
    public ItemData result;                        // e.g. Brain Noodles, or Fingers
    public ItemData leftover;                      // optional second part, e.g. Fingerless Arm
    public int cutCount = 4;
    public Vector2 cutDirection = Vector2.down;    // which way each cut goes (used by the minigame)

    void OnValidate() => cutCount = Mathf.Max(1, cutCount);
}
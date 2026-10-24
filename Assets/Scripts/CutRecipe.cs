using System.Collections.Generic;
using UnityEngine;

// One thing the butcher block can cut: input → result, plus an optional second part.
[CreateAssetMenu(menuName = "Cult of the Chef/Cut Recipe")]
public class CutRecipe : ScriptableObject
{
    public ItemData input;
    public ItemData result;     // e.g. Brain Noodles, or Fingers
    public ItemData leftover;   // optional second part, e.g. Fingerless Arm

    // One entry per cut, in order: the way you flick or drag for that line. (0, -1) is down,
    // (1, -1) is down-right; diagonals get tidied to length 1 automatically.
    public List<Vector2> cuts = new() { Vector2.down };

    public int CutCount => cuts.Count;

    void OnValidate()
    {
        if (cuts.Count == 0) cuts.Add(Vector2.down);
        for (int i = 0; i < cuts.Count; i++)
            cuts[i] = cuts[i] == Vector2.zero ? Vector2.down : cuts[i].normalized;
    }
}
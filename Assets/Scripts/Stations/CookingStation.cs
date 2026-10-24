using System.Collections.Generic;
using UnityEngine;

// Passive station (cauldron, brazier): cooks one item on a timer, then ruins it if you leave it.
public class CookingStation : Station
{
    [SerializeField] Transform slot;
    [SerializeField] ProgressBar bar;
    [SerializeField] List<CookRecipe> recipes = new();

    static readonly Color Cooking = new(0.4f, 0.9f, 0.3f);
    static readonly Color Warning = new(1f, 0.6f, 0.1f);

    Item held;
    CookRecipe recipe;   // the recipe for whatever's in the pot

    public override void Interact(PlayerHands hands)
    {
        if (held == null)
        {
            CookRecipe match = hands.Held != null ? recipes.Find(r => r.input == hands.Held.Data) : null;
            if (match == null) return;
            recipe = match;
            held = hands.PutDown(slot);
        }
        else if (hands.Held == null) { hands.PickUp(held); held = null; }
        // Holding a plate: scoop the food onto it, if the plate will take it.
        else if (hands.TryPlateWith(held) && hands.Held != null) held = null;
    }

    void Update()
    {
        if (held == null) { bar.Hide(); return; }

        if (held.Data == recipe.input)
        {
            if (Advance(recipe.cookTime)) held.SetData(recipe.output);
            bar.Show(held.CookProgress, Cooking);
        }
        else if (held.Data == recipe.output)
        {
            if (Advance(recipe.burnTime)) held.SetData(recipe.ruined);
            bool flash = Mathf.Repeat(Time.time * 4f, 1f) < 0.5f;
            bar.Show(1f - held.CookProgress, flash ? Color.red : Warning);   // drains as it gets closer to ruined
        }
        else bar.Hide();   // ruined: nothing more happens
    }

    bool Advance(float duration)
    {
        held.CookProgress += Time.deltaTime / duration;
        return held.CookProgress >= 1f;
    }
}
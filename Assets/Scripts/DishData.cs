using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cult of the Chef/Dish")]
public class DishData : ScriptableObject
{
    public string menuName;                     // "Spaghetti al Sanguine"
    public List<ItemData> ingredients = new();

    // Exact match: same ingredients and the same number of each, in any order.
    public bool Matches(IReadOnlyList<ItemData> plate)
    {
        if (plate.Count != ingredients.Count) return false;
        var needed = new Dictionary<ItemData, int>();
        foreach (ItemData item in ingredients) needed[item] = needed.GetValueOrDefault(item) + 1;
        foreach (ItemData item in plate)
        {
            if (!needed.TryGetValue(item, out int count) || count == 0) return false;
            needed[item] = count - 1;
        }
        return true;
    }

    // Catches typos in the editor, like a dish that needs something that can't be plated.
    void OnValidate()
    {
        foreach (ItemData item in ingredients)
            if (item != null && !item.canPlate)
                Debug.LogWarning($"{name} needs {item.displayName}, but that can't go on a plate.", this);
    }
}
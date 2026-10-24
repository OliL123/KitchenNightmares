using System.Collections.Generic;
using UnityEngine;

// Goes on the plate item. Holds up to four ingredients; nothing comes back off.
// Shows a row of ingredient icons above the plate, Overcooked style.
[RequireComponent(typeof(Item))]
public class Plate : MonoBehaviour
{
    const int Capacity = 4;
    const int IconOrder = 100;   // icons draw over everything else on the plate

    [SerializeField] Transform contentsRoot;          // where ingredients sit on the plate
    [SerializeField] Transform iconRoot;              // centre of the icon row, above the plate
    [SerializeField] float ingredientScale = 0.8f;    // relative to the plate
    [SerializeField] float iconScale = 0.5f;
    [SerializeField] float iconSpacing = 0.6f;

    readonly List<Item> contents = new();
    readonly List<SpriteRenderer> icons = new();

    public List<ItemData> Ingredients => contents.ConvertAll(item => item.Data);

    public bool CanAdd(Item item) => contents.Count < Capacity && item.Data != null && item.Data.canPlate;

    public bool TryAdd(Item item)
    {
        if (!CanAdd(item)) return false;
        contents.Add(item);

        item.SetLoose(false);
        item.transform.SetParent(contentsRoot);
        item.transform.localPosition = Vector3.zero;
        item.transform.localScale = Vector3.one * ingredientScale;
        var sprite = item.GetComponent<SpriteRenderer>();
        sprite.sortingOrder = 1 + item.Data.plateLayer;   // higher plate layers draw on top

        AddIcon(sprite);
        return true;
    }

    // The trash empties the plate but you keep it.
    public void Clear()
    {
        foreach (Item item in contents) Destroy(item.gameObject);
        foreach (SpriteRenderer icon in icons) Destroy(icon.gameObject);
        contents.Clear();
        icons.Clear();
    }

    void AddIcon(SpriteRenderer ingredient)
    {
        var icon = new GameObject("Icon").AddComponent<SpriteRenderer>();
        icon.transform.SetParent(iconRoot, false);
        icon.transform.localScale = Vector3.one * iconScale;
        icon.sprite = ingredient.sprite;
        icon.color = ingredient.color;
        icon.sortingOrder = IconOrder;
        icons.Add(icon);

        // Keep the row centred above the plate.
        for (int i = 0; i < icons.Count; i++)
            icons[i].transform.localPosition = Vector3.right * ((i - (icons.Count - 1) / 2f) * iconSpacing);
    }
}
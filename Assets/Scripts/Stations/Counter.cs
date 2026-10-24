using UnityEngine;

// Holds one item: put something down or pick it back up.
public class Counter : Station
{
    [SerializeField] Transform slot;   // where the item sits
    Item held;

    // Lets you place a starting item on the counter in the editor.
    void Awake()
    {
        held = slot.GetComponentInChildren<Item>();
        if (held != null) held.SetLoose(false);
    }

    public override void Interact(PlayerHands hands)
    {
        if (held == null && hands.Held != null) held = hands.PutDown(slot);
        else if (held != null && hands.Held == null) { hands.PickUp(held); held = null; }
        // Both hold something: try plating. If the hands still hold something afterwards,
        // they had the plate and took our ingredient, so the counter is now empty.
        else if (held != null && hands.TryPlateWith(held) && hands.Held != null) held = null;
    }
}
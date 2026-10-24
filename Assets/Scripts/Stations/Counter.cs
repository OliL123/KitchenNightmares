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
    }
}
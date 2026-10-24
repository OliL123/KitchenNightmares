using UnityEngine;

// Plates get emptied; anything else is destroyed.
public class Trash : Station
{
    public override void Interact(PlayerHands hands)
    {
        if (hands.Held == null) return;
        if (hands.Held.TryGetComponent(out Plate plate)) plate.Clear();
        else Destroy(hands.Release().gameObject);
    }
}
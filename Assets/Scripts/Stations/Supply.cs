using UnityEngine;

public class Supply : Station
{
    [SerializeField] Item item;   // what item it is

    public override void Interact(PlayerHands hands)
    {
        if (hands.Held == null) hands.PickUp(Instantiate(item));
        else if (hands.Held.TryGetComponent(out Plate plate) && plate.CanAdd(item))
            plate.TryAdd(Instantiate(item));
    }
}
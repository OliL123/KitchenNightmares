using UnityEngine;

public class Supply : Station
{
    [SerializeField] Item item;   // what item it is

    public override void Interact(PlayerHands hands)
    {
        if (hands.Held == null) { hands.PickUp(Instantiate(item)); }
    }
}
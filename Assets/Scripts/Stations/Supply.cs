using UnityEngine;

// Endless supply of one ingredient, like the sacrifice pen or the cheese box.
public class Supply : Station
{
    [SerializeField] Item itemPrefab;   // the generic Item prefab
    [SerializeField] ItemData gives;    // what this spot hands out

    public override void Interact(PlayerHands hands)
    {
        if (hands.Held == null) hands.PickUp(Spawn());
        else if (hands.Held.TryGetComponent(out Plate plate) && plate.CanAdd(gives)) plate.TryAdd(Spawn());
    }

    Item Spawn()
    {
        Item item = Instantiate(itemPrefab);
        item.SetData(gives);
        return item;
    }
}
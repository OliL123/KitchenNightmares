using UnityEngine;

// Anything that can be carried: in the chef's hands, on a station, or loose on the floor.
// What it is comes from its ItemData; this just puts it in the world.
[RequireComponent(typeof(SpriteRenderer), typeof(CircleCollider2D))]
public class Item : MonoBehaviour
{
    [SerializeField] ItemData data;

    public ItemData Data => data;

    void Awake() => Apply();

    // Updates the look in the editor as soon as you change the data.
    void OnValidate() => Apply();

    // Turn this into a different item, e.g. when a station cooks it.
    public void SetData(ItemData newData)
    {
        data = newData;
        Apply();
    }

    // Loose = lying on the floor: findable by the chef, and sorted with the rest of the room.
    // Otherwise it's held or on a station, drawn in front of whatever is holding it.
    public void SetLoose(bool loose)
    {
        GetComponent<Collider2D>().enabled = loose;
        GetComponent<SpriteRenderer>().sortingOrder = loose ? 0 : 1;
    }

    void Apply()
    {
        if (data == null) return;
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (data.sprite != null) spriteRenderer.sprite = data.sprite;
        spriteRenderer.color = data.tint;
    }
}
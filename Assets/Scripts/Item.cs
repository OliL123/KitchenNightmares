using UnityEngine;
using UnityEngine.Rendering;

// Anything that can be carried: in the chef's hands, on a station, or loose on the floor.
// What it is comes from its ItemData; this just puts it in the world.
[RequireComponent(typeof(SpriteRenderer), typeof(CircleCollider2D))]
public class Item : MonoBehaviour
{
    [SerializeField] ItemData data;

    public ItemData Data => data;
    public float CookProgress { get; set; }

    // 0 (sloppy) to 1 (perfect), from minigame grades. Survives SetData, so it follows the food.
    public float Quality { get; set; } = 1f;

    void Awake() => Apply();

    // Updates the look in the editor as soon as you change the data.
    void OnValidate() => Apply();
    public void SetData(ItemData newData)
    {
        data = newData;
        CookProgress = 0f;   // a new form starts from scratch
        Apply();
    }

    // Loose = lying on the floor: findable by the chef, and sorted with the rest of the room.
    // Otherwise it's held or on a station, drawn in front of whatever is holding it.
    public void SetLoose(bool loose)
    {
        GetComponent<Collider2D>().enabled = loose;
        int order = loose ? 0 : 1;
        if (TryGetComponent(out SortingGroup group)) group.sortingOrder = order;
        else GetComponent<SpriteRenderer>().sortingOrder = order;
    }

    void Apply()
    {
        if (data == null) return;
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (data.sprite != null) spriteRenderer.sprite = data.sprite;
        spriteRenderer.color = data.tint;
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHands : MonoBehaviour
{
    [SerializeField] Transform holdPoint;
    [SerializeField] float reach = 0.8f;
    [SerializeField, Range(-1f, 1f)] float facingCone = 0.3f;   // 1 = dead ahead only, 0 = anything in front
    [SerializeField] float dropDistance = 0.5f;
    [SerializeField] LayerMask interactLayers;                    // Stations + Items

    PlayerMovement movement;

    public Item Held { get; private set; }

    void Awake() => movement = GetComponent<PlayerMovement>();

    void OnInteract(InputValue value)
    {
        if (!value.isPressed) return;   // only on press, not on release
        Collider2D target = FindTarget();

        if (target != null && target.TryGetComponent(out Station station)) station.Interact(this);
        else if (target != null && target.TryGetComponent(out Item item)) InteractWithFloorItem(item);
        else if (Held != null) Drop();
    }
    void InteractWithFloorItem(Item item)
    {
        if (Held == null) PickUp(item);
        else if (!TryPlateWith(item)) Drop();
    }

    // Plating between what we're holding and another item, whichever one is the plate.
    public bool TryPlateWith(Item other)
    {
        if (Held.TryGetComponent(out Plate plate)) return plate.TryAdd(other);
        if (other.TryGetComponent(out plate) && plate.TryAdd(Held)) { Release(); return true; }
        return false;
    }

    public void PickUp(Item item)
    {
        Held = item;
        item.SetLoose(false);
        Attach(item, holdPoint);
    }

    public Item PutDown(Transform to)
    {
        Item item = Held;
        Held = null;
        Attach(item, to);
        return item;
    }

    // Let go without putting it anywhere; whoever called this now owns the item.
    public Item Release()
    {
        Item item = Held;
        Held = null;
        return item;
    }

    void Drop()
    {
        Item item = Held;
        Held = null;
        item.transform.SetParent(null);
        item.transform.position = (Vector2)transform.position + movement.Facing * dropDistance;
        item.SetLoose(true);
    }

    static void Attach(Item item, Transform parent)
    {
        item.transform.SetParent(parent);
        item.transform.localPosition = Vector3.zero;
    }

    // The closest station or loose item that's within reach and roughly in front of us.
    Collider2D FindTarget()
    {
        Vector2 origin = transform.position;
        Collider2D best = null;
        float bestDistance = float.MaxValue;
        foreach (Collider2D hit in Physics2D.OverlapCircleAll(origin, reach, interactLayers))
        {
            Vector2 toHit = hit.ClosestPoint(origin) - origin;
            float distance = toHit.magnitude;
            // Something right under our feet counts no matter which way we face.
            if (distance > 0.01f && Vector2.Dot(movement.Facing, toHit / distance) < facingCone) continue;
            if (distance < bestDistance) { best = hit; bestDistance = distance; }
        }
        return best;
    }
}
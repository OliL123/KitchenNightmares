using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHands : MonoBehaviour
{
    const int HighlightOrder = 50;   // draws over stations and items

    [SerializeField] Transform holdPoint;
    [SerializeField] float reach = 0.8f;
    [SerializeField, Range(-1f, 1f)] float facingCone = 0.3f;   // 1 = dead ahead only, 0 = anything in front
    [SerializeField] float dropDistance = 0.5f;
    [SerializeField] LayerMask interactLayers;                    // Stations + Items
    [SerializeField] Color highlightColor = new(1f, 1f, 1f, 0.35f);

    PlayerMovement movement;
    Rigidbody2D body;
    ContactFilter2D interactFilter;
    ContactFilter2D solidFilter;
    readonly List<Collider2D> nearby = new();
    readonly List<RaycastHit2D> blockers = new();
    SpriteRenderer highlight;

    public Item Held { get; private set; }

    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        body = GetComponent<Rigidbody2D>();

        interactFilter = new ContactFilter2D { useTriggers = true };   // floor items are triggers
        interactFilter.SetLayerMask(interactLayers);
        solidFilter = new ContactFilter2D { useTriggers = false };     // walls and stations

        highlight = CreateHighlight();
    }

    void OnDestroy()
    {
        if (highlight != null) Destroy(highlight.gameObject);
    }

    // Show which station or item Interact would use right now.
    void Update()
    {
        Collider2D target = Busy != null ? null : FindTarget();
        highlight.enabled = target != null;
        if (target == null) return;
        Bounds bounds = target.bounds;
        highlight.transform.position = bounds.center;
        highlight.transform.localScale = bounds.size;   // the sprite is 1x1 units
    }

    void OnInteract(InputValue value)
    {
        if (!value.isPressed || Busy != null) return;   // only on press; ignored while locked in
        Collider2D target = FindTarget();

        if (target != null && target.TryGetComponent(out Station station)) station.Interact(this);
        else if (target != null && target.TryGetComponent(out Item item)) InteractWithFloorItem(item);
        else if (Held != null) Drop();
    }

    // Esc / B: bail out of whatever station you're locked into. Its progress is kept.
    void OnCancel(InputValue value)
    {
        if (value.isPressed && Busy != null) Busy.Leave();
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
        item.transform.position = DropSpot();
        item.SetLoose(true);
    }

    // In front of us, unless a wall or station is in the way; then at our feet.
    Vector2 DropSpot()
    {
        Vector2 origin = transform.position;
        Vector2 spot = origin + movement.Facing * dropDistance;
        int hits = Physics2D.Linecast(origin, spot, solidFilter, blockers);
        for (int i = 0; i < hits; i++)
            if (blockers[i].collider.attachedRigidbody != body) return origin;   // ignore our own collider
        return spot;
    }

    static void Attach(Item item, Transform parent)
    {
        item.transform.SetParent(parent);
        item.transform.localPosition = Vector3.zero;
    }

    // The closest station or loose item that's within reach and roughly in front of us.
    // Something under our feet only counts when nothing is in front, so standing on a dropped
    // item doesn't stop you using the counter you're facing.
    Collider2D FindTarget()
    {
        Vector2 origin = transform.position;
        Collider2D ahead = null;
        Collider2D underFeet = null;
        float bestDistance = float.MaxValue;
        int count = Physics2D.OverlapCircle(origin, reach, interactFilter, nearby);
        for (int i = 0; i < count; i++)
        {
            Collider2D hit = nearby[i];
            Vector2 toHit = hit.ClosestPoint(origin) - origin;
            float distance = toHit.magnitude;
            if (distance <= 0.01f)
            {
                if (underFeet == null) underFeet = hit;
                continue;
            }
            if (Vector2.Dot(movement.Facing, toHit / distance) < facingCone) continue;
            if (distance < bestDistance) { ahead = hit; bestDistance = distance; }
        }
        return ahead != null ? ahead : underFeet;
    }

    // A see-through box laid over the current target. Placeholder until there's outline art.
    SpriteRenderer CreateHighlight()
    {
        var renderer = new GameObject($"{name} Highlight").AddComponent<SpriteRenderer>();
        renderer.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0f, 0f, 4f, 4f), new Vector2(0.5f, 0.5f), 4f);
        renderer.color = highlightColor;
        renderer.sortingOrder = HighlightOrder;
        renderer.enabled = false;
        return renderer;
    }

    public ActiveStation Busy { get; set; }   // set while locked into a station (cutting, chanting...)
    public PlayerMovement Movement => movement;
}

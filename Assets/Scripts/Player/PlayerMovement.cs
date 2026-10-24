using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float acceleration = 60f;

    Rigidbody2D body;
    PlayerHands hands;
    Vector2 walk;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        hands = GetComponent<PlayerHands>();
    }

    public Vector2 Facing { get; private set; } = Vector2.down;

    // The raw Walk input. Stations read it while you're locked in (cutting, picking a part).
    public Vector2 WalkInput => walk;

    bool Locked => hands.Busy != null;

    // Player Input calls this whenever the Walk action changes.
    void OnWalk(InputValue value)
    {
        walk = value.Get<Vector2>();
        // Keep the last direction when you stop; don't turn while locked into a station.
        if (walk != Vector2.zero && !Locked) Facing = walk.normalized;
    }

    void FixedUpdate()
    {
        Vector2 target = Locked ? Vector2.zero : walk * speed;
        body.linearVelocity = Vector2.MoveTowards(body.linearVelocity, target, acceleration * Time.fixedDeltaTime);
    }
}
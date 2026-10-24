using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float acceleration = 60f;

    const float ReleaseGrace = 0.1f;   // seconds; see OnWalk

    Rigidbody2D body;
    PlayerHands hands;
    Vector2 walk;
    Vector2 lastDiagonal;
    float diagonalEndedAt = -1f;

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
        if (Locked) return;   // don't turn while locked into a station

        if (walk == Vector2.zero)
        {
            // Letting go of a diagonal: the two keys almost never lift on the same frame, so for a
            // moment only one is held. If that moment was short, keep facing the diagonal.
            if (Time.time - diagonalEndedAt < ReleaseGrace) Facing = lastDiagonal;
            return;   // otherwise keep the last direction
        }

        if (IsDiagonal(Facing) && !IsDiagonal(walk))
        {
            lastDiagonal = Facing;
            diagonalEndedAt = Time.time;
        }
        Facing = walk.normalized;
    }

    static bool IsDiagonal(Vector2 direction) => Mathf.Abs(direction.x) > 0.3f && Mathf.Abs(direction.y) > 0.3f;

    void FixedUpdate()
    {
        Vector2 target = Locked ? Vector2.zero : walk * speed;
        body.linearVelocity = Vector2.MoveTowards(body.linearVelocity, target, acceleration * Time.fixedDeltaTime);
    }
}
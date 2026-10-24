using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float acceleration = 60f;

    Rigidbody2D body;
    Vector2 walk;

    void Awake() => body = GetComponent<Rigidbody2D>();

    // Player Input calls this whenever the Move action changes (WASD, arrows, gamepad stick).
    public Vector2 Facing { get; private set; } = Vector2.down;

    void OnWalk(InputValue value)
    {
        walk = value.Get<Vector2>();
        if (walk != Vector2.zero) Facing = walk.normalized;   // keep the last direction when you stop
    }

    void FixedUpdate()
    {
        Vector2 target = walk * speed;
        body.linearVelocity = Vector2.MoveTowards(body.linearVelocity, target, acceleration * Time.fixedDeltaTime);
    }
}
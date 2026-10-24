using UnityEngine;

// Shows the chef pose that matches the way they're facing.
[RequireComponent(typeof(SpriteRenderer))]
public class ChefLook : MonoBehaviour
{
    [SerializeField] Sprite front, frontLeft, frontRight, back, backLeft, backRight;

    PlayerMovement movement;
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void LateUpdate() => spriteRenderer.sprite = PoseFor(movement.Facing);

    Sprite PoseFor(Vector2 facing)
    {
        bool left = facing.x < -0.5f, right = facing.x > 0.5f;
        if (facing.y > 0.5f) return left ? backLeft : right ? backRight : back;
        if (facing.y < -0.5f) return left ? frontLeft : right ? frontRight : front;
        return facing.x < 0f ? frontLeft : frontRight;   // no side poses yet: turn towards the camera
    }
}
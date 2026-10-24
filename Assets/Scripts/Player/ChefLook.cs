using UnityEngine;

// Shows the chef pose that matches the way they're facing, turning through the in-between
// poses instead of snapping. Only the look turns gradually; Facing (used for targeting) is instant.
[RequireComponent(typeof(SpriteRenderer))]
public class ChefLook : MonoBehaviour
{
    [SerializeField] Sprite front, frontLeft, frontRight, back, backLeft, backRight;
    [SerializeField] float turnSpeed = 720f;   // degrees per second; lower = slower turns

    PlayerMovement movement;
    SpriteRenderer spriteRenderer;
    float shownAngle = -90f;   // the direction the sprite currently shows, in degrees (-90 = down)

    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        Vector2 facing = movement.Facing;
        float target = Mathf.Atan2(facing.y, facing.x) * Mathf.Rad2Deg;
        float turn = Mathf.DeltaAngle(shownAngle, target);
        // A full about-face could go either way; turn through the front so the chef faces the camera.
        if (Mathf.Abs(turn) > 179f) turn = 180f * Mathf.Sign(Mathf.DeltaAngle(shownAngle, -90f));
        float step = turnSpeed * Time.deltaTime;
        shownAngle += Mathf.Clamp(turn, -step, step);
        float radians = shownAngle * Mathf.Deg2Rad;
        spriteRenderer.sprite = PoseFor(new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)));
    }

    Sprite PoseFor(Vector2 facing)
    {
        bool left = facing.x < -0.5f, right = facing.x > 0.5f;
        if (facing.y > 0.5f) return left ? backLeft : right ? backRight : back;
        if (facing.y < -0.5f) return left ? frontLeft : right ? frontRight : front;
        return facing.x < 0f ? frontLeft : frontRight;   // no side poses yet: turn towards the camera
    }
}
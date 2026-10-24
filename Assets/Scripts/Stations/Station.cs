using UnityEngine;

// Base for anything the chef can press Interact on.
public abstract class Station : MonoBehaviour
{
    public abstract void Interact(PlayerHands hands);
}
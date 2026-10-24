using UnityEngine;

// A world-space bar made from sprites: the fill grows from the left.
public class ProgressBar : MonoBehaviour
{
    [SerializeField] SpriteRenderer fill;
    [SerializeField] float width = 1f;

    public void Show(float t, Color color)
    {
        gameObject.SetActive(true);
        t = Mathf.Clamp01(t);
        fill.color = color;
        fill.transform.localScale = new Vector3(width * t, fill.transform.localScale.y, 1f);
        fill.transform.localPosition = new Vector3((t - 1f) * width / 2f, 0f, 0f);
    }

    public void Hide() => gameObject.SetActive(false);
}
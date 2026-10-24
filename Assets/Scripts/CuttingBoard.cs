using System.Collections.Generic;
using TMPro;
using UnityEngine;

// The pop-up board above the butcher block: the food, a dotted line for each cut, and the
// rating text. Visuals only; ButcherBlock decides what happens.
public class CuttingBoard : MonoBehaviour
{
    const int DotsPerLine = 7;
    const int DotOrder = 5;            // inside the board's own Sorting Group
    const float StartDotScale = 1.8f;  // the first dot is bigger, to show where the cut starts

    [SerializeField] SpriteRenderer food;
    [SerializeField] SpriteRenderer leftPart;    // shown when picking which part to keep
    [SerializeField] SpriteRenderer rightPart;
    [SerializeField] TMP_Text label;
    [SerializeField] float lineSpacing = 0.45f;
    [SerializeField] float lineLength = 0.8f;
    [SerializeField] float dotSize = 0.07f;
    [SerializeField] float labelTime = 0.7f;     // how long a rating stays up

    static readonly Color Upcoming = new(1f, 1f, 1f, 0.45f);
    static readonly Color Current = new(1f, 0.85f, 0.2f);
    static readonly Color Done = new(0.75f, 0.1f, 0.1f);

    readonly List<SpriteRenderer[]> lines = new();
    Sprite dotSprite;
    int current;
    float labelShownAt = float.NegativeInfinity;
    float closeAt = float.PositiveInfinity;

    void Awake() => dotSprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0f, 0f, 4f, 4f), new Vector2(0.5f, 0.5f), 4f);

    // Show the food with one dotted line per cut, laid out left to right.
    public void Open(Item item, IReadOnlyList<Vector2> cuts, int cutsDone)
    {
        Show();
        CopyLook(item, food);
        leftPart.enabled = rightPart.enabled = false;
        ClearLines();
        float firstX = -(cuts.Count - 1) * lineSpacing / 2f;
        for (int i = 0; i < cuts.Count; i++)
            lines.Add(MakeLine(new Vector2(firstX + i * lineSpacing, 0f), cuts[i]));
        SetProgress(cutsDone);
        label.text = "";
    }

    // Show the two finished parts side by side, matching their places on the block.
    public void ShowChoice(Item left, Item right)
    {
        Show();
        ClearLines();
        food.enabled = false;
        CopyLook(left, leftPart);
        CopyLook(right, rightPart);
        ShowPrompt($"< {left.Data.displayName}        {right.Data.displayName} >");
    }

    public void SetProgress(int cutsDone)
    {
        current = cutsDone;
        for (int i = 0; i < lines.Count; i++)
        {
            Color color = i < cutsDone ? Done : i == cutsDone ? Current : Upcoming;
            for (int d = 0; d < lines[i].Length; d++)
            {
                lines[i][d].color = color;
                lines[i][d].transform.localScale = Vector3.one * DotScale(d);
            }
        }
    }

    // A rating like "Perfect!" that fades out.
    public void ShowRating(string text, Color color)
    {
        label.text = text;
        label.color = color;
        labelShownAt = Time.time;
    }

    // Close a moment from now, so the last rating can still be read.
    public void CloseAfter(float delay) => closeAt = Mathf.Min(closeAt, Time.time + delay);

    public void Close()
    {
        closeAt = float.PositiveInfinity;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (Time.time >= closeAt) { Close(); return; }

        // Pulse the line you're on, so it's obvious which one to cut.
        if (current < lines.Count)
        {
            float pulse = 1f + 0.25f * Mathf.Sin(Time.time * 10f);
            for (int d = 0; d < lines[current].Length; d++)
                lines[current][d].transform.localScale = Vector3.one * (DotScale(d) * pulse);
        }

        // Ratings fade out; prompts (shown at +infinity) don't.
        Color color = label.color;
        color.a = 1f - Mathf.Clamp01((Time.time - labelShownAt) / labelTime);
        label.color = color;
    }

    void Show()
    {
        closeAt = float.PositiveInfinity;
        gameObject.SetActive(true);
    }

    void ShowPrompt(string text)
    {
        label.text = text;
        label.color = Color.white;
        labelShownAt = float.PositiveInfinity;
    }

    float DotScale(int index) => dotSize * (index == 0 ? StartDotScale : 1f);

    SpriteRenderer[] MakeLine(Vector2 center, Vector2 direction)
    {
        var dots = new SpriteRenderer[DotsPerLine];
        for (int i = 0; i < DotsPerLine; i++)
        {
            float along = i / (DotsPerLine - 1f) - 0.5f;   // -0.5 at the start of the cut, 0.5 at the end
            var dot = new GameObject("Cut Dot").AddComponent<SpriteRenderer>();
            dot.sprite = dotSprite;
            dot.sortingOrder = DotOrder;
            dot.transform.SetParent(transform, false);
            dot.transform.localPosition = center + direction * (along * lineLength);
            dots[i] = dot;
        }
        return dots;
    }

    void ClearLines()
    {
        foreach (SpriteRenderer[] line in lines)
            foreach (SpriteRenderer dot in line) Destroy(dot.gameObject);
        lines.Clear();
    }

    static void CopyLook(Item from, SpriteRenderer to)
    {
        var look = from.GetComponent<SpriteRenderer>();
        to.sprite = look.sprite;
        to.color = look.color;
        to.enabled = true;
    }
}
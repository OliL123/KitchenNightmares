using System.Collections.Generic;
using UnityEngine;

// Active station: lock in and cut the food, Cooking Mama style. Each dotted line is cut by
// flicking the stick (or WASD) its way. An arm gives two parts and you pick which one to
// take; the other stays on the block.
public class ButcherBlock : ActiveStation
{
    const float ChoiceThreshold = 0.5f;   // how far to push left/right to pick a part
    const float CenterThreshold = 0.2f;   // below this the stick counts as let go
    const float FlickThreshold = 0.8f;    // above this it counts as a flick
    const float FlickWindow = 0.05f;      // seconds to wait, so two keys pressed for a diagonal both count
    const float SlowTime = 1.5f;          // taking longer than this on a line costs one rating step
    const float BoardLinger = 0.4f;       // the board stays up this long after you leave, to read the last rating

    // Ratings, best first.
    static readonly string[] RatingNames = { "Perfect!", "Good", "Sloppy..." };
    static readonly float[] RatingScores = { 1f, 0.6f, 0.2f };
    static readonly Color[] RatingColors = { new(0.4f, 1f, 0.4f), new(1f, 0.85f, 0.2f), new(1f, 0.35f, 0.3f) };

    [SerializeField] Transform slot;            // the food being cut, and the left part once it's done
    [SerializeField] Transform leftoverSlot;    // the right part, if there is one
    [SerializeField] Item itemPrefab;           // the generic Item prefab, for spawning the second part
    [SerializeField] ProgressBar bar;
    [SerializeField] CuttingBoard boardPrefab;
    [SerializeField] Vector2 boardOffset = new(0f, 1.9f);   // where the board pops up, above the block
    [SerializeField] List<CutRecipe> recipes = new();

    static readonly Color Cutting = new(0.4f, 0.9f, 0.3f);

    CuttingBoard board;
    Item food;          // being cut, or a finished part
    Item leftover;      // the second finished part
    CutRecipe recipe;   // what's being cut; null when nothing is
    int cutsDone;
    float gradeTotal;
    bool choosing;      // the player is picking which part to take
    bool choiceArmed;   // the stick has come back to the middle since the choice started

    // The flick for the current line.
    bool flickArmed;               // the stick has been let go since the last cut
    float flickStartedAt = -1f;    // when the stick was pushed, or -1 if it hasn't been yet
    Vector2 flickDirection;
    float lineShownAt;

    void Awake()
    {
        // The board lives outside the block, so it draws above everything instead of sorting with the block.
        board = Instantiate(boardPrefab, (Vector2)transform.position + boardOffset, Quaternion.identity);
        board.Close();
    }

    void OnDestroy()
    {
        if (board != null) Destroy(board.gameObject);
    }

    public override void Interact(PlayerHands hands)
    {
        if (food == null)
        {
            CutRecipe match = hands.Held != null ? recipes.Find(r => r.input == hands.Held.Data) : null;
            if (match == null) return;
            recipe = match;
            cutsDone = 0;
            gradeTotal = 0f;
            food = hands.PutDown(slot);
            StartCutting(hands);
        }
        else if (hands.Held != null) return;               // clear the block first
        else if (recipe != null) StartCutting(hands);      // resume a half-cut
        else if (leftover != null) StartChoosing(hands);   // two parts left: pick one
        else { hands.PickUp(food); food = null; }          // one part left: take it
    }

    void Update()
    {
        // The bar only matters for half-cut food nobody's working on; the board shows progress while cutting.
        if (recipe != null && User == null) bar.Show((float)cutsDone / recipe.CutCount, Cutting);
        else bar.Hide();

        if (User == null) return;
        if (choosing) Choose();
        else ReadFlick();
    }

    void StartCutting(PlayerHands hands)
    {
        Begin(hands);
        board.Open(food, recipe.cuts, cutsDone);
        NextLine();
    }

    void NextLine()
    {
        flickArmed = false;   // let go of the stick before the next flick counts
        flickStartedAt = -1f;
        lineShownAt = Time.time;
    }

    // A flick is the stick (or WASD) going from let go to pushed. We read its direction a moment
    // later, so two keys pressed for a diagonal both count even if they land a frame apart.
    void ReadFlick()
    {
        Vector2 input = User.Movement.WalkInput;
        if (flickStartedAt < 0f)
        {
            if (input.magnitude < CenterThreshold) flickArmed = true;
            else if (flickArmed && input.magnitude > FlickThreshold)
            {
                flickStartedAt = Time.time;
                flickDirection = input;
            }
            return;
        }

        if (input.sqrMagnitude > 0.01f) flickDirection = input;   // a second key joining in
        if (Time.time - flickStartedAt < FlickWindow) return;

        Cut(Rate(flickDirection, recipe.cuts[cutsDone]));
    }

    // 0 = Perfect, 1 = Good, 2 = Sloppy.
    int Rate(Vector2 cut, Vector2 line)
    {
        float angle = Vector2.Angle(cut, line);
        int rating = angle <= 22.5f ? 0 : angle <= 67.5f ? 1 : 2;   // spot on, one step off, worse
        if (Time.time - lineShownAt > SlowTime) rating = Mathf.Min(rating + 1, 2);
        return rating;
    }

    void Cut(int rating)
    {
        gradeTotal += RatingScores[rating];
        cutsDone++;
        board.ShowRating(RatingNames[rating], RatingColors[rating]);
        board.SetProgress(cutsDone);
        if (cutsDone < recipe.CutCount) { NextLine(); return; }

        // Done: the food becomes the result and keeps the average grade as its quality.
        food.Quality = gradeTotal / recipe.CutCount;
        food.SetData(recipe.result);
        CutRecipe finished = recipe;
        recipe = null;

        if (finished.leftover == null)
        {
            PlayerHands hands = User;
            Leave();
            hands.PickUp(food);
            food = null;
            return;
        }

        leftover = Instantiate(itemPrefab);
        leftover.SetData(finished.leftover);
        leftover.Quality = food.Quality;
        Place(leftover, leftoverSlot);
        StartChoosing(User);
    }

    void StartChoosing(PlayerHands hands)
    {
        choosing = true;
        choiceArmed = false;
        if (User == null) Begin(hands);
        board.ShowChoice(food, leftover);
    }

    // Left takes the result (fingers), right takes the leftover (the arm); the other stays here.
    void Choose()
    {
        float x = User.Movement.WalkInput.x;
        // Wait for the stick to come back to the middle, so a held direction doesn't pick by accident.
        if (!choiceArmed) { choiceArmed = Mathf.Abs(x) < ChoiceThreshold; return; }
        if (Mathf.Abs(x) < ChoiceThreshold) return;

        Item taken = x < 0f ? food : leftover;
        Item kept = x < 0f ? leftover : food;
        PlayerHands hands = User;
        Leave();
        hands.PickUp(taken);
        food = kept;        // the part you didn't take stays where it is
        leftover = null;
    }

    protected override void OnLeave()
    {
        choosing = false;
        board.CloseAfter(BoardLinger);
    }

    static void Place(Item item, Transform where)
    {
        item.SetLoose(false);
        item.transform.SetParent(where);
        item.transform.localPosition = Vector3.zero;
    }
}
using System.Collections.Generic;
using UnityEngine;

// Active station: lock in and cut the food, Cooking Mama style.
// An arm gives two parts and you pick which one to take; the other stays on the block.
public class ButcherBlock : ActiveStation
{
    const float ChoiceThreshold = 0.5f;   // how far to push left/right to pick a part

    [SerializeField] Transform slot;            // the food being cut, and the left part once it's done
    [SerializeField] Transform leftoverSlot;    // the right part, if there is one
    [SerializeField] Item itemPrefab;           // the generic Item prefab, for spawning the second part
    [SerializeField] ProgressBar bar;
    [SerializeField] List<CutRecipe> recipes = new();
    [SerializeField] float placeholderCutTime = 0.5f;   // TEMP: auto-cutting until the real minigame

    static readonly Color Cutting = new(0.4f, 0.9f, 0.3f);

    Item food;          // being cut, or a finished part
    Item leftover;      // the second finished part
    CutRecipe recipe;   // what's being cut; null when nothing is
    int cutsDone;
    float gradeTotal;
    bool choosing;      // the player is picking which part to take
    bool choiceArmed;   // the stick has come back to the middle since the choice started
    float placeholderTimer;

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
            Begin(hands);
        }
        else if (hands.Held != null) return;               // clear the block first
        else if (recipe != null) Begin(hands);             // resume a half-cut
        else if (leftover != null) StartChoosing(hands);   // two parts left: pick one
        else { hands.PickUp(food); food = null; }          // one part left: take it
    }

    void Update()
    {
        if (recipe != null) bar.Show((float)cutsDone / recipe.cutCount, Cutting);
        else bar.Hide();

        if (User == null) return;
        if (choosing) { Choose(); return; }

        // TEMP: one Perfect cut every half second, until the real minigame replaces this.
        placeholderTimer += Time.deltaTime;
        if (placeholderTimer < placeholderCutTime) return;
        placeholderTimer = 0f;
        Cut(1f);
    }

    // One cut, graded from 0 (sloppy) to 1 (perfect).
    void Cut(float grade)
    {
        cutsDone++;
        gradeTotal += grade;
        if (cutsDone < recipe.cutCount) return;

        // Done: the food becomes the result and keeps the average grade as its quality.
        food.Quality = gradeTotal / recipe.cutCount;
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
        placeholderTimer = 0f;
    }

    static void Place(Item item, Transform where)
    {
        item.SetLoose(false);
        item.transform.SetParent(where);
        item.transform.localPosition = Vector3.zero;
    }
}
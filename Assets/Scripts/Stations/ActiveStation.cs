// A station that locks the player in while they use it (butcher block, grinder, altar).
public abstract class ActiveStation : Station
{
    public PlayerHands User { get; private set; }

    protected void Begin(PlayerHands hands)
    {
        User = hands;
        hands.Busy = this;
    }

    // The player bailed out (or, later, got hit by a tentacle). Progress stays on the station.
    public void Leave()
    {
        if (User == null) return;
        User.Busy = null;
        User = null;
        OnLeave();
    }

    protected virtual void OnLeave() { }
}
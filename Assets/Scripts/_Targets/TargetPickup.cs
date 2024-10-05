public class TargetPickup : Target
{
    public override void OnHit()
    {
        if (TargetController is not PickUp tc) return;
        tc.OnHit();
    }
}

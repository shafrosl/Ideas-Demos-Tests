public class TargetPickup : Target
{
    public override void OnHit()
    {
        if (TargetController is not TargetControllerPickUp tc) return;
        tc.OnHit();
    }
}

public class TargetPickup : Target
{
    public override async void OnHit()
    {
        if (TargetController is not HealthKit tc) return;
        tc.OnHit();
    }
}

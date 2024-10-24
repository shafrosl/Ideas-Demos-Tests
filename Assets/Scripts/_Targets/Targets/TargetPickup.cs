using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TargetPickup : BaseTarget
{
    public override void OnHit()
    {
        if (TargetController is not TargetControllerPickUp tc) return;
        tc.OnHit();
    }
}

using Cysharp.Threading.Tasks;
using UnityEngine;

public class TargetHead : Target
{
    protected override void TargetUpdate()
    {
        if (TargetController is not TargetController tc) return;
        if (tc.HeadShot && !tc.HeadShot.IsCounting) tc.HeadShot = null;
    }

    public override UniTask InstantiateHole(Vector3 position, Vector3 worldPosition, SpriteRenderer SR, Vector2 offset)
    {
        if (TargetController is not TargetController tc) return UniTask.CompletedTask;
        if (!tc.HeadShot) tc.HeadShot = GameManager.Instance.PoolController.InstantiateHit(worldPosition);
        else tc.HeadShot.IncreaseSize();
        return base.InstantiateHole(position, worldPosition, SR, offset);
    }
    
    public override void OnHit() => GameManager.Instance.PlayerStats.Score.HeadShots++;
}

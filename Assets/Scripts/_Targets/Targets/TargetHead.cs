using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TargetHead : BaseTarget
{
    protected override void TargetUpdate()
    {
        if (TargetController is not TargetControllerEnemy tc) return;
        if (tc.HeadShot && !tc.HeadShot.IsCounting) tc.HeadShot = null;
    }

    public override UniTask InstantiateHole(Vector3 worldPosition, SpriteRenderer SR, Vector3 offset)
    {
        if (TargetController is not TargetControllerEnemy tc) return UniTask.CompletedTask;
        if (!tc.HeadShot) tc.HeadShot = GameManager.Instance.PoolController.InstantiateHit(worldPosition);
        else tc.HeadShot.IncreaseSize();
        return base.InstantiateHole(worldPosition, SR, offset);
    }
    
    public override void OnHit() => GameManager.Instance.PlayerStats.Score.HeadShots++;
}

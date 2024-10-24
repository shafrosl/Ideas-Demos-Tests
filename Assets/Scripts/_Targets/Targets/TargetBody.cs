using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TargetBody : BaseTarget
{
    protected override void TargetUpdate()
    {
        if (TargetController is not TargetControllerEnemy tc) return;
        if (tc.BodyShot && !tc.BodyShot.IsCounting) tc.BodyShot = null;
    }

    public override UniTask InstantiateHole(Vector3 position, Vector3 worldPosition, SpriteRenderer SR, Vector2 offset)
    {
        if (TargetController is not TargetControllerEnemy tc) return UniTask.CompletedTask;
        if (!tc.BodyShot) tc.BodyShot = GameManager.Instance.PoolController.InstantiateHit(worldPosition);
        else tc.BodyShot.IncreaseSize();
        return base.InstantiateHole(position, worldPosition, SR, offset);
    }

    public override void OnHit() => GameManager.Instance.PlayerStats.Score.BodyShots++;
}

using Cysharp.Threading.Tasks;
using UnityEngine;

public class TargetHead : Target
{
    protected override void TargetUpdate()
    {
        if (TargetController.HeadShot && !TargetController.HeadShot.IsCounting) TargetController.HeadShot = null;
    }

    public override UniTask InstantiateHole(Vector3 position, Vector3 worldPosition, SpriteRenderer SR, Vector2 offset)
    {
        if (!TargetController.HeadShot) TargetController.HeadShot = GameManager.Instance.PoolController.InstantiateHit(worldPosition);
        else TargetController.HeadShot.IncreaseSize();
        return base.InstantiateHole(position, worldPosition, SR, offset);
    }
    
    public override void AddScore() => GameManager.Instance.PlayerStats.Score.HeadShots++;
}

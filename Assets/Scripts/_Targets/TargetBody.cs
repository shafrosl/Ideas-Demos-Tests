using Cysharp.Threading.Tasks;
using UnityEngine;

public class TargetBody : Target
{
    protected override void TargetUpdate()
    {
        if (TargetController.BodyShot && !TargetController.BodyShot.IsCounting) TargetController.BodyShot = null;
    }

    public override UniTask InstantiateHole(Vector3 position, Vector3 worldPosition, SpriteRenderer SR, Vector2 offset)
    {
        if (!TargetController.BodyShot) TargetController.BodyShot = GameManager.Instance.PoolController.InstantiateHit(worldPosition);
        else TargetController.BodyShot.IncreaseSize();
        return base.InstantiateHole(position, worldPosition, SR, offset);
    }

    public override void AddScore() => GameManager.Instance.PlayerStats.Score.BodyShots++;
}

using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TargetWall : BaseTarget
{
    public override UniTask InstantiateHole(Vector3 worldPosition, SpriteRenderer SR, Vector3 offset)
    {
        var holeObj = GameManager.Instance.PoolController.InstantiateHole(SR, worldPosition, offset, holeCount++, out var holeRenderer);
        var spark = GameManager.Instance.PoolController.InstantiateSparks(holeObj.transform.position, holeObj.transform.rotation);
        if (spark.TryGetComponent(out ParticleSystemRenderer ps)) ps.sortingOrder = holeRenderer.sortingOrder + 1;
        return UniTask.CompletedTask;
    }
}

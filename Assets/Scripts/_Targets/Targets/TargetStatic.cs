using Cysharp.Threading.Tasks;
using UnityEngine;

public class TargetStatic : BaseTarget
{
    public override UniTask InstantiateHole(Vector3 worldPosition, SpriteRenderer SR, Vector3 normal)
    {
        var holeObj = GameManager.Instance.PoolController.InstantiateHole(transform, worldPosition, normal, holeCount++, out var holeRenderer);
        var spark = GameManager.Instance.PoolController.InstantiateSparks(holeObj.transform.position, holeObj.transform.rotation);
        if (spark.TryGetComponent(out ParticleSystemRenderer ps))
        {
            spark.transform.localEulerAngles = TargetController.PositionDotValue < 0 ? new Vector3(0, 0, 0) : new Vector3(0, 180, 0);
            ps.sortingOrder = holeRenderer.sortingOrder + 1;
        }
        TargetController.Rigidbody.AddForce(-normal.x * 100 * GameManager.Instance.PlayerController.Damage, -normal.y * 100 * GameManager.Instance.PlayerController.Damage, -normal.z * 100 * GameManager.Instance.PlayerController.Damage);
        OnHit();
        return UniTask.CompletedTask;
    }
}

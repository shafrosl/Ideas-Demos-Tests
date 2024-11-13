using System;
using Cysharp.Threading.Tasks;
using MyBox;
using UnityEngine;
using Utility;

public abstract class BaseTarget : MonoBehaviour, IBulletHole, IScore
{
    [ConditionalField(nameof(TargetController), true), SerializeField] public BaseTargetController TargetController;
    protected int holeCount;

    public virtual UniTask InstantiateHole(Vector3 worldPosition, SpriteRenderer SR, Vector3 normal)
    {
        var holeObj = GameManager.Instance.PoolController.InstantiateHole(SR, worldPosition, normal, holeCount++, out var holeRenderer);
        var spark = GameManager.Instance.PoolController.InstantiateSparks(holeObj.transform.position, holeObj.transform.rotation);
        if (spark.TryGetComponent(out ParticleSystemRenderer ps))
        {
            spark.transform.localEulerAngles = TargetController.PositionDotValue < 0 ? new Vector3(0, 0, 0) : new Vector3(0, 180, 0);
            ps.sortingOrder = holeRenderer.sortingOrder + 1;
        }

        if (Math.Abs(TargetController.transform.parent.localEulerAngles.y - 90) < 10f || Math.Abs(TargetController.transform.parent.localEulerAngles.y - 270) < 10f)
        {
            TargetController.Rigidbody.AddForce(100 * GameManager.Instance.PlayerController.Damage, 0, 0);
        }
        else
        {
            TargetController.Rigidbody.AddForce(0, 0, 100 * GameManager.Instance.PlayerController.Damage);
        }
        
        OnHit();
        return UniTask.CompletedTask;
    }

    public virtual UniTask InstantiateHole(Vector3 worldPosition, SpriteRenderer SR) => InstantiateHole(worldPosition, SR, Vector3.zero);
    public virtual void OnHit() { }
    protected virtual void TargetUpdate() { }
    private void Update() => TargetUpdate();
}

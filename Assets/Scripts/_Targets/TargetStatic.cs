using Cysharp.Threading.Tasks;
using UnityEngine;
using Utility;

public class TargetStatic : Target
{
    public override UniTask InstantiateHole(Vector3 position, Vector3 normal, SpriteRenderer SR, Vector2 offset)
    {
        var randHole = Random.Range(0, GameManager.Instance.BulletHoles.Length);
        var randZ = Random.Range(0, 360);
        var holeObj = new GameObject("Hole")
        {
            transform =
            {
                parent = transform.parent,
                localPosition = position + (normal * 0.01f),
                localRotation = Quaternion.FromToRotation(Vector3.forward, normal),
                localScale = Vector3.one,
            }
        };

        holeObj.transform.localEulerAngles.Modify(z: randZ);
        var holeRenderer = holeObj.AddComponent<SpriteRenderer>();
        holeRenderer.sprite = GameManager.Instance.BulletHoles[randHole];
        holeRenderer.sortingLayerName = "In Front";
        holeRenderer.sortingOrder = ++holeCount;
        GameManager.Instance.Holes.Add(holeObj);
        var spark = Instantiate(GameManager.Instance.Sparks, holeObj.transform);
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

using Cysharp.Threading.Tasks;
using UnityEngine;
using Debug = Utility.Debug;

[RequireComponent(typeof(SpriteRenderer))]
public class WallTarget : Target
{
    public override UniTask InstantiateHole(Vector3 position, Vector3 worldPosition, SpriteRenderer SR, Vector2 offset)
    {
        var randHole = Random.Range(0, GameManager.Instance.BulletHoles.Length);
        var randZ = Random.Range(0, 360);
        var holeObj = new GameObject("Hole")
        {
            transform =
            {
                parent = SR.transform,
                localPosition = new Vector3(position.x - offset.x, position.y - offset.y, 0),
                localEulerAngles = new Vector3(0, 0, randZ),
                localScale = Vector3.one
            }
        };

        var holeRenderer = holeObj.AddComponent<SpriteRenderer>();
        holeRenderer.sprite = GameManager.Instance.BulletHoles[randHole];
        holeRenderer.sortingLayerName = "In Front";
        holeRenderer.sortingOrder = ++holeCount + SR.sortingOrder;
        GameManager.Instance.Holes.Add(holeObj);
        var spark = Instantiate(GameManager.Instance.Sparks, holeObj.transform);
        if (spark.TryGetComponent(out ParticleSystemRenderer ps)) ps.sortingOrder = holeRenderer.sortingOrder + 1;
        return UniTask.CompletedTask;
    }
}

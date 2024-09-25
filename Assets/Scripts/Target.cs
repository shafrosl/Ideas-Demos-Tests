using Cysharp.Threading.Tasks;
using UnityEngine;
using Debug = Utility.Debug;
using Random = UnityEngine.Random;

public class Target : MonoBehaviour, IBulletHole
{
    public SpriteRenderer TargetRenderer;
    public Rigidbody Rigidbody;
    public Sprite[] Holes;
    private int holeCount;

    public UniTask InstantiateHole(Vector3 position)
    {
        var randHole = Random.Range(0, Holes.Length);
        var randZ = Random.Range(0, 360);
        
        var holeObj = new GameObject("Hole")
        {
            transform =
            {
                parent = TargetRenderer.transform,
                localPosition = new Vector3(position.x, position.y, 0),
                localEulerAngles = new Vector3(0, 0, randZ)
            }
        };

        var holeRenderer = holeObj.AddComponent<SpriteRenderer>();
        holeRenderer.sprite = Holes[randHole];
        holeRenderer.sortingLayerName = "In Front";
        holeRenderer.sortingOrder = ++holeCount + TargetRenderer.sortingOrder;
        GameManager.Instance.Holes.Add(holeObj);
        var spark = Instantiate(GameManager.Instance.Sparks, holeObj.transform);
        if (spark.TryGetComponent(out ParticleSystemRenderer ps)) ps.sortingOrder = holeRenderer.sortingOrder + 1;
        Rigidbody.AddForce(0, 0, 100 * GameManager.Instance.PlayerController.Damage);
        return UniTask.CompletedTask;
    }
}

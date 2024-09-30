using Cysharp.Threading.Tasks;
using MyBox;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class Target : MonoBehaviour, IBulletHole, IScore
{
    [ConditionalField(nameof(TargetController), true), SerializeField] public TargetController TargetController;
    protected int holeCount;

    public virtual UniTask InstantiateHole(Vector3 position, SpriteRenderer SR, Vector2 offset)
    {
        var randHole = Random.Range(0, TargetController.Holes.Length);
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
        holeRenderer.sprite = TargetController.Holes[randHole];
        holeRenderer.sortingLayerName = "In Front";
        holeRenderer.sortingOrder = ++holeCount + SR.sortingOrder;
        GameManager.Instance.Holes.Add(holeObj);
        var spark = Instantiate(GameManager.Instance.Sparks, holeObj.transform);
        if (spark.TryGetComponent(out ParticleSystemRenderer ps)) ps.sortingOrder = holeRenderer.sortingOrder + 1;
        TargetController.Rigidbody.AddForce(0, 0, 100 * GameManager.Instance.PlayerController.Damage);
        AddScore();
        return UniTask.CompletedTask;
    }

    public virtual UniTask InstantiateHole(Vector3 position, SpriteRenderer SR) => InstantiateHole(position, SR, Vector2.zero);
    
    public virtual void AddScore() { }
}

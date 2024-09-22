using Cysharp.Threading.Tasks;
using UnityEngine;
using Debug = Utility.Debug;
using Random = UnityEngine.Random;

public class Target : MonoBehaviour, IBulletHole
{
    public SpriteRenderer TargetRenderer;
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
                parent = transform,
                localPosition = new Vector3(position.x, position.y, 0),
                localEulerAngles = new Vector3(0, 0, randZ)
            }
        };

        var holeRenderer = holeObj.AddComponent<SpriteRenderer>();
        holeRenderer.sprite = Holes[randHole];
        holeRenderer.sortingLayerName = "In Front";
        holeRenderer.sortingOrder = ++holeCount + TargetRenderer.sortingOrder;
        GameManager.Instance.Holes.Add(holeObj);
        return UniTask.CompletedTask;
    }
}

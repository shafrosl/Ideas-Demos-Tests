using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Utility;

public class PoolController : MonoBehaviour
{
    [SerializeReference] private Sprite[] BulletHoles;
    [SerializeReference] private GameObject TextPopUp;
    [SerializeReference] private GameObject Spark;
    private List<TextPopUp> TextPopUps = new();
    private List<Particle> Sparks = new();
    private List<GameObject> Holes = new();

    public TextPopUp InstantiateHit(Vector3 position)
    {
        foreach (var pop in TextPopUps.Where(pop => !pop.gameObject.activeSelf))
        {
            pop.Instantiate(position);
            return pop;
        }
        
        var p = Instantiate(TextPopUp).GetComponent<TextPopUp>().Instantiate(position);
        TextPopUps.Add(p);
        return p;
    }
    
    public Particle InstantiateSparks(Vector3 position, Quaternion rotation)
    {
        if (!GameManager.Instance.GameStarted) return null;
        foreach (var particle in Sparks.Where(part => part.IsStopped()))
        {
            SetPSPosition(particle, position, rotation);
            return particle;
        }
        
        var p = Instantiate(Spark).GetComponent<Particle>();
        SetPSPosition(p, position, rotation);
        Sparks.Add(p);
        p.Play();
        return p;
    }

    public GameObject InstantiateHole(SpriteRenderer SR, Vector3 position, Vector3 offset, int holeCount, out SpriteRenderer holeRenderer)
    {
        var randZ = Random.Range(0, 360);
        var holeObj = new GameObject("Hole")
        {
            transform =
            {
                position = new Vector3(position.x - offset.x, position.y - offset.y, 0),
                localEulerAngles = new Vector3(0, 0, randZ),
                localScale = Vector3.one,
                parent = SR.transform,
            }
        };

        holeRenderer = holeObj.AddComponent<SpriteRenderer>();
        holeRenderer.sprite = BulletHoles.RandomValue();
        holeRenderer.sortingLayerName = "In Front";
        holeRenderer.sortingOrder = holeCount + SR.sortingOrder;
        Holes.Add(holeObj);
        return holeObj;
    }
    
    public GameObject InstantiateHole(Transform parent, Vector3 position, Vector3 normal, int holeCount, out SpriteRenderer holeRenderer)
    {
        var randZ = Random.Range(0, 360);
        var holeObj = new GameObject("Hole")
        {
            transform =
            {
                position = position + (normal * 0.01f),
                localRotation = Quaternion.FromToRotation(Vector3.forward, normal),
                localScale = Vector3.one,
                parent = parent,
            }
        };

        holeObj.transform.localEulerAngles.Modify(z: randZ);
        
        holeRenderer = holeObj.AddComponent<SpriteRenderer>();
        holeRenderer.sprite = BulletHoles.RandomValue();
        holeRenderer.sortingLayerName = "In Front";
        holeRenderer.sortingOrder = holeCount;
        Holes.Add(holeObj);
        return holeObj;
    }
    
    private void SetPSPosition(Particle particle, Vector3 position, Quaternion rotation)
    {
        if (!particle.gameObject.activeSelf) particle.gameObject.SetActive(true);
        particle.transform.localScale = Vector3.one;
        particle.transform.position = position;
        particle.transform.rotation = rotation;
        particle.transform.parent = GameManager.Instance.PoolController.transform;
        particle.Play();
    }

    public UniTask ClearPool()
    {
        if (TextPopUps.IsSafe())
        {
            foreach (var text in TextPopUps)
            {
                Destroy(text.gameObject);
            }
            TextPopUps.Clear();
        }
        
        if (Sparks.IsSafe())
        {
            foreach (var spark in Sparks)
            {
                Destroy(spark.gameObject);
            }
            Sparks.Clear();
        }
        
        if (Holes.IsSafe())
        {
            foreach (var hole in Holes)
            {
                Destroy(hole);
            }
            Holes.Clear();
        }
        
        return UniTask.CompletedTask;
    }
}

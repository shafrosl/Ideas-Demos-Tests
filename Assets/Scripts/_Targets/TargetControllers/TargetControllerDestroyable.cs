using Cysharp.Threading.Tasks;
using UnityEngine;

public class TargetControllerDestroyable : TargetControllerStatic
{
    private int hitsAllowed;
    public int HitsAllowed;
    public GameObject Mesh;
    public Fracture Fracture;

    public override void SpecialisedTask()
    {
        hitsAllowed--;
        if (hitsAllowed <= 0) DestructObject();
    }

    protected override UniTask Initialize()
    {
        if (isInitializing) return UniTask.CompletedTask;
        isInitializing = true;
        if (isInitialized) return UniTask.CompletedTask;
        if (!GameManager.Instance.GameStarted) return UniTask.CompletedTask;
        hitsAllowed = HitsAllowed;
        SnapToGround();
        isInitialized = true;
        isInitializing = false;
        return UniTask.CompletedTask;
    }

    public override UniTask PostInitialize()
    {
        if (isPostInitialized) return UniTask.CompletedTask;
        isPostInitialized = true;
        Rigidbody.velocity = Vector3.zero;
        return base.PostInitialize();
    }

    private void DestructObject()
    {
        Mesh.gameObject.SetActive(false);
        Fracture.gameObject.SetActive(true);
        Fracture.Explode();
    }

    private void SnapToGround()
    {
        var hits = Physics.RaycastAll(transform.position + Vector3.up, Vector3.down, 10f);
        foreach(var hit in hits)
        {
            if (hit.collider.transform.parent.parent.gameObject == transform.parent.parent.gameObject) continue;
            transform.position = hit.point;
            break;
        }
    }
}

using Cysharp.Threading.Tasks;
using UnityEngine;

public class TargetControllerDestroyable : TargetControllerStatic
{
    private int hitsAllowed;
    public int HitsAllowed;

    public ParticleSystem ExplodeParticles;

    public override void SpecialisedTask()
    {
        hitsAllowed--;
        Debug.Log("hit! " + hitsAllowed);
        if (hitsAllowed <= 0) DestructObject();
    }

    protected override UniTask Initialize()
    {
        if (isInitializing) return UniTask.CompletedTask;
        isInitializing = true;
        if (isInitialized) return UniTask.CompletedTask;
        if (!GameManager.Instance.GameStarted) return UniTask.CompletedTask;
        hitsAllowed = HitsAllowed;
        isInitialized = true;
        isInitializing = false;
        return UniTask.CompletedTask;
    }

    private void DestructObject()
    {
        Debug.Log("DESTROYED!");
    }
}

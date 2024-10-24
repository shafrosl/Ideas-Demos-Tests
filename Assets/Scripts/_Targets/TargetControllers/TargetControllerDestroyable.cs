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
        if (hitsAllowed <= 0) DestructObject();
    }

    protected override UniTask Initialize()
    {
        hitsAllowed = HitsAllowed;
        return base.Initialize();
    }

    private void DestructObject()
    {
        
    }
}

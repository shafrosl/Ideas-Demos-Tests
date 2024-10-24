using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(HingeJoint))]
public abstract class TargetControllerPickUp : BaseTargetController
{
    public float yPosBase;
    public SpriteRenderer SR;

    private void Start() => yPosBase = transform.parent.localPosition.y;

    protected override async UniTask<UniTask> ControllerUpdate()
    {
        await base.ControllerUpdate();
        Alert();
        return UniTask.CompletedTask;
    }
    
    protected override UniTask Initialize()
    {
        if (isInitializing) return UniTask.CompletedTask;
        isInitializing = true;
        if (isInitialized) return UniTask.CompletedTask;
        if (!GameManager.Instance.GameStarted) return UniTask.CompletedTask;
        internalCountdown = 10;
        isInitialized = true;
        isInitializing = false;
        return UniTask.CompletedTask;
    }
    
    protected virtual async void Alert()
    {
        if (internalCountdown <= 0)
        {
            var parent = transform.parent;
            await parent.DOJump(new Vector3(parent.position.x, yPosBase, parent.position.z), 0.6f, 2, 0.25f).WithCancellation(ctx.Token).SuppressCancellationThrow();
            internalCountdown = 10;
        }
        else
        {
            internalCountdown -= Time.deltaTime;
        }
    }
    
    public virtual void OnHit() { }
}

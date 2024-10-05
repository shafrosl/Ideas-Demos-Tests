using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class PickUp : BaseTargetController
{
    public SpriteRenderer SR;
    protected override void ControllerUpdate() => Alert();
    private float yPosBase;

    private void Start()
    {
        yPosBase = transform.parent.localPosition.y;
        internalCountdown = 10;
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

using System.Threading;
using Cysharp.Threading.Tasks;
using MyBox;
using UnityEngine;
using DG.Tweening;
using Utility;

public class Fracture : MonoBehaviour
{
    public Rigidbody[] Rigidbodies;
    private CancellationTokenSource ctx = new();
    
    [ButtonMethod()]
    public void Explode()
    {
        if (!Rigidbodies.IsSafe()) return;
        foreach (var body in Rigidbodies)
        {
            body.isKinematic = false;
            body.useGravity = true;
            body.AddExplosionForce(30, transform.position, 0.25f, 2, ForceMode.Impulse);
            ShrinkAndDestroy(body);
        }

        GameManager.Instance.PlayerController.ExplosionImpulse.GenerateImpulseWithForce(0.65f);
        Rigidbodies = null;
    }

    private async void ShrinkAndDestroy(Component rb)
    {
        await UniTask.Delay(500);
        await DOTween.To(() => rb.transform.localScale, x => rb.transform.localScale = x, Vector3.zero, 2.5f).WithCancellation(ctx.Token).SuppressCancellationThrow();
        Destroy(rb.gameObject);
    }
}

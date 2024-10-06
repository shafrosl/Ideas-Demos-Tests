using Cysharp.Threading.Tasks;
using DG.Tweening;

public class TargetControllerHealthKit : TargetControllerPickUp
{
    public override async void OnHit()
    {
        GameManager.Instance.PlayerController.OnHeal();
        await SR.DOFade(0, 0.2f).WithCancellation(ctx.Token).SuppressCancellationThrow();
        Destroy(gameObject);
    }

    protected override void Alert()
    {
        if (GameManager.Instance.PlayerStats.Health > GameManager.Instance.PlayerStats.FullHealth / 2) return;
        base.Alert();
    }
}

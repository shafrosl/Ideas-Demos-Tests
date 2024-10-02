using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : BaseController
{
    public TextMeshProUGUI TotalNum, CurrNum, Slash;
    public Image Bullet;
    private bool SwitchColor;

    private CancellationTokenSource ctx = new();
    
    public void SetTotal(int num) => TotalNum.text = num.ToString();
    public void SetCurrent(int num) => CurrNum.text = num.ToString();
    
    public async void AnimateShot()
    {
        DOTween.To(() => CurrNum.color, x => CurrNum.color = x, GameManager.Instance.Yellow, 0.1f).WithCancellation(ctx.Token).SuppressCancellationThrow();
        await DOTween.To(() => Bullet.transform.localScale, x => Bullet.transform.localScale = x, new Vector3(4.0f, 4.0f, 4.0f), 0.1f).WithCancellation(ctx.Token).SuppressCancellationThrow();
        DOTween.To(() => CurrNum.color, x => CurrNum.color = x, SwitchColor ? GameManager.Instance.White : GameManager.Instance.Black, 0.1f).WithCancellation(ctx.Token).SuppressCancellationThrow();
        await DOTween.To(() => Bullet.transform.localScale, x => Bullet.transform.localScale = x, new Vector3(3.0f, 3.0f, 3.0f), 0.1f).WithCancellation(ctx.Token).SuppressCancellationThrow();
    }

    public async void OutOfBullets()
    {
        Bullet.transform.DOShakeRotation(0.1f, new Vector3(0, 0, 15), 3, 10, true, ShakeRandomnessMode.Harmonic).WithCancellation(ctx.Token).SuppressCancellationThrow();
        await DOTween.To(() => CurrNum.color, x => CurrNum.color = x, GameManager.Instance.Red, 0.1f).WithCancellation(ctx.Token).SuppressCancellationThrow();
        await DOTween.To(() => CurrNum.color, x => CurrNum.color = x, SwitchColor ? GameManager.Instance.White : GameManager.Instance.Black, 0.1f).WithCancellation(ctx.Token).SuppressCancellationThrow();
    }

    public void SwitchTextColor(bool switchColor)
    {
        SwitchColor = switchColor;
        if (!GameManager.Instance.PlayerController.isShooting) CurrNum.color = TotalNum.color = Slash.color = SwitchColor ? GameManager.Instance.White : GameManager.Instance.Black;
    }
}

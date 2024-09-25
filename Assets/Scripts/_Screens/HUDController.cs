using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : BaseController
{
    public TextMeshProUGUI TotalNum, CurrNum;
    public Image Bullet;

    private CancellationTokenSource ctx = new();
    
    public void SetTotal(int num) => TotalNum.text = num.ToString();
    public void SetCurrent(int num) => CurrNum.text = num.ToString();
    
    public async void AnimateShot()
    {
        DOTween.To(() => CurrNum.color, x => CurrNum.color = x, GameManager.Instance.Yellow, 0.1f).WithCancellation(ctx.Token).SuppressCancellationThrow();
        await DOTween.To(() => Bullet.transform.localScale, x => Bullet.transform.localScale = x, new Vector3(4.0f, 4.0f, 4.0f), 0.1f).WithCancellation(ctx.Token).SuppressCancellationThrow();
        DOTween.To(() => CurrNum.color, x => CurrNum.color = x, GameManager.Instance.Black, 0.1f).WithCancellation(ctx.Token).SuppressCancellationThrow();
        await DOTween.To(() => Bullet.transform.localScale, x => Bullet.transform.localScale = x, new Vector3(3.0f, 3.0f, 3.0f), 0.1f).WithCancellation(ctx.Token).SuppressCancellationThrow();
    }

    public async void OutOfBullets()
    {
        Bullet.transform.DOShakeRotation(0.1f, new Vector3(0, 0, 15), 3, 10, true, ShakeRandomnessMode.Harmonic).WithCancellation(ctx.Token).SuppressCancellationThrow();
        await DOTween.To(() => CurrNum.color, x => CurrNum.color = x, GameManager.Instance.Red, 0.1f).WithCancellation(ctx.Token).SuppressCancellationThrow();
        await DOTween.To(() => CurrNum.color, x => CurrNum.color = x, GameManager.Instance.Black, 0.1f).WithCancellation(ctx.Token).SuppressCancellationThrow();
    }
}

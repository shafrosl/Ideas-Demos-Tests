using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

public class HUDController : BaseController
{
    public TextMeshProUGUI TotalNum, CurrNum, Slash;
    public Image Bullet;
    public Image Cover;
    public Image[] Hearts;
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
        TotalNum.color = Slash.color = SwitchColor ? GameManager.Instance.White : GameManager.Instance.Black;
        if (!GameManager.Instance.PlayerController.isShooting) CurrNum.color = SwitchColor ? GameManager.Instance.White : GameManager.Instance.Black;
    }

    public async void Hit()
    {
        for (var i = 0; i < Hearts.Length; i++)
        {
            if (!Hearts[i].gameObject.activeSelf) continue;
            DOTween.To(() => Hearts[i].color, x => Hearts[i].color = x, GameManager.Instance.Red, 0.75f).WithCancellation(ctx.Token).SuppressCancellationThrow();
            await Hearts[i].transform.DOShakeRotation(0.75f, new Vector3(0, 0, 15), 20, 2, true, ShakeRandomnessMode.Harmonic).WithCancellation(ctx.Token).SuppressCancellationThrow();
            DOTween.To(() => Hearts[i].color, x => Hearts[i].color = x, GameManager.Instance.White, 0.2f).WithCancellation(ctx.Token).SuppressCancellationThrow();
            await Hearts[i].DOFade(0, 1f).WithCancellation(ctx.Token).SuppressCancellationThrow();
            Hearts[i].gameObject.SetActive(false);
            break;
        }
    }

    public async void Heal()
    {
        for (var i = 0; i < Hearts.Length; i++)
        {
            if (Hearts[i].gameObject.activeSelf) continue;            
            Hearts[i].gameObject.SetActive(true);
            DOTween.To(() => Hearts[i].color, x => Hearts[i].color = x, GameManager.Instance.Green, 0.5f).WithCancellation(ctx.Token).SuppressCancellationThrow();
            await Hearts[i].DOFade(1, 0.5f).WithCancellation(ctx.Token).SuppressCancellationThrow();
            DOTween.To(() => Hearts[i].color, x => Hearts[i].color = x, GameManager.Instance.White, 0.1f).WithCancellation(ctx.Token).SuppressCancellationThrow();
            break;
        }
    }

    public UniTask ResetHearts(int num = 5)
    {
        for (var i = 0; i < num; i++)
        {
            Hearts[i].gameObject.SetActive(true);
            Hearts[i].color = Hearts[i].color.Modify(a: 1);
        }
        
        return UniTask.CompletedTask;
    }

    public async UniTask<UniTask> FadeCover(bool fadeIn)
    {
        await Cover.DOFade(fadeIn ? 1 : 0, 0.25f).SetUpdate(true);
        return UniTask.CompletedTask;
    }
}

using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

public class HUDController : BaseController
{
    public TextMeshProUGUI TotalNum, CurrNum, Slash;
    public Image Bullet;
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
        if (!GameManager.Instance.PlayerController.isShooting) CurrNum.color = TotalNum.color = Slash.color = SwitchColor ? GameManager.Instance.White : GameManager.Instance.Black;
    }

    public async void Hit()
    {
        for (var i = 0; i < Hearts.Length; i++)
        {
            if (!Hearts[i].gameObject.activeSelf) continue;
            await Hearts[i].transform.DOShakeRotation(0.75f, new Vector3(0, 0, 15), 20, 2, true, ShakeRandomnessMode.Harmonic).WithCancellation(ctx.Token).SuppressCancellationThrow();
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
            await Hearts[i].DOFade(1, 0.5f).WithCancellation(ctx.Token).SuppressCancellationThrow();
            break;
        }
    }

    public UniTask ResetHearts(int num = 5)
    {
        var numOfHearts = num;
        foreach (var heart in Hearts)
        {
            heart.gameObject.SetActive(true);
            heart.color.Modify(a: 1);
            numOfHearts--;
            if (numOfHearts == 0) break;
        }
        
        return UniTask.CompletedTask;
    }
}

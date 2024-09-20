using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyBox;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    [ConditionalField(nameof(Canvas), true), SerializeField] public Canvas Canvas;
    [ConditionalField(nameof(CanvasGroup), true), SerializeField] public CanvasGroup CanvasGroup;

    public virtual async UniTask Start()
    {
        while (!GameManager.Instance.IsReady) await UniTask.Delay(50);
        await Initialize();
    }

    public virtual UniTask Initialize() => UniTask.CompletedTask;

    public virtual async UniTask<UniTask> ToggleScreen(bool instant)
    {
        var ct = this.GetCancellationTokenOnDestroy();
        if (CanvasGroup.alpha == 0)
        {
            await CanvasGroup.DOFade(1, instant ? 0 : 0.5f).SetUpdate(true).WithCancellation(ct);
            CanvasGroup.interactable = CanvasGroup.blocksRaycasts = true;
        }
        else
        {
            await CanvasGroup.DOFade(0, instant ? 0 : 0.5f).SetUpdate(true).WithCancellation(ct);
            CanvasGroup.interactable = CanvasGroup.blocksRaycasts = false;
        }
        
        return UniTask.CompletedTask;
    }
    
    public virtual async UniTask<UniTask> ToggleScreen(bool instant, bool show)
    {
        var ct = this.GetCancellationTokenOnDestroy();
        if (show)
        {
            await CanvasGroup.DOFade(1, instant ? 0 : 0.5f).SetUpdate(true).WithCancellation(ct);
            CanvasGroup.interactable = CanvasGroup.blocksRaycasts = true;
        }
        else
        {
            await CanvasGroup.DOFade(0, instant ? 0 : 0.5f).SetUpdate(true).WithCancellation(ct);
            CanvasGroup.interactable = CanvasGroup.blocksRaycasts = false;
        }

        return UniTask.CompletedTask;
    }
}
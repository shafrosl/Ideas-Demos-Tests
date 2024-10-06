using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class LoadingController : BaseController
{
    public override async UniTask<UniTask> ToggleScreen(bool instant)
    {
        var ct = this.GetCancellationTokenOnDestroy();
        if (CanvasGroup.alpha == 0)
        {
            if (GameManager.Instance.GameStarted)
            {
                Time.timeScale = 0;
                GameManager.Instance.PlayerController.lockMovement = true;
            }

            GameManager.Instance.HUDController.FadeCover(true);
            await CanvasGroup.DOFade(1, .5f).SetUpdate(true).WithCancellation(ct).SuppressCancellationThrow();
            CanvasGroup.interactable = CanvasGroup.blocksRaycasts = true;
            GameManager.Instance.ToggleCamera(Cam.UI);
            GameManager.Instance.HUDController.FadeCover(false);
        }
        else
        {
            GameManager.Instance.HUDController.FadeCover(true);
            GameManager.Instance.ToggleCamera(GameManager.Instance.GameStarted ? Cam.Game : Cam.UI);
            await CanvasGroup.DOFade(0, 0.5f).SetUpdate(true).WithCancellation(ct).SuppressCancellationThrow();
            CanvasGroup.interactable = CanvasGroup.blocksRaycasts = false;
            if (GameManager.Instance.GameStarted) GameManager.Instance.PlayerController.lockMovement = false;
            GameManager.Instance.HUDController.FadeCover(false);
            Time.timeScale = 1;
        }

        return UniTask.CompletedTask;
    }
    
    public override async UniTask<UniTask> ToggleScreen(bool instant, bool show)
    {
        var ct = this.GetCancellationTokenOnDestroy();
        if (show)
        {
            if (GameManager.Instance.GameStarted)
            {
                Time.timeScale = 0;
                GameManager.Instance.PlayerController.lockMovement = true;
            }
            
            GameManager.Instance.HUDController.FadeCover(true);
            await CanvasGroup.DOFade(1, .5f).SetUpdate(true).WithCancellation(ct).SuppressCancellationThrow();
            CanvasGroup.interactable = CanvasGroup.blocksRaycasts = true;
            GameManager.Instance.ToggleCamera(Cam.UI);
            GameManager.Instance.HUDController.FadeCover(false);
        }
        else
        {
            GameManager.Instance.ToggleCamera(GameManager.Instance.GameStarted ? Cam.Game : Cam.UI);
            GameManager.Instance.HUDController.FadeCover(true);
            await CanvasGroup.DOFade(0, 0.5f).SetUpdate(true).WithCancellation(ct).SuppressCancellationThrow();
            CanvasGroup.interactable = CanvasGroup.blocksRaycasts = false;
            if (GameManager.Instance.GameStarted) GameManager.Instance.PlayerController.lockMovement = false;
            GameManager.Instance.HUDController.FadeCover(false);
            Time.timeScale = 1;
        }

        return UniTask.CompletedTask;
    }
}

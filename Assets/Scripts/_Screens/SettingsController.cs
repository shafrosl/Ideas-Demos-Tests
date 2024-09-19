using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Utility;

public class SettingsController : BaseController
{
    public Button MenuBtn, ResumeBtn;
    
    private void ToggleResumeBtn(bool show) => ResumeBtn.gameObject.SetActive(show);
    private async void ResumeGame() => await ToggleScreen(false);

    private async void ReturnToMenu()
    {
        if (!GameManager.Instance.GunSelectController) return;
        if (GameManager.Instance.GameStarted)
        {
            if (!GameManager.Instance.GemsInGame.IsSafe()) return;
            var restored = await GameManager.Instance.GunSelectController.GemScreen.RestoreGems();
            if (!restored) return;
            GameManager.Instance.UICamera.gameObject.SetActive(false);
            GameManager.Instance.GameCamera.gameObject.SetActive(true);
        }
        await GameManager.Instance.GunSelectController.ToggleScreen(true, true);
        await ToggleScreen(false,false);
        GameManager.Instance.ToggleCursorLock(false);
    }

    public override async UniTask<UniTask> ToggleScreen(bool instant)
    {
        var ct = this.GetCancellationTokenOnDestroy();
        ToggleResumeBtn(GameManager.Instance.GameStarted);
        if (CanvasGroup.alpha == 0)
        {
            await CanvasGroup.DOFade(1, .5f).SetUpdate(true).WithCancellation(ct);
            CanvasGroup.interactable = CanvasGroup.blocksRaycasts = true;
            GameManager.Instance.ToggleCursorLock(false);
            GameManager.Instance.ToggleGamePaused(true);
            Time.timeScale = 0;
        }
        else
        {
            await CanvasGroup.DOFade(0, 0.5f).SetUpdate(true).WithCancellation(ct);
            CanvasGroup.interactable = CanvasGroup.blocksRaycasts = false;
            GameManager.Instance.ToggleCursorLock(true);
            GameManager.Instance.ToggleGamePaused(false);
            Time.timeScale = 1;
        }

        return UniTask.CompletedTask;
    }
    
    public override async UniTask<UniTask> ToggleScreen(bool instant, bool show)
    {
        var ct = this.GetCancellationTokenOnDestroy();
        ToggleResumeBtn(GameManager.Instance.GameStarted);
        if (show)
        {
            await CanvasGroup.DOFade(1, .5f).SetUpdate(true).WithCancellation(ct);
            CanvasGroup.interactable = CanvasGroup.blocksRaycasts = true;
            GameManager.Instance.ToggleCursorLock(false);
            GameManager.Instance.ToggleGamePaused(true);
            Time.timeScale = 0;
        }
        else
        {
            await CanvasGroup.DOFade(0, 0.5f).SetUpdate(true).WithCancellation(ct);
            CanvasGroup.interactable = CanvasGroup.blocksRaycasts = false;
            GameManager.Instance.ToggleCursorLock(true);
            GameManager.Instance.ToggleGamePaused(false);
            Time.timeScale = 1;
        }

        return UniTask.CompletedTask;
    }
    
    public override UniTask Initialize()
    {
        MenuBtn.onClick.RemoveAllListeners();
        MenuBtn.onClick.AddListener(ReturnToMenu);
        ResumeBtn.onClick.RemoveAllListeners();
        ResumeBtn.onClick.AddListener(ResumeGame);
        return base.Initialize();
    }
}

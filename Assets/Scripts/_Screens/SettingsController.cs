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

    private async void ReturnToGunSelect()
    {
        if (!GameManager.Instance.GunSelectController) return;
        await GameManager.Instance.LoadingController.ToggleScreen(false, true);
        await ToggleScreen(false,false);
        
        if (GameManager.Instance.GameStarted)
        {
            GameManager.Instance.PoolController.ClearPool();
            if (GameManager.Instance.GameMode == GameMode.TimeCrisis)
            {
                GameManager.Instance.MapController.ClearMap();
            }

            if (GameManager.Instance.GemsInGame.IsSafe())
            {
                var restored = await GameManager.Instance.GunSelectController.GemScreen.RestoreGems();
                if (!restored) return;
            }
            
            GameManager.Instance.GemsInGame.Clear();
            GameManager.Instance.InGame = GameManager.Instance.GameStarted = false;
        }
        
        await GameManager.Instance.ToggleCursorLock(false);
        await GameManager.Instance.GunSelectController.ToggleScreen(true, true);
        if (GameManager.Instance.GameMode == GameMode.GunRange) await GameManager.Instance.GameOverController.ToggleScreen(false, true);
        await GameManager.Instance.LoadingController.ToggleScreen(false, false);
    }

    public override async UniTask<UniTask> ToggleScreen(bool instant)
    {
        var ct = this.GetCancellationTokenOnDestroy();
        await Initialize();
        ToggleResumeBtn(GameManager.Instance.GameStarted);
        if (CanvasGroup.alpha == 0)
        {
            if (GameManager.Instance.GameStarted) GameManager.Instance.PlayerController.lockMovement = true;
            await CanvasGroup.DOFade(1, .5f).SetUpdate(true).WithCancellation(ct);
            CanvasGroup.interactable = CanvasGroup.blocksRaycasts = true;
            GameManager.Instance.ToggleCamera(Cam.UI);
            GameManager.Instance.ToggleCursorLock(false);
            GameManager.Instance.ToggleGamePaused(true);
            Time.timeScale = 0;
        }
        else
        {
            GameManager.Instance.ToggleCamera(GameManager.Instance.GameStarted ? Cam.Game : Cam.UI);
            await CanvasGroup.DOFade(0, 0.5f).SetUpdate(true).WithCancellation(ct);
            CanvasGroup.interactable = CanvasGroup.blocksRaycasts = false;
            GameManager.Instance.ToggleCursorLock(true);
            GameManager.Instance.ToggleGamePaused(false);
            Time.timeScale = 1;
            if (GameManager.Instance.GameStarted) GameManager.Instance.PlayerController.lockMovement = false;
        }

        return UniTask.CompletedTask;
    }
    
    public override async UniTask<UniTask> ToggleScreen(bool instant, bool show)
    {
        var ct = this.GetCancellationTokenOnDestroy();
        await Initialize();
        ToggleResumeBtn(GameManager.Instance.GameStarted);
        if (show)
        {
            if (GameManager.Instance.GameStarted) GameManager.Instance.PlayerController.lockMovement = true;
            await CanvasGroup.DOFade(1, .5f).SetUpdate(true).WithCancellation(ct);
            CanvasGroup.interactable = CanvasGroup.blocksRaycasts = true;
            GameManager.Instance.ToggleCamera(Cam.UI);
            GameManager.Instance.ToggleCursorLock(false);
            GameManager.Instance.ToggleGamePaused(true);
            Time.timeScale = 0;
        }
        else
        {
            GameManager.Instance.ToggleCamera(GameManager.Instance.GameStarted ? Cam.Game : Cam.UI);
            await CanvasGroup.DOFade(0, 0.5f).SetUpdate(true).WithCancellation(ct);
            CanvasGroup.interactable = CanvasGroup.blocksRaycasts = false;
            GameManager.Instance.ToggleCursorLock(true);
            GameManager.Instance.ToggleGamePaused(false);
            Time.timeScale = 1;
            if (GameManager.Instance.GameStarted) GameManager.Instance.PlayerController.lockMovement = false;
        }

        return UniTask.CompletedTask;
    }
    
    protected override UniTask Initialize()
    {
        MenuBtn.onClick.RemoveAllListeners();
        MenuBtn.onClick.AddListener(ReturnToGunSelect);
        ResumeBtn.onClick.RemoveAllListeners();
        ResumeBtn.onClick.AddListener(ResumeGame);
        return base.Initialize();
    }
}

using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using Utility;

public class GameOverController : BaseController
{
    public TextMeshProUGUI Header, Title, Score, Verdict;
    public Button ReturnToGunSelectBtn;

    private void Start() => Initialize();
    
    protected override UniTask Initialize()
    {
        ReturnToGunSelectBtn.onClick.RemoveAllListeners();
        ReturnToGunSelectBtn.onClick.AddListener(ReturnToGunSelect);
        return base.Initialize();
    }

    public override async UniTask<UniTask> ToggleScreen(bool instant)
    {
        var ct = this.GetCancellationTokenOnDestroy();
        if (CanvasGroup.alpha == 0)
        {
            ShowScore();
            if (GameManager.Instance.GameStarted) GameManager.Instance.PlayerController.lockMovement = true;
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
        }

        return UniTask.CompletedTask;
    }
    
    public override async UniTask<UniTask> ToggleScreen(bool instant, bool show)
    {
        var ct = this.GetCancellationTokenOnDestroy();
        if (show)
        {
            ShowScore();
            if (GameManager.Instance.GameStarted) GameManager.Instance.PlayerController.lockMovement = true;
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
        }

        return UniTask.CompletedTask;
    }

    private async void ReturnToGunSelect()
    {
        if (!GameManager.Instance.GunSelectController) return;
        await GameManager.Instance.LoadingController.ToggleScreen(false, true);
        await ToggleScreen(false,false);
        await GameManager.Instance.PoolController.ClearPool();

        if (GameManager.Instance.GameMode == GameMode.TimeCrisis)
        {
            GameManager.Instance.MapController.ClearMap();
        }

        if (GameManager.Instance.GemsInGame.IsSafe())
        {
            var restored = await GameManager.Instance.GunSelectController.GemScreen.RestoreGems();
            if (!restored) return;
        }

        await GameManager.Instance.ToggleCursorLock(false);
        if (!GameManager.Instance.GunSelectController.IsShowing)
        {
            GameManager.Instance.GemsInGame.Clear();
            GameManager.Instance.InGame = GameManager.Instance.GameStarted = false;
            await GameManager.Instance.GunSelectController.ToggleScreen(true, true);
        }
        
        await GameManager.Instance.LoadingController.ToggleScreen(false, false);
    }

    private void ShowScore()
    {
        var score = GameManager.Instance.PlayerStats.Score;
        switch (GameManager.Instance.GameMode)
        {
            case GameMode.GunRange:
                Header.text = "Some things we saw...";
                Title.text = "Head Shots\nBody Shots\nMissed Shots\nTotal Shots\nAccuracy\nScore";
                Score.text = score.HeadShots + "\n" + 
                             score.BodyShots + "\n" + 
                             score.GetMisses() + "\n" +
                             score.TotalShots + "\n" + 
                             score.GetAccuracy() + "%\n" + 
                             score.GetScore();
                score.GetVerdict(Verdict);
                break;
            case GameMode.TimeCrisis:
                Header.text = "Game Over";
                Title.text = "Kills\nHead Shots\nBody Shots\nMissed Shots\nTotal Shots\nAccuracy\nScore";
                Score.text = score.Kills + "\n" + 
                             score.HeadShots + "\n" + 
                             score.BodyShots + "\n" + 
                             score.GetMisses() + "\n" + 
                             score.TotalShots + "\n" + 
                             score.GetAccuracy() + "%\n" + 
                             score.GetScore();
                score.GetVerdict(Verdict);
                break;
            case GameMode.NotSelected:
                break;
        }
    }
}

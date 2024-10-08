using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using Utility;

public class GameOverController : LoadingController
{
    public TextMeshProUGUI Title, Score, Verdict;
    public Button ReturnToGunSelectBtn;

    private void Start() => Initialize();
    
    protected override UniTask Initialize()
    {
        ReturnToGunSelectBtn.onClick.RemoveAllListeners();
        ReturnToGunSelectBtn.onClick.AddListener(ReturnToGunSelect);
        return base.Initialize();
    }

    public override UniTask<UniTask> ToggleScreen(bool instant, bool show)
    {
        if (show) ShowScore();
        return base.ToggleScreen(instant, show);
    }

    public override UniTask<UniTask> ToggleScreen(bool instant)
    {
        if (CanvasGroup.alpha == 0) ShowScore();
        return base.ToggleScreen(instant);
    }

    private async void ReturnToGunSelect()
    {
        if (!GameManager.Instance.GunSelectController) return;
        await GameManager.Instance.LoadingController.ToggleScreen(false, true);
        await ToggleScreen(false,false);
        
        if (GameManager.Instance.GameStarted)
        {
            if (GameManager.Instance.Holes.IsSafe())
            {
                foreach (var hole in GameManager.Instance.Holes) Destroy(hole);
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
        await GameManager.Instance.LoadingController.ToggleScreen(false, false);
    }

    private void ShowScore()
    {
        switch (GameManager.Instance.GameMode)
        {
            case GameMode.GunRange:
                var score = GameManager.Instance.PlayerStats.Score;
                Title.text = "Head Shots\nBody Shots\nMissed Shots\nTotal Shots\nMistakes\nScore";
                Score.text = score.HeadShots + "\n" + score.BodyShots + "\n" + score.GetMisses() + "\n" +
                             score.TotalShots + "\n" + score.Mistakes + "\n" + score.GetScore();
                score.GetVerdict(Verdict);
                break;
            case GameMode.TimeCrisis:
                break;
            case GameMode.NotSelected:
                break;
        }
    }
}

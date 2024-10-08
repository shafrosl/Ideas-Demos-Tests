using Cysharp.Threading.Tasks;
using UnityEngine.UI;

public class MainMenuController : BaseController
{
    public Button GunRange;
    public Button TimeCrisis;

    private void Start() => Initialize();

    protected override UniTask Initialize()
    {
        GunRange.onClick.RemoveAllListeners();
        GunRange.onClick.AddListener(GunRangeStart);
        TimeCrisis.onClick.RemoveAllListeners();
        TimeCrisis.onClick.AddListener(TimeCrisisStart);
        return base.Initialize();
    }

    private async void GunRangeStart()
    {
        GameManager.Instance.GameMode = GameMode.GunRange;
        await ToGunSelect();
    }

    private async void TimeCrisisStart()
    {
        GameManager.Instance.GameMode = GameMode.TimeCrisis;
        await ToGunSelect();
    }

    private async UniTask ToGunSelect()
    {
        ToggleScreen(false, false);
        await GameManager.Instance.GunSelectController.LoadData();
    }
}

using Cysharp.Threading.Tasks;
using MyBox;
using UnityEngine.UI;
using Debug = Utility.Debug;

public class GunSelectController : BaseController
{
    [ConditionalField(nameof(GunSelectScreen), true)] public GunSelectScreen GunSelectScreen;
    [ConditionalField(nameof(GunDataScreen), true)] public GunDataScreen GunDataScreen;
    [ConditionalField(nameof(GemScreen), true)] public GemScreen GemScreen;

    public int currentItem;

    public Button RandomizeAttachment;
    public Button RandomizeGems;
    public Button StartGameBtn;
    public Button BackToMenuBtn;
    
    public override async UniTask LoadData()
    {
        await GameManager.Instance.LoadingController.ToggleScreen(false, true);
        await base.LoadData();
        if (GunSelectScreen) await GunSelectScreen.Initialize();
        if (GemScreen) await GemScreen.Initialize();
        if (GunDataScreen)
        {
            await GunDataScreen.InitializeData(GameManager.Instance.PlayerStats.CurrBodyObj);
            await GunDataScreen.AddData(GameManager.Instance.PlayerStats.CurrStockObj);
            await GunDataScreen.AddData(GameManager.Instance.PlayerStats.CurrBarrelObj);
        }
        await ToggleScreen(true, true);
        await GameManager.Instance.LoadingController.ToggleScreen(false, false);
    }

    protected override UniTask Initialize()
    {
        if (GunSelectScreen)
        {
            GunSelectScreen.Left.onClick.RemoveAllListeners();
            GunSelectScreen.Left.onClick.AddListener(PrevWeapon);
            GunSelectScreen.Right.onClick.RemoveAllListeners();
            GunSelectScreen.Right.onClick.AddListener(NextWeapon);
        }

        if (RandomizeAttachment)
        {
            RandomizeAttachment.onClick.RemoveAllListeners();
            RandomizeAttachment.onClick.AddListener(RandomizeAttachments);
        }
        
        if (RandomizeGems) RandomizeGems.onClick.RemoveAllListeners();
        
        if (StartGameBtn)
        {
            StartGameBtn.onClick.RemoveAllListeners();
            StartGameBtn.onClick.AddListener(GameManager.Instance.StartGame);
        }

        if (BackToMenuBtn)
        {
            BackToMenuBtn.onClick.RemoveAllListeners();
            BackToMenuBtn.onClick.AddListener(BackToMenu);
        }
        
        currentItem = 0;
        return base.Initialize();
    }
    
    private async void PrevWeapon()
    {
        if (currentItem == 0) currentItem = GameManager.Instance.GunData.Bodies.Count - 1;
        else currentItem--;

        if (GunSelectScreen)
        {
            await GunSelectScreen.DestroyGems();
            await GunSelectScreen.UpdateWeapon(currentItem);
        }
        if (GunDataScreen)
        {
            await GunDataScreen.InitializeData(GameManager.Instance.PlayerStats.CurrBodyObj);
            await GunDataScreen.AddData(GameManager.Instance.PlayerStats.CurrStockObj);
            await GunDataScreen.AddData(GameManager.Instance.PlayerStats.CurrBarrelObj);
        }
    }

    private async void NextWeapon()
    {
        if (currentItem == GameManager.Instance.GunData.Bodies.Count - 1) currentItem = 0;
        else currentItem++;

        if (GunSelectScreen)
        {
            await GunSelectScreen.DestroyGems();
            await GunSelectScreen.UpdateWeapon(currentItem);
        }
        if (GunDataScreen)
        {
            await GunDataScreen.InitializeData(GameManager.Instance.PlayerStats.CurrBodyObj);
            await GunDataScreen.AddData(GameManager.Instance.PlayerStats.CurrStockObj);
            await GunDataScreen.AddData(GameManager.Instance.PlayerStats.CurrBarrelObj);
        }
    }
    
    private async void RandomizeAttachments()
    {
        GunSelectScreen.LoadAttachments(currentItem);
        if (GunDataScreen)
        {
            await GunDataScreen.InitializeData(GameManager.Instance.PlayerStats.CurrBodyObj);
            await GunDataScreen.AddData(GameManager.Instance.PlayerStats.CurrStockObj);
            await GunDataScreen.AddData(GameManager.Instance.PlayerStats.CurrBarrelObj);
        }
    }

    public async UniTask StartGame()
    {
        await GameManager.Instance.PlayerStats.SetStats(GameManager.Instance.GunSelectController.GunDataScreen.DataList);
        await GunSelectScreen.StoreGems();
        await GunSelectScreen.DestroyGems();
        await GemScreen.StoreGems();
        await GemScreen.DestroyGems();
    }

    public async void BackToMenu()
    {
        await GameManager.Instance.LoadingController.ToggleScreen(false, true);
        ToggleScreen(true, false);
        await GunSelectScreen.DestroyGems();
        await GemScreen.DestroyGems();
        GameManager.Instance.LoadingController.ToggleScreen(true, false);
        await GameManager.Instance.MainMenuController.ToggleScreen(false, true);
    }
}

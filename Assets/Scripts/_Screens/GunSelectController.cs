using Cysharp.Threading.Tasks;
using MyBox;
using UnityEngine;
using UnityEngine.UI;
using Debug = Utility.Debug;

public class GunSelectController : MonoBehaviour
{
    [ConditionalField(nameof(Canvas), true), SerializeField] public Canvas Canvas;
    [ConditionalField(nameof(GunSelectScreen), true)] public GunSelectScreen GunSelectScreen;
    [ConditionalField(nameof(GunDataScreen), true)] public GunDataScreen GunDataScreen;
    [ConditionalField(nameof(GemScreen), true)] public GemScreen GemScreen;

    public int currentItem;

    public Button RandomizeAttachment;
    public Button RandomizeGems;

    private async void Start()
    {
        while (!GameManager.Instance.IsReady) await UniTask.Delay(50);
        await Initialize();
        if (GunSelectScreen) await GunSelectScreen.Initialize();
        if (GemScreen) await GemScreen.Initialize();
        if (GunDataScreen)
        {
            await GunDataScreen.InitializeData(GameManager.Instance.currBodyObj);
            await GunDataScreen.AddData(GameManager.Instance.currStockObj);
            await GunDataScreen.AddData(GameManager.Instance.currBarrelObj);
        }
    }

    private UniTask Initialize()
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
        currentItem = 0;
        return UniTask.CompletedTask;
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
            await GunDataScreen.InitializeData(GameManager.Instance.currBodyObj);
            await GunDataScreen.AddData(GameManager.Instance.currStockObj);
            await GunDataScreen.AddData(GameManager.Instance.currBarrelObj);
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
            await GunDataScreen.InitializeData(GameManager.Instance.currBodyObj);
            await GunDataScreen.AddData(GameManager.Instance.currStockObj);
            await GunDataScreen.AddData(GameManager.Instance.currBarrelObj);
        }
    }
    
    private async void RandomizeAttachments()
    {
        GunSelectScreen.LoadAttachments(currentItem);
        if (GunDataScreen)
        {
            await GunDataScreen.InitializeData(GameManager.Instance.currBodyObj);
            await GunDataScreen.AddData(GameManager.Instance.currStockObj);
            await GunDataScreen.AddData(GameManager.Instance.currBarrelObj);
        }
    }

    private async void RandomizeRarity()
    {
        if (GunSelectScreen) GunSelectScreen.RandomizeRarity(currentItem);
        if (GunDataScreen)
        {
            await GunDataScreen.InitializeData(GameManager.Instance.currBodyObj);
            await GunDataScreen.AddData(GameManager.Instance.currStockObj);
            await GunDataScreen.AddData(GameManager.Instance.currBarrelObj);
        }
    }
}

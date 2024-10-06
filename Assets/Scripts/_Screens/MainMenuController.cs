using System;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

public class MainMenuController : BaseController
{
    public Button Demo;

    private void Start() => Initialize();

    protected override UniTask Initialize()
    {
        Demo.onClick.RemoveAllListeners();
        Demo.onClick.AddListener(DemoStart);
        return base.Initialize();
    }

    private async void DemoStart()
    {
        ToggleScreen(false, false);
        await GameManager.Instance.GunSelectController.LoadData();
    }
}

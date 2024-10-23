using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MyBox;
using UnityEditor;
using UnityEngine;
using Utility;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [ConditionalField(nameof(UICamera), true), SerializeField] public Camera UICamera;
    [ConditionalField(nameof(GameCamera), true), SerializeField] public Camera GameCamera;

    [Header("Data")] 
    public GunObjectData GunData;
    public GemObjectData GemData;
    public PlayerStats PlayerStats;
    public List<Tuple<DraggableObjectReceiver, List<GemColorValue>>> GemsInGame = new();
    public List<GameObject> Holes = new();
    public List<GameObject> Targets = new();

    [Header("Game Colors")] 
    public Color Black;
    public Color White;
    public Color Red;
    public Color Green;
    public Color Blue;
    public Color Yellow;

    [Header("Shared Assets")] 
    public Sprite[] BulletHoles;
    public GameObject Sparks;
    public GameObject[] Enemies;
    public GameObject[] Obstacles;

    [Header("Game States")] 
    public GameMode GameMode = GameMode.NotSelected;
    public bool GameStarted;
    public bool InGame;

    [ConditionalField(nameof(GunSelectController), true), SerializeField] public GunSelectController GunSelectController;
    [ConditionalField(nameof(SettingsController), true), SerializeField] public SettingsController SettingsController;
    [ConditionalField(nameof(MainMenuController), true), SerializeField] public MainMenuController MainMenuController;
    [ConditionalField(nameof(LoadingController), true), SerializeField] public LoadingController LoadingController;
    [ConditionalField(nameof(GameOverController), true), SerializeField] public GameOverController GameOverController;
    [ConditionalField(nameof(MapController), true), SerializeField] public MapController MapController;
    [ConditionalField(nameof(PlayerController), true), SerializeField] public Player PlayerController;
    [ConditionalField(nameof(HUDController), true), SerializeField] public HUDController HUDController;
    [ConditionalField(nameof(PoolController), true), SerializeField] public PoolController PoolController;
    
    public bool IsReady => Instance && GunData && GemData;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        GunData = Resources.Load<GunObjectData>("GunData");
        GemData = Resources.Load<GemObjectData>("GemData");
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyUp(KeyCode.P)) EditorApplication.isPaused = !EditorApplication.isPaused; 
        if (Input.GetKeyUp(KeyCode.Q)) EditorApplication.ExitPlaymode();
        if (Input.GetKeyUp(KeyCode.M)) ToggleCursorLock();
#endif
        if (Input.GetKeyUp(KeyCode.Escape)) ToggleSettingsScreen();
        GameOverCheck();
    }

    public UniTask ToggleCursorLock(bool lockPointer)
    {
        if (!GameStarted)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return UniTask.CompletedTask;
        }
        
        if (lockPointer)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        return UniTask.CompletedTask;
    }
    
    public UniTask ToggleCursorLock()
    {
        if (!GameStarted)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return UniTask.CompletedTask;;
        }
        
        if (Cursor.visible)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        return UniTask.CompletedTask;
    }

    private async void ToggleSettingsScreen()
    {
        if (!SettingsController) return;
        await SettingsController.ToggleScreen(false);
    }

    public async void StartGame()
    {
        await LoadingController.ToggleScreen(false, true);
        await GunSelectController.ToggleScreen(false, false);
        ToggleCamera(Cam.Game);
        InGame = GameStarted = true;
        PlayerController.lockMovement = true;
        PlayerStats = new PlayerStats(PlayerStats);
        await MapController.SetMap();
        await ToggleCursorLock(true);
        await HUDController.FadeCover(false);
        await GunSelectController.StartGame();
        await PlayerController.SetData();
        PlayerController.ResetState();
        await LoadingController.ToggleScreen(false, false);
    }

    public async void GameOverCheck()
    {
        if (!InGame) return;
        if (!GameStarted) return;
        if (PlayerStats.Health > 0) return;
        InGame = GameStarted = false;
        await GameOverController.ToggleScreen(false, true);
        await ToggleCursorLock(false);
    }

    public async void ToggleGamePaused(bool paused)
    {
        if (!GameStarted) return;
        InGame = !paused;
        await HUDController.FadeCover(paused);
    }

    public void ToggleCamera(Cam cam)
    {
        switch (cam)
        {
            case Cam.UI:
                UICamera.gameObject.SetActive(true);
                GameCamera.gameObject.SetActive(false);
                break;
            case Cam.Game:
                UICamera.gameObject.SetActive(false);
                GameCamera.gameObject.SetActive(true);
                break;
            case Cam.Null:
                break;
        }
    }

    #if UNITY_EDITOR
    [ButtonMethod()]
    public void SetGameCamera()
    {
        ToggleCamera(Cam.Game);
        HUDController.Cover.gameObject.SetActive(false);
        MainMenuController.CanvasGroup.alpha = 0;
    }

    [ButtonMethod()]
    public void SetDefaultCamera()
    {
        ToggleCamera(Cam.UI);
        MainMenuController.CanvasGroup.alpha = 1;
        HUDController.Cover.gameObject.SetActive(true);
    }
    
    #endif
}

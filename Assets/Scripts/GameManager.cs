using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MyBox;
using UnityEditor;
using UnityEngine;
using Debug = Utility.Debug;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [ConditionalField(nameof(UICamera), true), SerializeField] public Camera UICamera;
    [ConditionalField(nameof(GameCamera), true), SerializeField] public  Camera GameCamera;

    [Header("Data")] 
    public GunObjectData GunData;
    public GemObjectData GemData;
    public PlayerStats PlayerStats;
    public List<Tuple<DraggableObjectReceiver, List<GemColorValue>>> GemsInGame = new();
    
    [Header("Game States")]
    public bool GameStarted;
    private bool gamePaused;
    
    [ConditionalField(nameof(GunSelectController), true), SerializeField] public GunSelectController GunSelectController;
    [ConditionalField(nameof(SettingsController), true), SerializeField] public SettingsController SettingsController;
    [ConditionalField(nameof(PlayerController), true), SerializeField] public Player PlayerController;
    
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
        if (Input.GetKeyUp(KeyCode.Escape)) ToggleSettingsScreen();
#endif
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
        UICamera.gameObject.SetActive(false);
        GameCamera.gameObject.SetActive(true);
        PlayerController.lockMovement = true;
        GameStarted = true;
        await ToggleCursorLock(true);
        PlayerStats = new PlayerStats(PlayerStats);
        await GunSelectController.StartGame();
        await GunSelectController.ToggleScreen(false, false);
        PlayerController.ResetState();
    }

    public void ToggleGamePaused(bool paused)
    {
        if (!GameStarted) return;
        gamePaused = paused;
    }
}

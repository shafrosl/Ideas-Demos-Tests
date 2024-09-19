using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using MyBox;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [ConditionalField(nameof(UICamera), true), SerializeField] public Camera UICamera;
    [ConditionalField(nameof(GameCamera), true), SerializeField] public  Camera GameCamera;

    [Header("Data")] 
    public GunObjectData GunData;
    public GemObjectData GemData;
    public PlayerStats PlayerStats;
    [SerializeField, SerializedDictionary("Last Cached Slot", "Modifiers")] public SerializedDictionary<DraggableObjectReceiver, List<GemColorValue>> GemsInGame;
    
    [Header("Game States")]
    public bool GameStarted;
    private bool gamePaused;
    
    [ConditionalField(nameof(GunSelectController), true), SerializeField] public GunSelectController GunSelectController;
    [ConditionalField(nameof(SettingsController), true), SerializeField] public SettingsController SettingsController;
    
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

    public void ToggleCursorLock(bool lockPointer)
    {
        if (!GameStarted)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
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
    }
    
    public void ToggleCursorLock()
    {
        if (!GameStarted)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
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
    }

    private async void ToggleSettingsScreen()
    {
        if (!SettingsController) return;
        await SettingsController.ToggleScreen(false);
    }

    public async void StartGame()
    {
        GemsInGame.Clear();
        PlayerStats = new PlayerStats();
        await GunSelectController.StartGame();
        await GunSelectController.ToggleScreen(false, false);
        UICamera.gameObject.SetActive(false);
        GameCamera.gameObject.SetActive(true);
        ToggleCursorLock(true);
        GameStarted = true;
    }

    public void ToggleGamePaused(bool paused)
    {
        if (!GameStarted) return;
        gamePaused = paused;
    }
}

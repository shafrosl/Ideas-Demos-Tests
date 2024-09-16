using System;
using MyBox;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GunObjectData GunData;
    public GemObjectData GemData;
    [ConditionalField(nameof(GunSelectController), true), SerializeField] public GunSelectController GunSelectController;

    [Header("Player Stats")] 
    public BodyObject currBodyObj;
    public BarrelObject currBarrelObj;
    public StockObject currStockObj;
    
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
        if (Input.GetKeyUp(KeyCode.M))
        {
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
#endif
    }
}

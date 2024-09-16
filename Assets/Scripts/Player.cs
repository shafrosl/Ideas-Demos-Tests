using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyBox;
using UnityEngine;

public class Player : MonoBehaviour, IMovement, IGun
{
    public Transform Sprites;
    
    [Header("Player State")] 
    public bool isHidden;
    public bool isHiding;
    public bool isShooting;
    public bool isReloading;
    public bool isReloaded;
    
    [Header("Movement Settings")]
    [MinMaxRange(-15, 15)] public RangedFloat range;
    public float speedH = 2.0f;
    public float speedV = 2.0f;
    private float yaw, pitch;
    
    private void Update()
    {
        Look();
        Hide();
        OnShoot();
        OnReload();
    }

    public void Look()
    {
        if (isHidden) return;
        if (!isHiding)
        {
            yaw += speedH * Input.GetAxis("Mouse X");
            pitch -= speedV * Input.GetAxis("Mouse Y");
            if (pitch > range.Max) pitch = range.Max;
            else if (pitch < range.Min) pitch = range.Min;
        }
        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }

    public async void Hide()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isHiding = true;
            pitch = 12.5f;
            await Sprites.DOLocalMoveY(-700, 0.5f);
            isHiding = false;
            isHidden = true;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isReloading = false;
            isHidden = false;
            isHiding = true;
            pitch = 0;
            await Sprites.DOLocalMoveY(0, 0.5f);
            isHiding = false;
        }
    }

    public void OnShoot()
    {
        if (Input.GetMouseButtonDown(0)) isShooting = true;
        if (Input.GetMouseButtonUp(0)) isShooting = false;
    }

    public void OnReload()
    {
        if (isHidden) isReloading = true;
        if (isReloading)
        {
            
        }
        else
        {
            
        }
    }
}

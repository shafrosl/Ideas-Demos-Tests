using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyBox;
using UnityEngine;
using Debug = Utility.Debug;

public class Player : MonoBehaviour, IMovement, IGun
{
    public Transform Sprites;
    public Transform BulletExit;
    
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

    [Header("Timers")] 
    private float internalCooldownTimer;
    public float cooldownTimer;

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
        if (internalCooldownTimer > 0)
        {
            internalCooldownTimer -= Time.deltaTime;
        }
        else
        {
            if (isShooting)
            {
                internalCooldownTimer = cooldownTimer;
                Ray ray = new Ray(BulletExit.position, GameManager.Instance.GameCamera.transform.forward);
                var hit = Physics2D.GetRayIntersection(ray);
                if (hit.collider is null) return;
                if (hit.collider.CompareTag("Target"))
                {
                    if (hit.transform.TryGetComponent(out Target target))
                    {
                        target.InstantiateHole(target.transform.InverseTransformPoint(hit.point));
                    }
                }
            }
        }
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

    private void OnDrawGizmos()
    {
        Debug.DrawLine(BulletExit.position, GameManager.Instance.GameCamera.transform.forward, 100, Color.red);
    }
}

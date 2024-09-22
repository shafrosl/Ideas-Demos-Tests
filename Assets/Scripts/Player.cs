using System.Threading;
using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyBox;
using UnityEngine;
using Debug = Utility.Debug;

public class Player : MonoBehaviour, IMovement, IGun
{
    public Transform BulletExit;
    public CinemachineVirtualCamera Cinemachine;
    public CinemachineImpulseSource CinemachineImpulseSource;

    [Header("Player State")] 
    public bool isHidden;
    public bool isHiding;
    public bool isShooting;
    public bool isReloading;
    public bool isEmpty;
    public bool lockMovement;
    
    [Header("Movement Settings")]
    [MinMaxRange(-50, 50)] public RangedFloat range;
    public float speedH = 2.0f;
    public float speedV = 2.0f;
    private float yaw, pitch;
    
    private float fireRateCooldownTimer;
    private float internalFireRateCooldownTimer;
    private float recoilControl;
    private float numOfBulletsCurrent;
    private float numOfBulletsTotal;
    
    private CancellationTokenSource ctx = new();
    
    public void ResetState() => isHidden = isHiding = isShooting = isReloading = lockMovement = false;
    private void CheckMagazine() => isEmpty = numOfBulletsCurrent <= 0;

    public void SetData()
    {
        foreach (var stat in GameManager.Instance.PlayerStats.GunStats)
        {
            switch (stat.Key)
            {
                case Modifier.FireRate:
                    fireRateCooldownTimer = 1 / (float)stat.Value;
                    break;
                case Modifier.RecoilControl: 
                    recoilControl = 1 / (float)stat.Value;
                    break;
                case Modifier.MagazineSize:
                    numOfBulletsCurrent = numOfBulletsTotal = 5 * stat.Value;
                    break;
            }
        }
    }
    
    private void Update()
    {
        if (lockMovement) return;
        Look();
        Hide();
        OnShoot();
        OnReload();
        CheckMagazine();
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
        Cinemachine.transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }

    public async void Hide()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isShooting = false;
            if (isHiding)
            {
                ctx.Cancel();
                ctx.Dispose();
                ctx = new();
            }
            isHiding = true;
            
            DOTween.To(() => pitch, x => pitch = x, 20f, 0.25f).WithCancellation(ctx.Token).SuppressCancellationThrow();
            await transform.DOLocalMoveY(-0.2f, 0.5f).WithCancellation(ctx.Token).SuppressCancellationThrow();
            isHiding = false;
            isHidden = true;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isShooting = false;
            if (isHiding)
            {
                ctx.Cancel();
                ctx.Dispose();
                ctx = new();
            }
            isReloading = false;
            isHidden = false;
            isHiding = true;
            
            DOTween.To(() => pitch, x => pitch = x, 0f, 0.25f).WithCancellation(ctx.Token).SuppressCancellationThrow();
            await transform.DOLocalMoveY(0, 0.5f).WithCancellation(ctx.Token).SuppressCancellationThrow();
            isHiding = false;
        }
    }

    public void OnShoot()
    {
        if (Input.GetMouseButtonDown(0)) isShooting = true;
        if (isEmpty)
        {
            isShooting = false;
            return;
        }
        
        if (internalFireRateCooldownTimer > 0)
        {
            internalFireRateCooldownTimer -= Time.deltaTime;
        }
        else
        {
            if (isShooting)
            {
                internalFireRateCooldownTimer = fireRateCooldownTimer;
                CinemachineImpulseSource.GenerateImpulseWithForce(recoilControl);
                numOfBulletsCurrent--;
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
            // await sfx here then...
            numOfBulletsCurrent = numOfBulletsTotal;
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Debug.DrawLine(BulletExit.position, GameManager.Instance.GameCamera.transform.forward, 100, Color.red);
        }
    }
}

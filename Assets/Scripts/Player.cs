using System;
using System.Linq;
using System.Threading;
using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyBox;
using PathCreation;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour, IMovement, IGun
{
    public Transform BulletExit;
    public Collider Collider;
    public ParticleSystem MuzzleFlash;
    public CinemachineVirtualCamera Cinemachine;
    public CinemachineImpulseSource RecoilImpulse;
    public CinemachineImpulseSource HitImpulse;
    public CinemachineImpulseSource ExplosionImpulse;

    [Header("Sprites")] 
    public Transform PlayerSpriteGroup;
    public SpriteRenderer LeftHand;
    public SpriteRenderer RightHand;
    public SpriteRenderer[] DeathOverlay;

    [Header("Player State")] 
    public bool isMoving;
    public bool isHidden;
    public bool isHiding;
    public bool isShooting;
    public bool isReloading;
    public bool isEmpty;
    public bool isHit;
    public bool lockMovement;
    
    [Header("Movement Settings")]
    [MinMaxRange(-50, 50)] public RangedFloat range;
    public float speedH = 2.0f;
    public float speedV = 2.0f;
    public float yaw, pitch;
    public float movementSpeed;
    public EndOfPathInstruction endOfPathInstruction;

    private float internalProtectionCooldownTimer;
    private int damage;
    private float fireRateCooldownTimer;
    private float internalFireRateCooldownTimer;
    private float recoilControl;
    private int numOfBulletsCurrent;
    private int numOfBulletsTotal;
    private float distanceTravelled;

    public Vector3 TargetPos;

    public int Damage => damage;
    
    private CancellationTokenSource ctx = new();
    
    public void ResetState() => isHidden = isHiding = isShooting = isReloading = lockMovement = false;
    private void CheckMagazine() => isEmpty = numOfBulletsCurrent <= 0;
    private void OnPathChanged() => distanceTravelled = GameManager.Instance.MapController.PathCreator.path.GetClosestDistanceAlongPath(transform.position);

    public async UniTask<UniTask> SetData(int hearts = 5)
    {
        foreach (var stat in GameManager.Instance.PlayerStats.GunStats)
        {
            switch (stat.Key)
            {
                case Modifier.FireRate:
                    fireRateCooldownTimer = 1 / (float)stat.Value;
                    break;
                case Modifier.RecoilControl: 
                    recoilControl = (1 / (float)stat.Value) * 2;
                    break;
                case Modifier.MagazineSize:
                    numOfBulletsCurrent = numOfBulletsTotal = 5 * stat.Value;
                    break;
                case Modifier.Damage:
                    damage = stat.Value;
                    break;
            }
        }
        
        GameManager.Instance.HUDController.SetTotal(numOfBulletsTotal);
        GameManager.Instance.HUDController.SetCurrent(numOfBulletsTotal);
        await ResetOverlay();
        await GameManager.Instance.HUDController.ResetHearts(hearts);
        return UniTask.CompletedTask;
    }

    private void Start()
    {
        if (GameManager.Instance.MapController.PathCreator != null)
        {
            // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
            GameManager.Instance.MapController.PathCreator.pathUpdated += OnPathChanged;
        }
    }

    private void Update()
    {
        if (lockMovement) return;
        Look();
        Hide();
        Move();
        OnShoot();
        OnReload();
        OnHit();
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
        Cinemachine.transform.rotation = Quaternion.Euler(pitch, yaw, 0.0f);
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
            
            DOTween.To(() => pitch, x => pitch = x, 40f, 0.25f).WithCancellation(ctx.Token).SuppressCancellationThrow();
            await PlayerSpriteGroup.DOLocalMoveY(-0.2f, 0.5f).WithCancellation(ctx.Token).SuppressCancellationThrow();
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
            await PlayerSpriteGroup.DOLocalMoveY(0, 0.5f).WithCancellation(ctx.Token).SuppressCancellationThrow();
            isHiding = false;
        }
    }

    [ButtonMethod()]
    public void Move()
    {
        isMoving = true;
        var pc = GameManager.Instance.MapController.PathCreator;
        if (pc != null)
        {
            distanceTravelled += movementSpeed * Time.deltaTime;
            Cinemachine.transform.position = pc.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
        }
        
        if (distanceTravelled >= pc.path.length) isMoving = false;
    }
    
    public void OnShoot()
    {
        if (Input.GetMouseButtonDown(0)) isShooting = true;
        if (isEmpty)
        {
            if (!isShooting) return;
            AnimateShootingHands();
            GameManager.Instance.HUDController.OutOfBullets();
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
                var results = RayExtensions.GetRaycastHits3D(BulletExit.position, GameManager.Instance.GameCamera.transform.forward, out var size); 
                if (size > 0)
                {
                    foreach (var result in results)
                    {
                        if (result.collider is null) continue;
                        if (!result.collider.CompareTag("Target") && !result.collider.CompareTag("StaticTarget")) continue;
                        if (!result.collider.transform.parent.TryGetComponent(out BaseTarget target)) continue;
                        if (result.collider.transform.parent.TryGetComponent<SpriteRenderer>(out var SR))
                        {
                            
                            target.InstantiateHole(result.point, SR, result.normal);
                        }
                        else
                        {
                            target.InstantiateHole(result.point, null, result.normal);
                        }
                    
                        break;
                    }
                }
                
                AnimateShootingHands();
                RecoilImpulse.GenerateImpulseWithForce(recoilControl);
                MuzzleFlash.Play();
                numOfBulletsCurrent--;
                GameManager.Instance.PlayerStats.Score.TotalShots++;
                GameManager.Instance.HUDController.SetCurrent(numOfBulletsCurrent);
                GameManager.Instance.HUDController.AnimateShot();
                DOTween.To(() => pitch, x => pitch = x, pitch - recoilControl, 0.1f).WithCancellation(ctx.Token).SuppressCancellationThrow();
            }
        }
        if (Input.GetMouseButtonUp(0)) isShooting = false;
    }

    public async void OnReload()
    {
        if (isHidden) isReloading = true;
        if (isReloading)
        {
            // await sfx here then...
            numOfBulletsCurrent = numOfBulletsTotal;
            GameManager.Instance.HUDController.SetCurrent(numOfBulletsCurrent);
        }
    }

    public async void OnHit()
    {
        if (internalProtectionCooldownTimer > 0) internalProtectionCooldownTimer -= Time.deltaTime;
        if (!isHit) return;
        isHit = false;
        if (!isHidden && internalProtectionCooldownTimer <= 0)
        {
            if (GameManager.Instance.PlayerStats.Damage())
            {
                // play hit sfx
                internalProtectionCooldownTimer = 6;
                HitImpulse.GenerateImpulseWithForce(0.15f);
                GameManager.Instance.HUDController.Hit();
                await OverlaySelector(true);
            }
        }
        else
        {
            // play close miss sfx
        }
    }

    public async void OnHeal()
    {
        if (GameManager.Instance.PlayerStats.Heal())
        {
            GameManager.Instance.HUDController.Heal();
            await OverlaySelector(false);
        }
    }

    private UniTask ResetOverlay()
    {
        foreach (var overlay in DeathOverlay)
        {
            overlay.color = overlay.color.Modify(a: 0);
            overlay.gameObject.SetActive(false);
        }
        
        return UniTask.CompletedTask;
    }

    private async UniTask<UniTask> OverlaySelector(bool show)
    {
        var results = (DeathOverlay.Where(SR => show ? !SR.gameObject.activeSelf : SR.gameObject.activeSelf)).ToArray();
        var i = Random.Range(0, results.Length);
        for (var tries = 0; i < 20; i++)
        {
            if (show ? results[i].gameObject.activeSelf : !results[i].gameObject.activeSelf) i = Random.Range(0, results.Length);
            else
            {
                results[i].gameObject.SetActive(show);
                await results[i].DOFade(show ? 0.75f : 0, 0.2f);
                break;
            }
        }
        return UniTask.CompletedTask;
    }

    private async void AnimateShootingHands()
    {
        await DOTween.To(() => RightHand.transform.localEulerAngles, x => RightHand.transform.localEulerAngles = x, new Vector3(0, 0, isEmpty ? 1f : 2f), 0.1f).WithCancellation(ctx.Token).SuppressCancellationThrow();
        await DOTween.To(() => RightHand.transform.localEulerAngles, x => RightHand.transform.localEulerAngles = x, Vector3.zero, 0.1f).WithCancellation(ctx.Token).SuppressCancellationThrow();
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            DebugExtensions.DrawLine(BulletExit.position, GameManager.Instance.GameCamera.transform.forward, 100, Color.red);
        }
    }
}

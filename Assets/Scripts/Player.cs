using System.Collections.Generic;
using System.Linq;
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
    public ParticleSystem MuzzleFlash;
    public CinemachineVirtualCamera Cinemachine;
    public CinemachineImpulseSource RecoilImpulse;
    public CinemachineImpulseSource HitImpulse;

    [Header("Sprites")] 
    public Transform PlayerSpriteGroup;
    public SpriteRenderer LeftHand;
    public SpriteRenderer RightHand;
    public SpriteRenderer[] DeathOverlay;

    [Header("Player State")] 
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
    private float yaw, pitch;

    private int damage;
    private float fireRateCooldownTimer;
    private float internalFireRateCooldownTimer;
    private float recoilControl;
    private int numOfBulletsCurrent;
    private int numOfBulletsTotal;

    public int Damage => damage;
    
    private CancellationTokenSource ctx = new();
    
    public void ResetState() => isHidden = isHiding = isShooting = isReloading = lockMovement = false;
    private void CheckMagazine() => isEmpty = numOfBulletsCurrent <= 0;

    public UniTask SetData()
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
                case Modifier.Damage:
                    damage = stat.Value;
                    break;
            }
        }
        
        GameManager.Instance.HUDController.SetTotal(numOfBulletsTotal);
        GameManager.Instance.HUDController.SetCurrent(numOfBulletsTotal);
        return UniTask.CompletedTask;
    }
    
    private void Update()
    {
        if (lockMovement) return;
        Look();
        Hide();
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
                var ray = new Ray(BulletExit.position, GameManager.Instance.GameCamera.transform.forward);
                var results = new RaycastHit[16];
                var size = Physics.RaycastNonAlloc(ray, results);
                System.Array.Sort(results, (a, b) => (a.distance.CompareTo(b.distance)));
                if (size < 1) return;
                foreach (var result in results)
                {
                    if (result.collider is null) continue;
                    if (!result.collider.CompareTag("Target")) continue;
                    if (!result.collider.transform.parent.TryGetComponent(out Target target)) continue;
                    target.InstantiateHole(result.transform.InverseTransformPoint(result.point), (result.point + target.transform.parent.position), result.collider.transform.parent.GetComponent<SpriteRenderer>(), target.transform.localPosition);
                    break;
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
        if (!isHit) return;
        isHit = false;
        if (!isHidden)
        {
            if (GameManager.Instance.PlayerStats.Damage())
            {
                // play hit sfx
                HitImpulse.GenerateImpulseWithForce(0.15f);
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
        if (GameManager.Instance.PlayerStats.Heal()) await OverlaySelector(false);
    }

    public async UniTask<UniTask> OverlaySelector(bool show)
    {
        var results = (DeathOverlay.Where(SR => show ? !SR.gameObject.activeSelf : SR.gameObject.activeSelf)).ToArray();
        var i = Random.Range(0, results.Length);
        for (var tries = 0; i < 20; i++)
        {
            if (show ? results[i].gameObject.activeSelf : !results[i].gameObject.activeSelf) i = Random.Range(0, results.Length);
            else
            {
                results[i].gameObject.SetActive(show);
                await results[i].DOFade(show ? 1 : 0, 0.2f);
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
            Debug.DrawLine(BulletExit.position, GameManager.Instance.GameCamera.transform.forward, 100, Color.red);
        }
    }
}

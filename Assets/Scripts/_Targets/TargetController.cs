using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Debug = Utility.Debug;
using Random = UnityEngine.Random;

public class TargetController : MonoBehaviour
{
    public Transform BulletExit;
    public Rigidbody Rigidbody;
    public ParticleSystem MuzzleFlash;

    [Header("States")] 
    public bool isMoving;

    [Header("Settings")] 
    [Range(0.0f, 1.0f)] public float Chance;
    public float DistanceToShoot;
    public float ShootCountdownTimer;
    private float internalShootCountdownTimer;
    
    [Header("Pop Ups")]
    public TextPopUp HeadShot;
    public TextPopUp BodyShot;
    
    private bool inShootingRange => Vector3.Distance(transform.position, GameManager.Instance.PlayerController.transform.position) < DistanceToShoot;


    private void Start()
    {
        MovePingPongX(new Vector2(-10, 10), speedB: 8);
    }

    private void Update()
    {
        if (internalShootCountdownTimer > 0) internalShootCountdownTimer -= Time.deltaTime;
        else
        {
            Shoot();
            internalShootCountdownTimer = ShootCountdownTimer;
        }
    }

    private void Shoot()
    {
        if (!inShootingRange) return;
        MuzzleFlash.Play();
        // play shoot sfx
        var hit = Random.Range(0.0f, 1.0f);
        if (hit > Chance)
        {
            // play miss sfx
        }
        else
        {
            GameManager.Instance.PlayerController.isHit = true;
        }
    }

    private async UniTask<UniTask> MoveLinearX(float xDir, float speed = 2)
    {
        await transform.parent.DOMoveX(xDir, speed);
        return UniTask.CompletedTask;
    }
    
    private async UniTask<UniTask> MoveLinearZ(float xDir, float speed = 2)
    {
        await transform.parent.DOMoveZ(xDir, speed);
        return UniTask.CompletedTask;
    }

    private async void MovePingPongX(Vector2 xDirs, float? speedA = null, float? speedB = null, int? delayMs = null)
    {
        await MoveLinearX(xDirs.x, speedA ?? 2);
        await UniTask.Delay(delayMs ?? 0);
        await MoveLinearX(xDirs.y, speedB ?? 2);
        MovePingPongX(xDirs, speedA, speedB);
    }
    
    private async void MovePingPongZ(Vector2 xDirs, float? speedA = null, float? speedB = null, int? delayMs = null)
    {
        await MoveLinearZ(xDirs.x, speedA ?? 2);
        await UniTask.Delay(delayMs ?? 0);
        await MoveLinearZ(xDirs.y, speedB ?? 2);
        MovePingPongZ(xDirs, speedA, speedB);
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Debug.DrawLine(BulletExit.position, GameManager.Instance.PlayerController.transform.position - transform.position, DistanceToShoot, Color.blue);
        }
    }
}

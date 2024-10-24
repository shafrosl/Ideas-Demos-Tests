using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyBox;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

[RequireComponent(typeof(HingeJoint))]
public class TargetControllerEnemy : BaseTargetController
{
    public Transform BulletExit;
    public ParticleSystem MuzzleFlash;

    [Header("Settings")] 
    public bool IsPingPong;
    [ConditionalField(nameof(IsPingPong)), ConstantsSelection(typeof(Axis))] public Axis PingPongAxis;
    [ConditionalField(nameof(IsPingPong))] public Vector2 PingPongValues;
    [ConditionalField(nameof(IsPingPong))] public Vector2 PingPongSpeed;
    [ConditionalField(nameof(IsPingPong))] public int PingPongDelayBetween;

    [Range(0.0f, 1.0f)] public float Chance;
    public float DistanceToShoot;
    public float ShootCountdownTimer;
    
    [HideInInspector] public TextPopUp HeadShot;
    [HideInInspector] public TextPopUp BodyShot;
    
    private bool inShootingRange => Distance < DistanceToShoot;
    
    protected override UniTask Initialize()
    {
        if (isInitialized) return UniTask.CompletedTask;
        internalCountdown = Random.Range(ShootCountdownTimer/2, ShootCountdownTimer*2);
        PingPongAtStart();
        GameManager.Instance.Targets.Add(transform.parent.gameObject);
        return base.Initialize();
    }
    
    protected override async UniTask<UniTask> ControllerUpdate()
    {
        await base.ControllerUpdate();
        if (!GameManager.Instance.GameStarted) return UniTask.CompletedTask;
        if (internalCountdown > 0) internalCountdown -= Time.deltaTime;
        else
        {
            Shoot();
            internalCountdown = Random.Range(1.0f, ShootCountdownTimer);
        }
        
        return UniTask.CompletedTask;
    }
    
    private void Shoot()
    {
        var gm = GameManager.Instance;
        if (!gm.GameStarted) return;
        if (gm.GameMode == GameMode.GunRange) return;
        if (!inShootingRange) return;
        MuzzleFlash.transform.localEulerAngles = PositionDotValue < 0 ? new Vector3(0, 180, 0) : new Vector3(0, 0, 0);
        MuzzleFlash.Play();
        // play shoot sfx
        var hit = (Random.Range(0.0f, 0.01f)) * Distance;
        if (hit > Chance)
        {
            // play miss sfx
        }
        else
        {
            var hitInfo = RayExtensions.GetRaycastHit3D(BulletExit.position, gm.PlayerController.Collider.transform.position - BulletExit.position);
            if (hitInfo.collider is null) return;
            if (hitInfo.collider != gm.PlayerController.Collider) return;
            gm.PlayerController.isHit = true;
        }
    }

    private async UniTask<UniTask> MoveLinearX(float xDir, float speed = 2)
    {
        await transform.parent.DOLocalMoveX(xDir, speed).WithCancellation(ctx.Token).SuppressCancellationThrow();
        return UniTask.CompletedTask;
    }
    
    private async UniTask<UniTask> MoveLinearZ(float xDir, float speed = 2)
    {
        await transform.parent.DOLocalMoveZ(xDir, speed).WithCancellation(ctx.Token).SuppressCancellationThrow();
        return UniTask.CompletedTask;
    }

    private async void MovePingPongX(Vector2 xDirs, float? speedA = null, float? speedB = null, int? delayMs = null)
    {
        if (!GameManager.Instance.GameStarted) return;
        await MoveLinearX(xDirs.x, speedA ?? 2);
        if (!GameManager.Instance.GameStarted) return;
        await UniTask.Delay(delayMs ?? 0);
        if (!GameManager.Instance.GameStarted) return;
        await MoveLinearX(xDirs.y, speedB ?? 2);
        if (!GameManager.Instance.GameStarted) return;
        MovePingPongX(xDirs, speedA, speedB);
    }
    
    private async void MovePingPongZ(Vector2 xDirs, float? speedA = null, float? speedB = null, int? delayMs = null)
    {
        if (!GameManager.Instance.GameStarted) return;
        await MoveLinearZ(xDirs.x, speedA ?? 2);
        if (!GameManager.Instance.GameStarted) return;
        await UniTask.Delay(delayMs ?? 0);
        if (!GameManager.Instance.GameStarted) return;
        await MoveLinearZ(xDirs.y, speedB ?? 2);
        if (!GameManager.Instance.GameStarted) return;
        MovePingPongZ(xDirs, speedA, speedB);
    }
    
    private async void MovePingPongXZ(Vector2 xDirs, float? speedA = null, float? speedB = null, int? delayMs = null)
    {
        if (!GameManager.Instance.GameStarted) return;
        await MoveLinearX(xDirs.x, speedA ?? 2);
        if (!GameManager.Instance.GameStarted) return;
        await UniTask.Delay(delayMs ?? 0);
        if (!GameManager.Instance.GameStarted) return;
        await MoveLinearZ(xDirs.y, speedB ?? 2);
        if (!GameManager.Instance.GameStarted) return;
        await UniTask.Delay(delayMs ?? 0);
        if (!GameManager.Instance.GameStarted) return;
        await MoveLinearZ(-xDirs.y, speedB ?? 2);
        if (!GameManager.Instance.GameStarted) return;
        await UniTask.Delay(delayMs ?? 0);
        if (!GameManager.Instance.GameStarted) return;
        await MoveLinearX(-xDirs.x, speedA ?? 2);
        if (!GameManager.Instance.GameStarted) return;
        MovePingPongXZ(xDirs, speedA, speedB);
    }
    
    private async void MovePingPongZX(Vector2 xDirs, float? speedA = null, float? speedB = null, int? delayMs = null)
    {
        if (!GameManager.Instance.GameStarted) return;
        await MoveLinearZ(xDirs.x, speedA ?? 2);
        if (!GameManager.Instance.GameStarted) return;
        await UniTask.Delay(delayMs ?? 0);
        if (!GameManager.Instance.GameStarted) return;
        await MoveLinearX(xDirs.y, speedB ?? 2);
        if (!GameManager.Instance.GameStarted) return;
        await UniTask.Delay(delayMs ?? 0);
        if (!GameManager.Instance.GameStarted) return;
        await MoveLinearX(-xDirs.y, speedB ?? 2);
        if (!GameManager.Instance.GameStarted) return;
        await UniTask.Delay(delayMs ?? 0);
        if (!GameManager.Instance.GameStarted) return;
        await MoveLinearZ(-xDirs.x, speedA ?? 2);
        if (!GameManager.Instance.GameStarted) return;
        MovePingPongXZ(xDirs, speedA, speedB);
    }

    private void PingPongAtStart()
    {
        if (!GameManager.Instance.GameStarted) return;
        if (!IsPingPong) return;
        switch (PingPongAxis)
        {
            case Axis.X:
                MovePingPongX(PingPongValues, PingPongSpeed.x == 0 ? null : PingPongSpeed.x, PingPongSpeed.y == 0 ? null : PingPongSpeed.y, PingPongDelayBetween);
                break;
            case Axis.Z:
                MovePingPongZ(PingPongValues, PingPongSpeed.x == 0 ? null : PingPongSpeed.x, PingPongSpeed.y == 0 ? null : PingPongSpeed.y, PingPongDelayBetween);
                break;
            case Axis.XZ:
                MovePingPongXZ(PingPongValues, PingPongSpeed.x == 0 ? null : PingPongSpeed.x, PingPongSpeed.y == 0 ? null : PingPongSpeed.y, PingPongDelayBetween);
                break;
            case Axis.ZX:
                MovePingPongZX(PingPongValues, PingPongSpeed.x == 0 ? null : PingPongSpeed.x, PingPongSpeed.y == 0 ? null : PingPongSpeed.y, PingPongDelayBetween);
                break;
        }
    }
}

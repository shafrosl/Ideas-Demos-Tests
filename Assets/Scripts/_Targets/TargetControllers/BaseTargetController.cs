using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class BaseTargetController : MonoBehaviour
{
    public Rigidbody Rigidbody;
    protected float Distance => Vector3.Distance(transform.position, GameManager.Instance.PlayerController.transform.position);
    public float PositionDotValue => Vector3.Dot((GameManager.Instance.PlayerController.transform.position - transform.position), transform.forward);

    protected bool isInitialized;
    protected float internalCountdown;
    protected CancellationTokenSource ctx = new();
    
    private async void Update() => await ControllerUpdate();

    protected virtual async UniTask<UniTask> ControllerUpdate()
    {
        if (!GameManager.Instance.GameStarted) isInitialized = false;
        else await Initialize();
        return UniTask.CompletedTask;
    }

    protected virtual UniTask Initialize()
    {
        if (!GameManager.Instance.GameStarted) return UniTask.CompletedTask;
        isInitialized = true;
        return UniTask.CompletedTask;
    }

    public virtual void SpecialisedTask() { }
}

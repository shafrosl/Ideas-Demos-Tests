using System.Threading;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HingeJoint))]
public abstract class BaseTargetController : MonoBehaviour
{
    public Rigidbody Rigidbody;
    protected float Distance => Vector3.Distance(transform.position, GameManager.Instance.PlayerController.transform.position);
    public float PositionDotValue => Vector3.Dot((GameManager.Instance.PlayerController.transform.position - transform.position), transform.forward);

    protected float internalCountdown;
    protected CancellationTokenSource ctx = new();

    private void Update() => ControllerUpdate();
    protected virtual void ControllerUpdate() { }
}

using MyBox;
using UnityEngine;
using UnityEngine.UI.Extensions;
using Debug = Utility.Debug;

public class TargetController : MonoBehaviour
{
    public Transform BulletExit;
    public Rigidbody Rigidbody;
    public ParticleSystem MuzzleFlash;

    [Header("Settings")] 
    [Range(0.0f, 1.0f)] public float Chance;
    public float DistanceToShoot;
    public float ShootCountdownTimer;
    private float internalShootCountdownTimer;
    
    [Header("Pop Ups")]
    public TextPopUp HeadShot;
    public TextPopUp BodyShot;
    
    private bool inShootingRange => Vector3.Distance(transform.position, GameManager.Instance.PlayerController.transform.position) < DistanceToShoot;

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

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Debug.DrawLine(BulletExit.position, GameManager.Instance.PlayerController.transform.position - transform.position, DistanceToShoot, Color.blue);
        }
    }
}

using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private void OnEnable() => transform.LookAt(GameManager.Instance.GameCamera.transform);
}

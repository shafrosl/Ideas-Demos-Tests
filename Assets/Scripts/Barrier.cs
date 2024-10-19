using MyBox;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    public Animator BarrierAnimController;

    [ButtonMethod()] public void TriggerOpen() => BarrierAnimController.SetBool("isOpen", true);
    [ButtonMethod()] public void TriggerClose() => BarrierAnimController.SetBool("isOpen", false);
}

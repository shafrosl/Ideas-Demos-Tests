using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Debug = Utility.Debug;

public class GunGemSlot : GemSlot, IPointerEnterHandler, IPointerExitHandler
{
    private float cooldownTimer = 0.25f;
    private float internalCooldownTimer;
    public bool updateData;

    public override async void OnDrop(PointerEventData eventData)
    {
        base.OnDrop(eventData);
        if (eventData.pointerDrag is null) return;
        if (eventData.pointerDrag.TryGetComponent(out GemObj gem))
        {
            var dataScreen = GameManager.Instance.GunSelectController.GunDataScreen;
            await dataScreen.AddData(gem);
            await dataScreen.RemoveTempData(gem);
        }
    }

    public async void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag is null) return;
        if (eventData.pointerDrag.TryGetComponent(out GemObj gem))
        {
            var dataScreen = GameManager.Instance.GunSelectController.GunDataScreen;
            if (updateData) await dataScreen.AddTempData(gem);
        }
        await UniTask.Delay(10);
    }
    
    public async void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag is null) return;
        if (eventData.pointerDrag.TryGetComponent(out GemObj gem))
        {
            var dataScreen = GameManager.Instance.GunSelectController.GunDataScreen;
            await dataScreen.RemoveTempData(gem);
        }
        await UniTask.Delay(10);
    }

    public override void DORUpdate()
    {
        if (!updateData)
        {
            internalCooldownTimer += Time.deltaTime;
            if (!(internalCooldownTimer > cooldownTimer)) return;
            internalCooldownTimer = 0;
            updateData = true;
        }
    }
}

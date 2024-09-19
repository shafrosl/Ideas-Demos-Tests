using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableObjectReceiver : MonoBehaviour, IDropHandler
{
    public void Update() => DORUpdate();
    public virtual void DORUpdate() { }
    public virtual void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag is null) return;
        if (eventData.pointerDrag.TryGetComponent(out GemObject gem))
        {
            gem.transform.position = transform.position;
        }
    }
}
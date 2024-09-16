using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableObject : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public Guid ObjID;
    [SerializeField] public RectTransform rectTransform;
    [SerializeField] protected CanvasGroup canvasGroup;
    
    public virtual void Initialize()
    {
        if (TryGetComponent(out RectTransform rt)) rectTransform = rt;
        if (TryGetComponent(out CanvasGroup cg)) canvasGroup = cg;
        ObjID = Guid.NewGuid();
    }

    public virtual void OnPointerDown(PointerEventData eventData) { }

    public virtual void OnBeginDrag(PointerEventData eventData) { }

    public virtual void OnEndDrag(PointerEventData eventData) { }

    public virtual void OnDrag(PointerEventData eventData)
    {
         rectTransform.anchoredPosition += eventData.delta / GameManager.Instance.GunSelectController.Canvas.scaleFactor;
    }
}

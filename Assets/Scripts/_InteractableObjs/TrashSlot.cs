using UnityEngine.EventSystems;

public class TrashSlot : GemSlot
{
    public override void OnDrop(PointerEventData eventData)
    {
        base.OnDrop(eventData);
        if (eventData.pointerDrag is null) return;
        if (eventData.pointerDrag.TryGetComponent(out GemObject gem))
        {
            var gemsGeS = GameManager.Instance.GunSelectController.GemScreen.Gems;
            var found = gemsGeS.FindIndex(x => x.ObjID == gem.ObjID);
            if (found > -1) gemsGeS.RemoveAt(found);
            Destroy(gem.gameObject);
        }
    }
}

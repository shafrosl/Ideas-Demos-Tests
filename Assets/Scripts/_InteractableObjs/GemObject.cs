using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utility;
using Debug = Utility.Debug;

public class GemObject : DraggableObject
{
    public DraggableObjectReceiver lastCachedGemSlot;
    public List<GemColorValue> Mods = new();
    private Vector3 originalPos;

    public override async void Initialize()
    {
        base.Initialize();
        Mods = await GameManager.Instance.GemData.GetGem();
        var children = Transforms.GetAllChildrenInTransform(transform, out _);

        var j = 0;
        for (var i = 0; i < 5; i++)
        {
            children[i].GetComponent<Image>().color = Mods[j].Color;
            if (j == Mods.Count - 1) j = 0;
            else j++;
        }
    }

    public UniTask RestoreGem(List<GemColorValue> mods)
    {
        if (!mods.IsSafe()) return UniTask.CompletedTask;
        base.Initialize();
        Mods = mods;
        var children = Transforms.GetAllChildrenInTransform(transform, out _);
        var j = 0;
        for (var i = 0; i < 5; i++)
        {
            children[i].GetComponent<Image>().color = Mods[j].Color;
            if (j == Mods.Count - 1) j = 0;
            else j++;
        }
        
        return UniTask.CompletedTask;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        originalPos = transform.position;
    }

    public override async void OnBeginDrag(PointerEventData eventData)
    {
        if (!canvasGroup) return;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.9f;
        
        var results = Physics2D.OverlapCircleAll(transform.position, 10);
        for (var i  = 0; i < results.Length; i++) if (results[i].gameObject == gameObject) results = results.Where((val, idx) => idx != i).ToArray();
        if (results.Length > 0)
        {
            var gemsGuS = GameManager.Instance.GunSelectController.GunSelectScreen.Gems;
            for (var i = 0; i < results.Length; i++)
            {
                if (results[i].gameObject.CompareTag("Slot") && results[i].gameObject.TryGetComponent(out GunGemSlot ggslot))
                {
                    ggslot.updateData = false;
                    transform.localScale = VectorExtensions.CreateV3(0.8f);
                    var dataScreen = GameManager.Instance.GunSelectController.GunDataScreen;
                    await dataScreen.RemoveData(this);
                    var found = gemsGuS.FindIndex(x => x.ObjID == ObjID);
                    if (found > -1) gemsGuS.RemoveAt(found);
                }
                else if (results[i].gameObject.CompareTag("Slot") && results[i].gameObject.TryGetComponent(out GemSlot gSlot))
                {
                    transform.localScale = VectorExtensions.CreateV3(0.8f);
                }
            }
        }
        
        base.OnBeginDrag(eventData);
    }
    
    public override async void OnEndDrag(PointerEventData eventData)
    {
        if (!canvasGroup) return;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        var badTouch = false;
        var results = Physics2D.OverlapCircleAll(transform.position, 10);
        for (var i  = 0; i < results.Length; i++) if (results[i].gameObject == gameObject) results = results.Where((val, idx) => idx != i).ToArray();
        if (results.Length > 0)
        {
            for (var i = 0; i < results.Length; i++)
            {
                if (results[i].gameObject.CompareTag("Gem")) badTouch = true;
                if (!results[i].gameObject.CompareTag("Slot")) badTouch = true;
                if (badTouch) break;
            }

            if (badTouch) transform.position = originalPos;

            var gemsGeS = GameManager.Instance.GunSelectController.GemScreen.Gems;
            var gemsGuS = GameManager.Instance.GunSelectController.GunSelectScreen.Gems;
            for (var i = 0; i < results.Length; i++)
            {
                if (results[i].gameObject.CompareTag("Slot") && results[i].gameObject.TryGetComponent(out GunGemSlot ggSlot))
                {
                    transform.localScale = VectorExtensions.CreateV3(0.8f);
                    if (lastCachedGemSlot is GunGemSlot && lastCachedGemSlot != ggSlot)
                    {
                        gemsGuS.Add(this);
                    }
                    else
                    {
                        var found = gemsGeS.FindIndex(x => x.ObjID == ObjID);
                        if (found > -1)
                        {
                            gemsGeS.RemoveAt(found);
                            gemsGuS.Add(this);
                        }
                    }
                    
                    lastCachedGemSlot = ggSlot;
                }
                else if (results[i].gameObject.CompareTag("Slot") && results[i].gameObject.TryGetComponent(out GemSlot gSlot))
                {
                    transform.localScale = VectorExtensions.CreateV3(1.65f);
                    if (!gemsGeS.Find(x => x.ObjID == ObjID)) gemsGeS.Add(this);
                    lastCachedGemSlot = gSlot;
                }
            }
        }
        else
        {
            transform.position = originalPos;
            if (lastCachedGemSlot is GunGemSlot)
            {
                var gemsGuS = GameManager.Instance.GunSelectController.GunSelectScreen.Gems;
                transform.localScale = VectorExtensions.CreateV3(0.8f);
                await GameManager.Instance.GunSelectController.GunDataScreen.AddData(this);
                gemsGuS.Add(this);
            }
            else if (lastCachedGemSlot is GemSlot)
            {
                transform.localScale = VectorExtensions.CreateV3(1.65f);
            }
        }
    }
}

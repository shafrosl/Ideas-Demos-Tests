using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.UI;
using Utility;

[CreateAssetMenu(fileName = "GunPart", menuName = "CreateItems/GunPart", order = 1)]
public class GunPartItem : BaseItem
{
    [SerializeField] protected Vector3 GemSlotPosition;

    [Space(10), Header("Attributes")] 
    [SerializeField] public List<GunModifierValue> Stats;
    
    protected bool slotHelperBool, partHelperBool;
    protected GameObject tempSlot, tempItem;

    protected virtual void PartHelperToggle()
    {
        if (tempSlot) return;
        if (partHelperBool)
        {
            var GunData = Resources.Load<GunObjectData>("GunData");
            tempItem = new GameObject(ItemName)
            {
                transform =
                {
                    parent = gameObject.transform,
                    localPosition = Vector3.zero,
                    localScale = Vector3.one
                }
            };

            var objI = tempItem.AddComponent<Image>();
            UpdateSprite(objI);
        }
        else
        {
            if (tempItem) DestroyImmediate(tempItem);
        }

        partHelperBool = !partHelperBool;
    }
    
    [ButtonMethod]
    public void SlotHelperToggle()
    {
        if (slotHelperBool)
        {
            var GunData = Resources.Load<GunObjectData>("GunData");
            PartHelperToggle();
            if (!tempItem) return;
            tempSlot = new GameObject("Slot Pos")
                {
                    transform =
                    {
                        parent = tempItem.transform,
                        localPosition = GemSlotPosition,
                        localScale = Vector3.one
                    }
                };
            
            var objS = tempSlot.AddComponent<Image>();
            objS.sprite = GunData.SlotIcon;
            objS.preserveAspect = true;
            objS.rectTransform.SetWidth(70);
            objS.rectTransform.SetHeight(70);
        }
        else
        {
            if (tempSlot) DestroyImmediate(tempSlot);
            PartHelperToggle();
        }

        slotHelperBool = !slotHelperBool;
    }
    
    [ButtonMethod]
    public void PullSlotPosition()
    {
        if (slotHelperBool || !tempSlot) return;
        GemSlotPosition = tempSlot.transform.localPosition;
    }

    protected virtual void UpdateSprite(Image image)
    {
        image.sprite = ItemIcon;
        image.preserveAspect = true;
        image.SetNativeSize();
    }
}

using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.UI;
using Utility;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "GunData", menuName = "ItemObjectData/GunData", order = 1)]
[Serializable]
public class GunObjectData : ItemObjectData
{
    public Vector2 GunAnchor = new Vector2(-120, 90);
    public Sprite SlotIcon;
    [SerializeField] public List<BodyObject> Bodies = new();
    [SerializeField] public List<BarrelObject> Barrels = new();
    [SerializeField] public List<StockObject> Stocks = new();

    public override void UpdateItems()
    {
        if (Bodies.IsSafe())
        {
            foreach (var g in Bodies)
            {
                DebugExtensions.Log(g.Name + " updating from type " + g.GunPart + " with ID " + g.ID + "...");

                foreach (var gun in Bodies)
                {
                    if (gun.Name != g.Name) continue;
                    if (g.ID == gun.ID) continue;
                    DebugExtensions.Log(g.Name + " name already taken. ID: " + g.ID);
                    return;
                }

                DebugExtensions.Log(g.Name + " updated with type " + g.GunPart + " with ID " + g.ID);
            }
        }
        
        if (Barrels.IsSafe())
        {
            foreach (var g in Barrels)
            {
                DebugExtensions.Log(g.Name + " updating from type " + g.GunPart + " with ID " + g.ID + "...");

                foreach (var gun in Barrels)
                {
                    if (gun.Name != g.Name) continue;
                    if (g.ID == gun.ID) continue;
                    DebugExtensions.Log(g.Name + " name already taken. ID: " + g.ID);
                    return;
                }

                DebugExtensions.Log(g.Name + " updated with type " + g.GunPart + " with ID " + g.ID);
            }
        }
        
        if (Stocks.IsSafe())
        {
            foreach (var g in Stocks)
            {
                DebugExtensions.Log(g.Name + " updating from type " + g.GunPart + " with ID " + g.ID + "...");

                foreach (var gun in Stocks)
                {
                    if (gun.Name != g.Name) continue;
                    if (g.ID == gun.ID) continue;
                    DebugExtensions.Log(g.Name + " name already taken. ID: " + g.ID);
                    return;
                }

                DebugExtensions.Log(g.Name + " updated with type " + g.GunPart + " with ID " + g.ID);
            }
        }
        
        base.UpdateItems();
    }
    
    public bool InstantiateBody(int i, out GameObject gun, out GameObject body)
    {
        if (!Bodies.IsSafe())
        {
            gun = null;
            body = null;
            return false;
        }

        gun = new GameObject(Bodies[i].Name);
        body = new GameObject(Bodies[i].Name)
        {
            transform =
            {
                parent = gun.transform,
                localPosition = GunAnchor,
                localScale = Vector3.one
            }
        };
        
        var objI = body.AddComponent<Image>();
        GameManager.Instance.PlayerStats.CurrBodyObj.Set(Bodies[i]);
        objI.sprite = Bodies[i].Icon;
        objI.preserveAspect = true;
        objI.SetNativeSize();
        SlotChance(body.transform, Bodies[i].GemSlotPosition);
        return true;
    }
    
    public bool InstantiateBarrel(out GameObject barrel)
    {
        if (!Barrels.IsSafe())
        {
            barrel = null;
            return false;
        }
        
        var randBarrel = Barrels.RandomValue();
        barrel = new GameObject(randBarrel.Name);
        var objI = barrel.AddComponent<Image>();
        GameManager.Instance.PlayerStats.CurrBarrelObj.Set(randBarrel);
        objI.sprite = randBarrel.Icon;
        objI.preserveAspect = true;
        objI.SetNativeSize();
        SlotChance(barrel.transform, randBarrel.GemSlotPosition);
        return true;
    }
    
    public bool InstantiateStock(out GameObject stock)
    {
        if (!Barrels.IsSafe())
        {
            stock = null;
            return false;
        }
        
        var randStock = Stocks.RandomValue();
        stock = new GameObject(randStock.Name);
        var objI = stock.AddComponent<Image>();
        GameManager.Instance.PlayerStats.CurrStockObj.Set(randStock);
        objI.sprite = randStock.Icon;
        objI.preserveAspect = true;
        objI.SetNativeSize();
        SlotChance(stock.transform, randStock.GemSlotPosition);
        return true;
    }

    private void SlotChance(Transform parent, Vector3 slotPos)
    {
        var chance = Random.Range(0, 100);
        if (chance % 2 == 0)
        {
            var slot = new GameObject("Slot")
            {
                transform =
                {
                    parent = parent,
                    localPosition = slotPos,
                    localScale = Vector3.one
                }
            };

            var slotI = slot.AddComponent<Image>();
            slotI.sprite = SlotIcon;
            slotI.preserveAspect = true;
            slotI.rectTransform.SetWidth(70);
            slotI.rectTransform.SetHeight(70);
            slot.AddComponent<CircleCollider2D>().radius = 10;
            slot.AddComponent<GunGemSlot>();
            slot.tag = "Slot";
        }
    }

    public void SetPosition(Transform obj, Vector2 pos)
    {
        obj.localPosition = pos;
        obj.localScale = Vector3.one;
    }
}
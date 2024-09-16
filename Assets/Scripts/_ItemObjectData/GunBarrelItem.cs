using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Utility;
using Debug = Utility.Debug;
using UnityEngine;
using UnityEngine.UI;

public class GunBarrelItem : GunAttachmentItem
{
    public override void PullData()
    {
        var GunData = Resources.Load<GunObjectData>("GunData");
        if (!GunData.Barrels.IsSafe()) return;
        var barrelData = GunData.Barrels;
        if (ItemID < 0 && ItemID > barrelData.Count - 1)
        {
            Debug.Log( "Item not found. ID: " + ItemID);
            return;
        }

        ItemName = barrelData[ItemID].Name;
        ItemIcon = barrelData[ItemID].Icon;
        Connector = barrelData[ItemID].Connectors;
        GemSlotPosition = barrelData[ItemID].GemSlotPosition;
        if (!slotHelperBool) tempSlot.transform.localPosition = GemSlotPosition;
        if (!tempItem) return;
        if (!tempItem.TryGetComponent(out Image image)) return;
        UpdateSprite(image);
    }

    public override void CreateItem()
    {
        var GunData = Resources.Load<GunObjectData>("GunData");
        if (GunData.Barrels.IsSafe())
        {
            var barrelData = GunData.Barrels;
        
            if (ItemID > -1)
            {
                if (barrelData.ElementAtOrDefault(ItemID) is not null)
                {
                    Debug.Log(ItemName + " already created. ID: " + ItemID);
                    return;
                }
            }
            
            foreach (var barrel in barrelData)
            {
                if (barrel.Name != ItemName) continue;
                Debug.Log(ItemName + " name already taken. ID: " + ItemID);
                return;
            }

            ItemID = barrelData.Count;
            var newBarrel = new BarrelObject(GunPart, ItemID, ItemName, ItemIcon, GemSlotPosition, Stats, Connector);
            barrelData.Add(newBarrel);
            Debug.Log(ItemName + " created with type " + GunPart + " with and ID " + ItemID);
            ItemID = -1;
        }
        else
        {
            ItemID = 0;
            var newBarrel = new BarrelObject(GunPart, ItemID, ItemName, ItemIcon, GemSlotPosition, Stats, Connector);
            GunData.Barrels = new List<BarrelObject>(1) { newBarrel };
            Debug.Log(ItemName + " created with type " + GunPart + " with ID " + ItemID);
            ItemID = -1;
        }
        
        EditorUtility.SetDirty(GunData);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    
    public override void UpdateItem()
    {
        var GunData = Resources.Load<GunObjectData>("GunData");
        if (GunData.Barrels.IsSafe())
        {
            var barrelData = GunData.Barrels;
            Debug.Log(barrelData[ItemID].Name + " updating from type " + barrelData[ItemID].GunPart + " with ID " + ItemID + "...");

            if (ItemID < 0 || ItemID > barrelData.Count - 1)
            {
                Debug.Log(ItemName + " not found. ID: " + ItemID);
                return;
            }
            
            foreach (var barrel in barrelData)
            {
                if (barrel.Name != ItemName) continue;
                if (ItemID == barrel.ID) continue;
                Debug.Log(ItemName + " name already taken. ID: " + ItemID);
                return;
            }

            var updateBarrel = new BarrelObject(GunPart, ItemID, ItemName, ItemIcon, GemSlotPosition, Stats, Connector);
            barrelData[ItemID] = updateBarrel;
            Debug.Log(ItemName + " updated with type " + GunPart + " with ID " + ItemID);
            EditorUtility.SetDirty(GunData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}

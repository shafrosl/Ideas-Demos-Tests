using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Utility;
using Debug = Utility.Debug;
using UnityEngine;
using UnityEngine.UI;

public class GunBodyItem : GunPartItem
{
    public override void PullData()
    {
        var GunData = Resources.Load<GunObjectData>("GunData");
        if (!GunData.Bodies.IsSafe()) return;
        var bodyData = GunData.Bodies;
        if (ItemID < 0 && ItemID > bodyData.Count - 1)
        {
            Debug.Log( "Item not found. ID: " + ItemID);
            return;
        }

        ItemName = bodyData[ItemID].Name;
        ItemIcon = bodyData[ItemID].Icon;
        GemSlotPosition = bodyData[ItemID].GemSlotPosition;
        if (!slotHelperBool) tempSlot.transform.localPosition = GemSlotPosition;
        if (!tempItem) return;
        if (!tempItem.TryGetComponent(out Image image)) return;
        UpdateSprite(image);
    }

    public override void CreateItem()
    {
        var GunData = Resources.Load<GunObjectData>("GunData");
        if (GunData.Bodies.IsSafe())
        {
            var bodyData = GunData.Bodies;
        
            if (ItemID > -1)
            {
                if (bodyData.ElementAtOrDefault(ItemID) is not null)
                {
                    Debug.Log(ItemName + " already created. ID: " + ItemID);
                    return;
                }
            }
            
            foreach (var body in bodyData)
            {
                if (body.Name != ItemName) continue;
                Debug.Log(ItemName + " name already taken. ID: " + ItemID);
                return;
            }

            ItemID = bodyData.Count;
            var newBody = new BodyObject(GunPart, ItemID, ItemName, ItemIcon, GemSlotPosition, Stats);
            bodyData.Add(newBody);
            Debug.Log(ItemName + " created with type " + GunPart + " with and ID " + ItemID);
            ItemID = -1;
        }
        else
        {
            ItemID = 0;
            var newBody = new BodyObject(GunPart, ItemID, ItemName, ItemIcon, GemSlotPosition, Stats);
            GunData.Bodies = new List<BodyObject>(1) { newBody };
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
        if (GunData.Bodies.IsSafe())
        {
            var bodyData = GunData.Bodies;
            Debug.Log(bodyData[ItemID].Name + " updating from type " + bodyData[ItemID].GunPart + " with ID " + ItemID + "...");

            if (ItemID < 0 || ItemID > bodyData.Count - 1)
            {
                Debug.Log(ItemName + " not found. ID: " + ItemID);
                return;
            }
            
            foreach (var body in bodyData)
            {
                if (body.Name != ItemName) continue;
                if (ItemID == body.ID) continue;
                Debug.Log(ItemName + " name already taken. ID: " + ItemID);
                return;
            }

            var updateBody = new BodyObject(GunPart, ItemID, ItemName, ItemIcon, GemSlotPosition, Stats);
            bodyData[ItemID] = updateBody;
            Debug.Log(ItemName + " updated with type " + GunPart + " with ID " + ItemID);
            EditorUtility.SetDirty(GunData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}

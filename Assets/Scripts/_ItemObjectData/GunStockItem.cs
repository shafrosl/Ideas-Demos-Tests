using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Utility;
using Debug = Utility.Debug;
using UnityEngine;
using UnityEngine.UI;

public class GunStockItem : GunAttachmentItem
{
    public override void PullData()
    {
        var GunData = Resources.Load<GunObjectData>("GunData");
        if (!GunData.Stocks.IsSafe()) return;
        var stockData = GunData.Stocks;
        if (ItemID < 0 && ItemID > stockData.Count - 1)
        {
            Debug.Log( "Item not found. ID: " + ItemID);
            return;
        }

        ItemName = stockData[ItemID].Name;
        ItemIcon = stockData[ItemID].Icon;
        Connector = stockData[ItemID].Connectors;
        GemSlotPosition = stockData[ItemID].GemSlotPosition;
        if (!slotHelperBool) tempSlot.transform.localPosition = GemSlotPosition;
        if (!tempItem) return;
        if (!tempItem.TryGetComponent(out Image image)) return;
        UpdateSprite(image);
    }

    public override void CreateItem()
    {
        var GunData = Resources.Load<GunObjectData>("GunData");
        if (GunData.Stocks.IsSafe())
        {
            var stockData = GunData.Stocks;
        
            if (ItemID > -1)
            {
                if (stockData.ElementAtOrDefault(ItemID) is not null)
                {
                    Debug.Log(ItemName + " already created. ID: " + ItemID);
                    return;
                }
            }
            
            foreach (var stock in stockData)
            {
                if (stock.Name != ItemName) continue;
                Debug.Log(ItemName + " name already taken. ID: " + ItemID);
                return;
            }

            ItemID = stockData.Count;
            var newStock = new StockObject(GunPart, ItemID, ItemName, ItemIcon, GemSlotPosition, Stats, Connector);
            stockData.Add(newStock);
            Debug.Log(ItemName + " created with type " + GunPart + " with and ID " + ItemID);
            ItemID = -1;
        }
        else
        {
            ItemID = 0;
            var newStock = new StockObject(GunPart, ItemID, ItemName, ItemIcon, GemSlotPosition, Stats, Connector);
            GunData.Stocks = new List<StockObject>(1) { newStock };
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
        if (GunData.Stocks.IsSafe())
        {
            var stockData = GunData.Stocks;
            Debug.Log(stockData[ItemID].Name + " updating from type " + stockData[ItemID].GunPart + " with ID " + ItemID + "...");

            if (ItemID < 0 || ItemID > stockData.Count - 1)
            {
                Debug.Log(ItemName + " not found. ID: " + ItemID);
                return;
            }
            
            foreach (var stock in stockData)
            {
                if (stock.Name != ItemName) continue;
                if (ItemID == stock.ID) continue;
                Debug.Log(ItemName + " name already taken. ID: " + ItemID);
                return;
            }

            var updateStock = new StockObject(GunPart, ItemID, ItemName, ItemIcon, GemSlotPosition, Stats, Connector);
            stockData[ItemID] = updateStock;
            Debug.Log(ItemName + " updated with type " + GunPart + " with ID " + ItemID);
            EditorUtility.SetDirty(GunData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}

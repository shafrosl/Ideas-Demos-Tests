using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using UnityEngine;

[Serializable]
public class PlayerStats
{
    [SerializeField, SerializedDictionary("Modifer", "Value")] public SerializedDictionary<Modifier, int> GunStats;
    public BodyObject CurrBodyObj;
    public BarrelObject CurrBarrelObj;
    public StockObject CurrStockObj;

    public PlayerStats() => GunStats = new();
    
    public PlayerStats(PlayerStats playerStats)
    {
        CurrBodyObj = playerStats.CurrBodyObj;
        CurrBarrelObj = playerStats.CurrBarrelObj;
        CurrStockObj = playerStats.CurrStockObj;
        GunStats = new();
    }

    public UniTask SetStats(List<DataObject> stats)
    {
        foreach (var stat in stats)
        {
            GunStats.Add(stat.Slider.Modifier, stat.Slider.CurrentValue);
        }

        return UniTask.CompletedTask;
    }
}

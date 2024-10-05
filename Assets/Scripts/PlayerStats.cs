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
    public Score Score;
    public int Health;
    public int FullHealth;
    
    public PlayerStats() => GunStats = new();
    
    public PlayerStats(PlayerStats playerStats)
    {
        CurrBodyObj = playerStats.CurrBodyObj;
        CurrBarrelObj = playerStats.CurrBarrelObj;
        CurrStockObj = playerStats.CurrStockObj;
        GunStats = new();
        Score = new();
        FullHealth = Health = 5;
    }

    public UniTask SetStats(List<DataObject> stats)
    {
        foreach (var stat in stats)
        {
            GunStats.Add(stat.Slider.Modifier, stat.Slider.CurrentValue);
        }

        return UniTask.CompletedTask;
    }

    public bool Damage()
    {
        if (Health < 1) return false;
        Health -= 1;
        if (Health < 4) GameManager.Instance.HUDController.SwitchTextColor(true);
        return Health != 0;
    }

    public bool Heal()
    {
        if (Health > 4) return false;
        Health += 1;
        if (Health > 3) GameManager.Instance.HUDController.SwitchTextColor(false);
        return Health <= 5;
    }
}

[Serializable]
public class Score
{
    [SerializeField] public int HeadShots;
    [SerializeField] public int BodyShots;
    [SerializeField] public int TotalShots;
    public int GetMisses() => TotalShots - (HeadShots + BodyShots);
    public double GetAccuracy() => (double)(HeadShots + BodyShots) / TotalShots;
}

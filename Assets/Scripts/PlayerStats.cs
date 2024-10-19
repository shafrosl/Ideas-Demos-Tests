using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using TMPro;
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
        return Health != 0;
    }

    public bool Heal()
    {
        if (Health > 4) return false;
        Health += 1;
        return Health <= 5;
    }
}

[Serializable]
public class Score
{
    [SerializeField] public int HeadShots;
    [SerializeField] public int BodyShots;
    [SerializeField] public int TotalShots;
    [SerializeField] public int Kills;
    [SerializeField] public int Mistakes;
    public int GetMisses() => TotalShots - (HeadShots + BodyShots);
    public int GetAccuracy() => (int)(((double)(HeadShots + BodyShots) / TotalShots) * 100);

    public int GetScore()
    {
        switch (GameManager.Instance.GameMode)
        {
            case GameMode.GunRange:
                return (HeadShots * 2) + BodyShots - (Mistakes * 2);
            case GameMode.TimeCrisis:
                return (HeadShots * 2) + BodyShots + (Kills * 10);
            default:
                return 0;
        }
    }

    public void GetVerdict(TextMeshProUGUI Text)
    {
        switch (GameManager.Instance.GameMode)
        {
            case GameMode.GunRange:
                if (GetAccuracy() >= 85)
                {
                    Text.text = "You're alright";
                    Text.color = GameManager.Instance.Black;
                }
                else if (GetAccuracy() >= 45 && GetAccuracy() < 85)
                {
                    Text.text = "You're... ehhh.";
                    Text.color = GameManager.Instance.Black;
                }
                else if (GetAccuracy() < 45)
                {
                    Text.text = "You're pretty shit";
                    Text.color = GameManager.Instance.Red;
                }
                break;
            case GameMode.TimeCrisis:
                break;
        }
    }
}

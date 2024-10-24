using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MyBox;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "GemData", menuName = "ItemObjectData/GemData", order = 2)]
[Serializable]
public class GemObjectData : ItemObjectData
{
    [SerializeField] public Sprite GemFrame;
    [SerializeField] public Sprite[] GemSprites = new Sprite[5];
    [SerializeField] public List<GemColorValue> GemColorValues = new();
    [SerializeField] public List<GemTierCombo> GemTierCombos;
    
    public UniTask<List<GemColorValue>> GetGem()
    {
        var chance = Random.Range(0, 100);
        var randomCombo = GemTierCombos.RandomValue();
        
        while (chance > randomCombo.Chance)
        {
            randomCombo = GemTierCombos.RandomValue();
        }
        
        List<GemColorValue> GCL = new(randomCombo.Range.Count);
        for (var i = 0; i < GCL.Capacity; i++)
        {
            var redo = false;
            var gclObj = GemColorValues.RandomValue();
            foreach (var g in GCL)
            {
                if (gclObj.Modifier == g.Modifier) redo = true;
            }

            if (redo)
            {
                i--;
                continue;
            }
            
            gclObj.RandomizeGem(randomCombo.Range[i]);
            GCL.Add(gclObj);
        }

        return UniTask.FromResult(GCL);
    }

    [Serializable]
    public class GemTierCombo
    {
        [SerializeField] public Tier Tier;
        [SerializeField, Range(0, 100)] public int Chance;
        [SerializeField, RangeVector(new float[] { -50, -45 }, new float[] { 45, 50 })] public List<Vector2Int> Range;
    }
}
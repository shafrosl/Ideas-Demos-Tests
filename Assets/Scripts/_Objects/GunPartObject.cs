using System;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;

[Serializable]
public class GunPartObject : ItemObject
{
    [ReadOnly, SerializeField] public GunPart GunPart;
    [SerializeField] public Vector3 GemSlotPosition;
    [SerializeField] public List<GunModifierValue> Stats = new();
    
    public GunPartObject() { }
    
    public GunPartObject(GunPart itemType, int id, string name, Sprite icon, Vector3 gemSlotPos, List<GunModifierValue> stats)
    {
        GunPart = itemType;
        ID = id;
        Name = name;
        Icon = icon;
        GemSlotPosition = gemSlotPos;
        
        var statsSorted = stats.OrderBy(s => s.Modifier).ToList();
        Stats = statsSorted;
    }
}

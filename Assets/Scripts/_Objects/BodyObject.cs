using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class BodyObject : GunPartObject
{
    public BodyObject(GunPart itemType, int id, string name, Sprite icon, Vector3 gemSlotPos,
        List<GunModifierValue> stats) : base(itemType, id, name, icon, gemSlotPos, stats)
    { }
    
    public virtual void Set(GunPartObject gunPartObject)
    {
        GunPart = gunPartObject.GunPart;
        ID = gunPartObject.ID;
        Name = gunPartObject.Name;
        Icon = gunPartObject.Icon;
        GemSlotPosition = gunPartObject.GemSlotPosition;
        
        var statsSorted = gunPartObject.Stats.OrderBy(s => s.Modifier).ToList();
        Stats = statsSorted;
    }
}
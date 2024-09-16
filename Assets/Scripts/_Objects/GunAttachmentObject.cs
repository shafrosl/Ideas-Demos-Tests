using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Serializable]
public class GunAttachmentObject : GunPartObject
{
    [SerializeField] public Connector Connectors;
    
    public GunAttachmentObject(GunPart itemType, int id, string name, Sprite icon, Vector3 gemSlotPos, List<GunModifierValue> stats, Connector connector = null)
    {
        GunPart = itemType;
        ID = id;
        Name = name;
        Icon = icon;
        GemSlotPosition = gemSlotPos;
        
        var statsSorted = stats.OrderBy(s => s.Modifier).ToList();
        Stats = statsSorted;
        if (connector is null && itemType is not GunPart.Body) Connectors = new Connector();
        Connectors = connector;
    }
    
    public virtual void Set(GunAttachmentObject gunAttachmentObject)
    {
        GunPart = gunAttachmentObject.GunPart;
        ID = gunAttachmentObject.ID;
        Name = gunAttachmentObject.Name;
        Icon = gunAttachmentObject.Icon;
        GemSlotPosition = gunAttachmentObject.GemSlotPosition;
        
        var statsSorted = gunAttachmentObject.Stats.OrderBy(s => s.Modifier).ToList();
        Stats = statsSorted;
        if (gunAttachmentObject.Connectors is null && gunAttachmentObject.GunPart is not GunPart.Body) Connectors = new Connector();
        Connectors = gunAttachmentObject.Connectors;
    }
    
    public Vector2 GetPosition(int bodyID)
    {
        var anchor = GameManager.Instance.GunData.GunAnchor;
        var point = Connectors.Points[bodyID];
        return new Vector2 (point.x + anchor.x, point.y + anchor.y);
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StockObject : GunAttachmentObject
{
    public StockObject(GunPart itemType, int id, string name, Sprite icon, Vector3 gemSlotPos,
        List<GunModifierValue> stats, Connector connector = null) : base(itemType, id, name, icon, gemSlotPos, stats, connector) { }
}

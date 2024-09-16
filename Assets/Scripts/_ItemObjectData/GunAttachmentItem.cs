using MyBox;
using UnityEngine;
using UnityEngine.UI;

public class GunAttachmentItem : GunPartItem
{
    [SerializeField] public Connector Connector;

    public int BodyID;

    protected override void PartHelperToggle()
    {
        if (tempSlot) return;
        if (partHelperBool)
        {
            tempItem = new GameObject(ItemName)
            {
                transform =
                {
                    parent = gameObject.transform,
                    localPosition = BodyID < Connector.Points.Count ? Connector.Points[BodyID] : Vector3.zero,
                    localScale = Vector3.one
                }
            };

            var objI = tempItem.AddComponent<Image>();
            objI.sprite = ItemIcon;
            objI.preserveAspect = true;
            objI.SetNativeSize();
        }
        else
        {
            if (tempItem) DestroyImmediate(tempItem);
        }

        partHelperBool = !partHelperBool;
    }
    
    [ButtonMethod]
    public void PullConnectorPos()
    {
        if (!tempItem) return;
        Connector.UpdateConnectors(BodyID, tempItem.transform.localPosition);
    }

    [ButtonMethod]
    public void MoveToConnectorPos()
    {
        if (!tempItem) return;
        if (BodyID > Connector.Points.Count) return;
        tempItem.transform.localPosition = Connector.Points[BodyID];
    }
}

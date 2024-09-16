using MyBox;
using UnityEngine;

[ExecuteInEditMode]
public class BaseItem : MonoBehaviour
{
    [SerializeField] private int itemID = -1;
    [SerializeField] private Sprite itemIcon;
    [SerializeField] private string itemName;
    [SerializeField] private GunPart gunPart;
    
    public string ItemName
    {
        get => itemName;
        set => itemName = value;
    }
    
    public Sprite ItemIcon
    {
        get => itemIcon;
        set => itemIcon = value;
    }

    public int ItemID
    {
        get => itemID;
        set => itemID = value;
    }

    public GunPart GunPart
    {
        get => gunPart;
        set => gunPart = value;
    }

    private void Update() => ItemUpdate();

    public virtual void ItemUpdate() { }

    [ButtonMethod]
    public virtual void CreateItem() { }

    [ButtonMethod]
    public virtual void UpdateItem() { }

    [ButtonMethod]
    public virtual void PullData() { }
}

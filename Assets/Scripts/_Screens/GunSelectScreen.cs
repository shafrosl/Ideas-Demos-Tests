using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

public class GunSelectScreen : BaseScreen
{
    public Button Left, Right;
    public TextMeshProUGUI Name;
    public Transform Backing;
    public GameObject Gun;
    public List<GemObj> Gems;
    
    public override UniTask Initialize()
    {
        UpdateWeapon();
        return base.Initialize();
    }

    public void RandomizeRarity(int weaponID)
    {
        UpdateWeapon(weaponID);
    }

    public async Task<UniTask> UpdateWeapon(int weaponID = 0)
    {
        if (Gun) Destroy(Gun);
        if (!GameManager.Instance.GunData.Bodies.IsSafe()) return UniTask.CompletedTask;
        if (!GameManager.Instance.GunData.InstantiateBody(weaponID, out var gun, out var body)) return UniTask.CompletedTask;
        if (GameManager.Instance.currBodyObj is not null) Name.text = GameManager.Instance.currBodyObj.Name;
        Gun = gun;
        Gun.transform.parent = Backing.transform;
        Gun.transform.localScale = Vector3.one;
        Gun.transform.localPosition = Vector3.zero;
        await LoadAttachments(weaponID);
        return UniTask.CompletedTask;
    }

    public UniTask LoadAttachments(int weaponID)
    {
        var attachments = Gun.transform.GetAllChildrenInTransform(out var count);
        if (count > 0)
        {
            for (var i = 1; i < count; i++)
            {
                var slotTransform = attachments[i].TryGetChild(0);
                if (slotTransform && slotTransform.TryGetComponent(out GunGemSlot gemSlot))
                {
                    var gemsGuS = GameManager.Instance.GunSelectController.GunSelectScreen.Gems;
                    var found = gemsGuS.FindIndex(x => x.lastCachedGemSlot == gemSlot);
                    if (found > -1)
                    {
                        Destroy(gemsGuS[found].gameObject);
                        gemsGuS.RemoveAt(found);
                    }
                }
                Destroy(attachments[i].gameObject);
            }
        }
        
        if (GameManager.Instance.GunData.InstantiateBarrel(out var barrel))
        {
            barrel.transform.parent = Gun.transform;
            GameManager.Instance.GunData.SetPosition(barrel.transform, GameManager.Instance.currBarrelObj.GetPosition(weaponID));
        }

        if (GameManager.Instance.GunData.InstantiateStock(out var stock))
        {
            stock.transform.parent = Gun.transform;            
            GameManager.Instance.GunData.SetPosition(stock.transform, GameManager.Instance.currStockObj.GetPosition(weaponID));
        }
        return UniTask.CompletedTask;
    }

    public UniTask DestroyGems()
    {
        foreach (var gem in Gems) Destroy(gem.gameObject);
        Gems.Clear();
        return UniTask.CompletedTask;
    }
}

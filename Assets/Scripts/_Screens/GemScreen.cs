using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Utility;
using Debug = Utility.Debug;

public class GemScreen : BaseScreen
{
    public GemSlot[] Slots = new GemSlot[3];
    public List<GemObj> Gems;

    public override async UniTask Initialize()
    {
        await UniTask.Delay(10);
        await CreateGems();
        GameManager.Instance.GunSelectController.RandomizeGems.onClick.AddListener(() => CreateGems());
    }

    private UniTask CreateGems()
    {
        if (Gems.IsSafe())
        {
            foreach (var gem in Gems) Destroy(gem.gameObject);
            Gems.Clear();
        }
        
        foreach (var slot in Slots)
        {
            var gObj = new GameObject("Gem")
            {
                transform =
                {
                    parent = GameManager.Instance.GunSelectController.Canvas.transform,
                    position = slot.transform.position
                }
            };

            gObj.tag = "Gem";
            gObj.AddComponent<RectTransform>();
            gObj.AddComponent<CanvasGroup>();
            gObj.AddComponent<CircleCollider2D>().radius = 35f;

            for (var i = 0; i < 5; i++)
            {
                var o = new GameObject("GemColor " + i)
                {
                    transform =
                    {
                        parent = gObj.transform,
                        localPosition = Vector3.zero
                    }
                };

                var oImg = o.AddComponent<Image>();
                oImg.sprite = GameManager.Instance.GemData.GemSprites[i];
                oImg.preserveAspect = true;
            }

            var f = new GameObject("GemFrame")
            {
                transform =
                {
                    parent = gObj.transform,
                    localPosition = Vector3.zero
                }
            };
            
            var fImg = f.AddComponent<Image>();
            fImg.sprite = GameManager.Instance.GemData.GemFrame;
            fImg.preserveAspect = true;
            
            var gemObj = gObj.AddComponent<GemObj>();
            gemObj.Initialize();
            gemObj.lastCachedGemSlot = slot;
            Gems.Add(gemObj);
        }
        
        return UniTask.CompletedTask;
    }
}

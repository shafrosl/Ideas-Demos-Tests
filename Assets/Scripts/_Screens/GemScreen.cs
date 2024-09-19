using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MyBox;
using UnityEngine;
using UnityEngine.UI;
using Utility;
using Debug = Utility.Debug;

public class GemScreen : BaseScreen
{
    public GemSlot[] Slots = new GemSlot[3];
    public List<GemObject> Gems;

    public override async UniTask Initialize()
    {
        await UniTask.Delay(10);
        await CreateGems();
        GameManager.Instance.GunSelectController.RandomizeGems.onClick.AddListener(() => CreateGems());
    }

    private async UniTask<UniTask> CreateGems()
    {
        await DestroyGems();
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
            gObj.AddComponent<CircleCollider2D>().radius = 40;

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
            
            var gemObj = gObj.AddComponent<GemObject>();
            gemObj.Initialize();
            gemObj.lastCachedGemSlot = slot;
            Gems.Add(gemObj);
        }
        
        return UniTask.CompletedTask;
    }

    public async UniTask<bool> RestoreGems()
    {
        if (!GameManager.Instance.GemsInGame.IsSafe()) return false;
        foreach (var gem in GameManager.Instance.GemsInGame)
        {
            var gObj = new GameObject("Gem")
            {
                transform =
                {
                    parent = GameManager.Instance.GunSelectController.Canvas.transform,
                    position = gem.Key.transform.position
                }
            };
            
            gObj.tag = "Gem";
            gObj.AddComponent<RectTransform>();
            gObj.AddComponent<CanvasGroup>();
            gObj.AddComponent<CircleCollider2D>().radius = 40;

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
            
            var gemObj = gObj.AddComponent<GemObject>();
            await gemObj.RestoreGem(gem.Value);
            gemObj.lastCachedGemSlot = gem.Key;
            if (gem.Key is GunGemSlot) gObj.transform.localScale = (VectorExtensions.CreateV3(0.8f));
            Gems.Add(gemObj);
        }
        
        return true;
    }

    public UniTask DestroyGems()
    {
        if (Gems.IsSafe())
        {
            foreach (var gem in Gems) Destroy(gem.gameObject);
            Gems.Clear();
        }
        
        return UniTask.CompletedTask;
    }
    
    public UniTask StoreGems()
    {
        foreach (var gem in Gems) GameManager.Instance.GemsInGame.Add(gem.lastCachedGemSlot, gem.Mods);
        return UniTask.CompletedTask;
    }
}

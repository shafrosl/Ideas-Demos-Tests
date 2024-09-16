using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Utility;
using Debug = Utility.Debug;

public class GunDataScreen : BaseScreen
{
    public float AnimationTime;
    public Transform Container;
    public DataObject Data;
    private List<DataObject> DataList = new();

    public UniTask InitializeData(BodyObject gunObject)
    {
        DataObject obj;
        foreach (var data in DataList) Destroy(data.gameObject);
        DataList.Clear();
        Data.gameObject.SetActive(true);

        foreach (var stat in gunObject.Stats)
        {
            obj = Instantiate(Data, Container);
            obj.Initialize(stat.Modifier.GetName(), stat.Value);
            DataList.Add(obj);
            obj.gameObject.SetActive(stat.Value > 0);
        }

        Data.gameObject.SetActive(false);
        return UniTask.CompletedTask;
    }

    public UniTask AddData(GunAttachmentObject attachmentObject)
    {
        DataObject obj;
        foreach (var stat in attachmentObject.Stats)
        {
            var found = false;
            foreach (var data in DataList)
            {
                if (data.Name.text == stat.Modifier.GetName())
                {
                    data.Slider.ChangeMainValue(stat.Value);
                    found = true;
                    data.gameObject.SetActive(data.Slider.CurrentValue > 0);
                }
            }
            
            if (found) continue;
            Data.gameObject.SetActive(true);
            obj = Instantiate(Data, Container);
            obj.Initialize(stat.Modifier.GetName(), stat.Value);
            DataList.Add(obj);
            obj.gameObject.SetActive(stat.Value > 0);
            Data.gameObject.SetActive(false);
        }
        
        return UniTask.CompletedTask;
    }
    
    public async UniTask<UniTask> AddData(GemObj gemObj)
    {
        DataObject obj;
        foreach (var mod in gemObj.Mods)
        {
            var found = false;
            foreach (var data in DataList)
            {
                if (data.Name.text == mod.Modifier.GetName())
                {
                    data.Slider.ChangeMainValue(mod.FinalizedValue);
                    found = true;
                    if (data.Slider.CurrentValue > 0)
                    {
                        if (!data.gameObject.activeSelf)
                        {
                            await data.Fade(0);
                            data.gameObject.SetActive(true);
                            data.Fade(1, AnimationTime);
                        }
                    }
                    else
                    {
                        data.Fade(0, AnimationTime);
                        data.gameObject.SetActive(false);
                        data.Slider.Reset();
                    }
                }
            }
            
            if (found) continue;
            Data.gameObject.SetActive(true);
            obj = Instantiate(Data, Container);
            obj.Initialize(mod.Modifier.GetName(), mod.FinalizedValue);
            DataList.Add(obj);
            obj.gameObject.SetActive(mod.FinalizedValue > 0);
            Data.gameObject.SetActive(false);
        }
        
        return UniTask.CompletedTask;
    }
    
    public UniTask<DataObject> AddData(GemColorValue mod)
    {
        DataObject obj;
        Data.gameObject.SetActive(true);
        obj = Instantiate(Data, Container);
        obj.Initialize(mod.Modifier.GetName(), 0);
        DataList.Add(obj);
        Data.gameObject.SetActive(false);
        return UniTask.FromResult(obj);
    }

    public async UniTask<UniTask> RemoveData(GemObj gemObj)
    {
        foreach (var mod in gemObj.Mods)
        {
            foreach (var data in DataList)
            {
                if (data.Name.text == mod.Modifier.GetName())
                {
                    data.Slider.ChangeMainValue(-mod.FinalizedValue);
                    if (data.Slider.CurrentValue > 0)
                    {
                        if (!data.gameObject.activeSelf)
                        {
                            await data.Fade(0);
                            data.gameObject.SetActive(true);
                            data.Fade(1, AnimationTime);
                        }
                    }
                    else
                    {
                        data.Fade(0, AnimationTime);
                        data.gameObject.SetActive(false);
                        data.Slider.Reset();
                    }
                }
            }
        }
        return UniTask.CompletedTask;
    }

    public async UniTask<UniTask> AddTempData(GemObj gemObj)
    {
        foreach (var mod in gemObj.Mods)
        {
            var found = false;
            foreach (var data in DataList)
            {
                if (data.Name.text == mod.Modifier.GetName())
                {
                    found = true;
                    data.Slider.ChangeTemporaryValue(mod.FinalizedValue);
                    if (data.Slider.AlteredValue > 0)
                    {
                        if (!data.gameObject.activeSelf)
                        {
                            await data.Fade(0);
                            data.gameObject.SetActive(true);
                            data.Fade(1, AnimationTime);
                        }
                    }
                    else
                    {
                        data.Fade(0, AnimationTime);
                        data.gameObject.SetActive(false);
                        data.Slider.Reset();
                    }
                }
            }

            if (!found)
            {
                var obj = await AddData(mod);
                obj.Slider.ChangeTemporaryValue(mod.FinalizedValue);
                if (obj.Slider.AlteredValue > 0)
                {
                    if (!obj.gameObject.activeSelf)
                    {
                        await obj.Fade(0);
                        obj.gameObject.SetActive(true);
                        obj.Fade(1, AnimationTime);
                    }
                }
                else
                {
                    obj.gameObject.SetActive(false);
                    obj.Slider.Reset();
                }
            }
        }
        return UniTask.CompletedTask;
    }
    
    public async UniTask<UniTask> RemoveTempData(GemObj gemObj)
    {
        foreach (var mod in gemObj.Mods)
        {
            foreach (var data in DataList)
            {
                if (data.Name.text == mod.Modifier.GetName())
                {
                    data.Slider.ResetAlteredValue();
                    if (data.Slider.CurrentValue > 0)
                    {
                        if (!data.gameObject.activeSelf)
                        {
                            await data.Fade(0);
                            data.gameObject.SetActive(true);
                            data.Fade(1, AnimationTime);
                        }
                    }
                    else
                    {
                        data.Fade(0, AnimationTime);
                        data.gameObject.SetActive(false);
                        data.Slider.Reset();
                    }
                }
            }
        }
        return UniTask.CompletedTask;
    }
}

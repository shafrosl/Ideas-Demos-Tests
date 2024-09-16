using System;
using MyBox;
using UnityEditor;
using UnityEngine;

[Serializable]
public class ItemObjectData : ScriptableObject
{
    [ButtonMethod]
    public virtual void UpdateItems()
    {
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
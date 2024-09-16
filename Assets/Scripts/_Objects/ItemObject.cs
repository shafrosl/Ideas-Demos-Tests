using System;
using MyBox;
using UnityEngine;

[Serializable]
public abstract class ItemObject
{
    [ReadOnly, SerializeField] public int ID;
    [SerializeField] public Sprite Icon;
    [SerializeField] public string Name;
}

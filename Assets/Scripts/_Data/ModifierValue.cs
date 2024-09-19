using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class ModifierValue
{
    [SerializeField] public Modifier Modifier;
}

[Serializable]
public class GemColorValue : ModifierValue
{
    [SerializeField] public Color Color;
    [SerializeField] public int FinalizedValue;
    public void RandomizeGem(Vector2Int Range) => FinalizedValue = Random.Range(Range.x, Range.y);
}

[Serializable]
public class GunModifierValue : ModifierValue
{
    [SerializeField] public int Value;
}
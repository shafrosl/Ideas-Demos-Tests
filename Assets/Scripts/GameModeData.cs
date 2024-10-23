using System;
using UnityEngine;

[Serializable]
public class GameModeData
{
    [SerializeField] public GameMode GameMode;
    [SerializeField] public GameObject Room;
    [SerializeField] public Vector3 StartPosition;
}
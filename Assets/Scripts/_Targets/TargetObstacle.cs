using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetObstacle : TargetStatic
{
    [SerializeReference] private  Transform position;
    public Vector3 GetPlayerPos => position.position;
}

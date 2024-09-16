using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using Debug = Utility.Debug;

[Serializable]
public class Connector
{
    [SerializeField] public List<Vector3> Points;
    
    public Connector() => RemakeList();

    private void RemakeList()
    {
        var BodyData = Resources.Load<GunObjectData>("GunData").Bodies;
        Points = new(BodyData.Count);
        foreach (var body in BodyData) Points.Add(Vector3.zero);
    }
    
    public void UpdateConnectors(List<Vector3> newPoints) => Points = newPoints;
    public void UpdateConnectors(int id, Vector3 point)
    {
        if (Points.IsSafe())
        {
            if (Points.Capacity > 0)
            {
                Points[id] = point;
            }
        }
        else
        {
            RemakeList();
            if (Points.Capacity > 0)
            {
                Points[id] = point;
            }
        }
    }

    public List<Vector3> GetConnectors()
    {
        var BodyData = Resources.Load<GunObjectData>("GunData").Bodies;
        var tempPoints = new List<Vector3>(BodyData.Count);

        for (var i = 0; i < Points.Count; i++)
        {
            tempPoints[i] = Points[i];
        }

        return tempPoints;
    }
}
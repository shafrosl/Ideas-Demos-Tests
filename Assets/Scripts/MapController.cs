using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using Utility;
using Debug = Utility.Debug;

public class MapController : MonoBehaviour
{
    [SerializeReference] private GameObject Range;
    [SerializeReference] private GameObject TimeCrisis;

    public GameObject StartingRoom;
    public List<GameObject> Rooms;
    private List<GameObject> SpawnedRooms = new();
    
    public void SetMap()
    {
        if (Range) Range.SetActive(GameManager.Instance.GameMode == GameMode.GunRange);
        if (TimeCrisis) TimeCrisis.SetActive(GameManager.Instance.GameMode == GameMode.TimeCrisis);
    }
    
    
    [ButtonMethod()]
    public void GenerateMap()
    {
        if (SpawnedRooms.IsSafe()) SpawnedRooms.Clear();
        var parent = new GameObject("Rooms")
        {
            transform =
            {
                parent = TimeCrisis.transform
            }
        };

        var currDir = Direction.Right;
        var x = 0;
        var z = 0;
        for (var i = 0; i < 10; i++)
        {
            GameObject room;
            Vector3 pos = Vector3.zero;
            var tempX = x;
            var tempZ = z;
            
            switch (currDir)
            {
                case Direction.Up:
                    pos = new Vector3(tempX -= 103, 0, z);
                    break;
                case Direction.Down:
                    pos = new Vector3(tempX += 103, 0, z);
                    break;
                case Direction.Left:
                    pos = new Vector3(x, 0, tempZ -= 103);
                    break;
                case Direction.Right:
                    pos = new Vector3(x, 0, tempZ += 103);
                    break;
            }
            
            if (i == 0)
            {
                room = StartingRoom;
            }
            else 
            {
                room = SpawnedRooms[^1].GetComponent<TargetControllerRoom>().GetNextRoom(Rooms, currDir, out var newDir);
                if (room is null)
                {
                    i--;
                    continue;
                }
                
                Collider[] results = new Collider[4];
                var c = 0;
                switch (newDir)
                {
                    case Direction.Up:
                        c = Physics.OverlapSphereNonAlloc(pos.Modify(x:tempX - 103), 5, results);
                        break;
                    case Direction.Down:
                        c = Physics.OverlapSphereNonAlloc(pos.Modify(x:tempX + 103), 5, results);
                        break;
                    case Direction.Left:
                        c = Physics.OverlapSphereNonAlloc(pos.Modify(z:tempZ - 103), 5, results);
                        break;
                    case Direction.Right:
                        c = Physics.OverlapSphereNonAlloc(pos.Modify(z:tempZ + 103), 5, results);
                        break; 
                }
                
                if (c > 0)
                {
                    Debug.Log(i + ": " + room.name + " c = " + c + " dir: " + newDir);
                    i--;
                    continue;
                }
                currDir = newDir;
            }
            
            x = tempX;
            z = tempZ;
            Debug.Log(i + ": " + pos + " dir: " + currDir);
            Instantiate(room, pos, Quaternion.identity, parent.transform);
            SpawnedRooms.Add(room);
        }
    }
}

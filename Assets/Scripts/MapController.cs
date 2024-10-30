using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MyBox;
using UnityEditor;
using UnityEngine;
using Utility;

public class MapController : MonoBehaviour
{
    [Header("Data")]
    public GameModeData Range;
    public GameModeData TimeCrisis;
    
    [Header("Rooms")]
    public TargetControllerRoom StartingRoom;
    public List<TargetControllerRoom> Rooms;
    
    private List<TargetControllerRoom> SpawnedRooms = new();
    private GameObject TCParentRoom;
    
    public async UniTask SetMap()
    {
        if (Range.Room)
        {
            Range.Room.SetActive(GameManager.Instance.GameMode == GameMode.GunRange);
            GameManager.Instance.PlayerController.Cinemachine.transform.position = Range.StartPosition;
        }
        
        if (TimeCrisis.Room)
        {
            TimeCrisis.Room.SetActive(GameManager.Instance.GameMode == GameMode.TimeCrisis);
            GameManager.Instance.PlayerController.Cinemachine.transform.position = TimeCrisis.StartPosition;
            await GenerateTCMap();
        }
    }

    [ButtonMethod()]
    public UniTask GenerateTCMap()
    {
        if (SpawnedRooms.IsSafe()) SpawnedRooms.Clear();
        TCParentRoom = new GameObject("Rooms")
        {
            transform =
            {
                parent = TimeCrisis.Room.transform
            }
        };

        var currDir = Direction.Right;
        var x = 0;
        var z = 0;
        var numOfTries = 10;
        for (var i = 0; i < 10; i++)
        {
            TargetControllerRoom room;
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
                room = SpawnedRooms[^1].GetNextRoom(Rooms, currDir, out var newDir);
                if (room is null)
                {
                    i--;
                    continue;
                }
                
                var results = new Collider[4];
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
                    i--;
                    numOfTries--;
                    if (numOfTries < 0)
                    {
                        Debug.Log("failed");
                        EditorApplication.ExitPlaymode();
                        return UniTask.CompletedTask;
                    }
                    continue;
                }
                currDir = newDir;
            }
            
            x = tempX;
            z = tempZ;
            var newRoom = Instantiate(room, pos, Quaternion.identity, TCParentRoom.transform);
            newRoom.SpawnObjects();
            SpawnedRooms.Add(newRoom);
            numOfTries = 10;
        }

        return UniTask.CompletedTask;
    }

    public void ClearMap()
    {
        if (GameManager.Instance.Targets.IsSafe())
        {
            foreach (var target in GameManager.Instance.Targets) Destroy(target);
            GameManager.Instance.Targets.Clear();
        }

        if (SpawnedRooms.IsSafe())
        {
            foreach (var room in SpawnedRooms) Destroy(room.gameObject);
            SpawnedRooms.Clear();
            Destroy(TCParentRoom);
            TCParentRoom = null;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using MyBox;
using UnityEditor;
using UnityEngine;
using Utility;
using PathCreation;

public class MapController : MonoBehaviour
{
    [Header("Data")]
    public GameModeData Range;
    public GameModeData TimeCrisis;
    public PathCreator PathCreator;
    
    [Header("Rooms")]
    public TargetControllerRoom StartingRoom;
    public List<TargetControllerRoom> Rooms;
    
    private Queue<TargetControllerRoom> SpawnedRooms = new();
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
    public async UniTask GenerateTCMap()
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
                room = SpawnedRooms.ToList()[^1].GetNextRoom(Rooms, currDir, out var newDir);
                if (room is null)
                {
                    i--;
                    continue;
                }
                
                var results = new Collider[4];
                var c = 0;
                // checks for next grid, and three grids around the next grid
                switch (newDir)
                {
                    case Direction.Up:
                        c = Physics.OverlapSphereNonAlloc(pos.Modify(x:tempX - 103), 5, results);
                        c += Physics.OverlapSphereNonAlloc(pos.Modify(x:tempX - 206), 5, results);
                        c += Physics.OverlapSphereNonAlloc(pos.Modify(x:tempX - 103, z:tempZ - 103), 5, results);
                        c += Physics.OverlapSphereNonAlloc(pos.Modify(x:tempX - 103, z:tempZ + 103), 5, results);
                        break;
                    case Direction.Down:
                        c = Physics.OverlapSphereNonAlloc(pos.Modify(x:tempX + 103), 5, results);
                        c += Physics.OverlapSphereNonAlloc(pos.Modify(x:tempX + 206), 5, results);
                        c += Physics.OverlapSphereNonAlloc(pos.Modify(x:tempX + 103, z:tempZ - 103), 5, results);
                        c += Physics.OverlapSphereNonAlloc(pos.Modify(x:tempX + 103, z:tempZ + 103), 5, results);
                        break;
                    case Direction.Left:
                        c = Physics.OverlapSphereNonAlloc(pos.Modify(z:tempZ - 103), 5, results);
                        c += Physics.OverlapSphereNonAlloc(pos.Modify(z:tempZ - 206), 5, results);
                        c += Physics.OverlapSphereNonAlloc(pos.Modify(z:tempZ - 103, x:tempX - 103), 5, results);
                        c += Physics.OverlapSphereNonAlloc(pos.Modify(z:tempZ - 103, x:tempX + 103), 5, results);
                        break;
                    case Direction.Right:
                        c = Physics.OverlapSphereNonAlloc(pos.Modify(z:tempZ + 103), 5, results);
                        c += Physics.OverlapSphereNonAlloc(pos.Modify(z:tempZ + 206), 5, results);
                        c += Physics.OverlapSphereNonAlloc(pos.Modify(z:tempZ + 103, x:tempX - 103), 5, results);
                        c += Physics.OverlapSphereNonAlloc(pos.Modify(z:tempZ + 103, x:tempX + 103), 5, results);
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
                        return;
                    }
                    continue;
                }
                currDir = newDir;
            }
            
            var newRoom = Instantiate(room, pos, Quaternion.identity, TCParentRoom.transform);
            newRoom.xPos = x = tempX;
            newRoom.zPos = z = tempZ;
            newRoom.SpawnObjects();
            SpawnedRooms.Enqueue(newRoom);
            numOfTries = 10;
        }

        await CreatePath();
    }

    [ButtonMethod()]
    public async UniTask AdjustMap()
    {
        if (!SpawnedRooms.IsSafe()) return;
        TCParentRoom = new GameObject("Rooms")
        {
            transform =
            {
                parent = TimeCrisis.Room.transform
            }
        };

        var spawnRoomList = SpawnedRooms.ToList();
        var currDir = spawnRoomList[^1].RoomDirection;
        var x = spawnRoomList[^1].xPos;
        var z = spawnRoomList[^1].zPos;
        var numOfTries = 5;
        for (var i = 0; i < 2; i++)
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
            
            room = SpawnedRooms.ToList()[^1].GetNextRoom(Rooms, currDir, out var newDir);
            if (room is null)
            {
                i--;
                continue;
            }
                
            var results = new Collider[4];
            var c = 0;
            // checks for next grid, and three grids around the next grid
            switch (newDir)
            {
                case Direction.Up:
                    c = Physics.OverlapSphereNonAlloc(pos.Modify(x:tempX - 103), 5, results);
                    c += Physics.OverlapSphereNonAlloc(pos.Modify(x:tempX - 206), 5, results);
                    c += Physics.OverlapSphereNonAlloc(pos.Modify(x:tempX - 103, z:tempZ - 103), 5, results);
                    c += Physics.OverlapSphereNonAlloc(pos.Modify(x:tempX - 103, z:tempZ + 103), 5, results);
                    break;
                case Direction.Down:
                    c = Physics.OverlapSphereNonAlloc(pos.Modify(x:tempX + 103), 5, results);
                    c += Physics.OverlapSphereNonAlloc(pos.Modify(x:tempX + 206), 5, results);
                    c += Physics.OverlapSphereNonAlloc(pos.Modify(x:tempX + 103, z:tempZ - 103), 5, results);
                    c += Physics.OverlapSphereNonAlloc(pos.Modify(x:tempX + 103, z:tempZ + 103), 5, results);
                    break;
                case Direction.Left:
                    c = Physics.OverlapSphereNonAlloc(pos.Modify(z:tempZ - 103), 5, results);
                    c += Physics.OverlapSphereNonAlloc(pos.Modify(z:tempZ - 206), 5, results);
                    c += Physics.OverlapSphereNonAlloc(pos.Modify(z:tempZ - 103, x:tempX - 103), 5, results);
                    c += Physics.OverlapSphereNonAlloc(pos.Modify(z:tempZ - 103, x:tempX + 103), 5, results);
                    break;
                case Direction.Right:
                    c = Physics.OverlapSphereNonAlloc(pos.Modify(z:tempZ + 103), 5, results);
                    c += Physics.OverlapSphereNonAlloc(pos.Modify(z:tempZ + 206), 5, results);
                    c += Physics.OverlapSphereNonAlloc(pos.Modify(z:tempZ + 103, x:tempX - 103), 5, results);
                    c += Physics.OverlapSphereNonAlloc(pos.Modify(z:tempZ + 103, x:tempX + 103), 5, results);
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
                    return;
                }
                continue;
            }
            currDir = newDir;
            
            var newRoom = Instantiate(room, pos, Quaternion.identity, TCParentRoom.transform);
            newRoom.xPos = x = tempX;
            newRoom.zPos = z = tempZ;
            newRoom.SpawnObjects();
            SpawnedRooms.Enqueue(newRoom);

            var oldRoom = SpawnedRooms.Peek();
            Destroy(oldRoom.gameObject);
            SpawnedRooms.Dequeue();
            
            numOfTries = 5;
        }

        await CreatePath();
    }

    private UniTask CreatePath()
    {
        List<Transform> paths = new();
        foreach (var room in SpawnedRooms)
        {
            foreach (var pts in room.GetSortedPathPoints(paths.IsSafe() ? paths[^1] : null))
            {
                paths.Add(pts);
            }
        }

        PathCreator.bezierPath = new BezierPath(paths, false, PathSpace.xz);
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

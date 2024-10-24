using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

public class TargetControllerRoom : TargetControllerStatic
{
    public Direction[] Directions;
    public RoomType RoomType;
    public int TargetFacing;
    public Transform[] EnemyPos, ObstaclePos;

    public TargetControllerRoom GetNextRoom(List<TargetControllerRoom> rooms, Direction currDir, out Direction newDir)
    {
        List<TargetControllerRoom> allowedRooms = new();
        var backtrackDir = Direction.None;
        foreach (var room in rooms)
        {
            switch (currDir)
            {
                case Direction.Up:
                    backtrackDir = Direction.Down;
                    allowedRooms.AddRange(from dir in room.Directions where dir == Direction.Down select room);
                    break;
                case Direction.Down:
                    backtrackDir = Direction.Up;
                    allowedRooms.AddRange(from dir in room.Directions where dir == Direction.Up select room);
                    break;
                case Direction.Left:
                    backtrackDir = Direction.Right;
                    allowedRooms.AddRange(from dir in room.Directions where dir == Direction.Right select room);
                    break;
                case Direction.Right:
                    backtrackDir = Direction.Left;
                    allowedRooms.AddRange(from dir in room.Directions where dir == Direction.Left select room);
                    break;
            }
        }

        var returnRoom = allowedRooms.RandomValue();
        newDir = Direction.None;
        if (returnRoom.Directions.Length > 1)
        {
            if (returnRoom.RoomType == RoomType.Straight) newDir = currDir;
            else
            {
                foreach (var dir in returnRoom.Directions)
                {
                    if (dir != currDir && dir != backtrackDir) newDir = dir;
                }
            }
        }
        else
        {
            newDir = returnRoom.Directions[0];
        }

        returnRoom.SetRoomRotation(currDir);
        return returnRoom;
    }

    public void SpawnObjects()
    {
        var gm = GameManager.Instance;
        foreach (var ePos in EnemyPos)
        {
            var chance = Random.Range(0, 10);
            if (chance > 6) continue;
            if (!gm.Enemies.IsSafe()) break;
            var enemy = Instantiate(gm.Enemies.RandomValue(), ePos.position, Quaternion.identity, ePos);
            enemy.transform.rotation = Quaternion.Euler(0, TargetFacing, 0);
            gm.Targets.Add(enemy);
        }
        
        foreach (var oPos in ObstaclePos)
        {
            var chance = Random.Range(0, 10);
            if (chance > 8) continue;
            if (!gm.Obstacles.IsSafe()) break;
            var obs = Instantiate(gm.Obstacles.RandomValue(), oPos.position, Quaternion.identity, oPos);
            obs.transform.rotation = quaternion.Euler(0, Random.Range(0, 359), 0);
            gm.Targets.Add(obs);
        }
    }

    private void SetRoomRotation(Direction currDir)
    {
        switch (currDir)
        {
            case Direction.Up:
                TargetFacing = 270;
                break;
            case Direction.Down:
                TargetFacing = 90;
                break;
            case Direction.Left:
                TargetFacing = 180;
                break;
            case Direction.Right:
                TargetFacing = 0;
                break;
        }
    }
}

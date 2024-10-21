using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Debug = Utility.Debug;

public class TargetControllerRoom : TargetControllerStatic
{
    public Direction[] Directions;
    public RoomType RoomType;

    public GameObject GetNextRoom(List<GameObject> rooms, Direction currDir, out Direction newDir)
    {
        List<GameObject> allowedRooms = new();
        var backtrackDir = Direction.None;
        foreach (var room in rooms)
        {
            if (!room.TryGetComponent(out TargetControllerRoom tcr)) continue;
            switch (currDir)
            {
                case Direction.Up:
                    backtrackDir = Direction.Down;
                    allowedRooms.AddRange(from dir in tcr.Directions where dir == Direction.Down select room);
                    break;
                case Direction.Down:
                    backtrackDir = Direction.Up;
                    allowedRooms.AddRange(from dir in tcr.Directions where dir == Direction.Up select room);
                    break;
                case Direction.Left:
                    backtrackDir = Direction.Right;
                    allowedRooms.AddRange(from dir in tcr.Directions where dir == Direction.Right select room);
                    break;
                case Direction.Right:
                    backtrackDir = Direction.Left;
                    allowedRooms.AddRange(from dir in tcr.Directions where dir == Direction.Left select room);
                    break;
            }
        }

        var returnRoom = allowedRooms[Random.Range(0, allowedRooms.Count)];
        newDir = Direction.None;
        if (!returnRoom.TryGetComponent(out TargetControllerRoom tgc)) return null;
        if (tgc.Directions.Length > 1)
        {
            if (tgc.RoomType == RoomType.Straight) newDir = currDir;
            else
            {
                foreach (var dir in tgc.Directions)
                {
                    if (dir != currDir && dir != backtrackDir) newDir = dir;
                }
            }
        }
        else
        {
            newDir = tgc.Directions[0];
        }
        
        return returnRoom;
    }
}

using System.Collections.Generic;
using UnityEngine;

public class RoomDataExtractor
{
    public void ProcessRoom(Room room, HashSet<Vector2Int> pathTiles)
    {
        foreach (var tilePosition in room.FloorTiles)
        {
            int neighborsCount = 0;

            bool hasUp = room.FloorTiles.Contains(tilePosition + Vector2Int.up);
            bool hasDown = room.FloorTiles.Contains(tilePosition + Vector2Int.down);
            bool hasRight = room.FloorTiles.Contains(tilePosition + Vector2Int.right);
            bool hasLeft = room.FloorTiles.Contains(tilePosition + Vector2Int.left);

            if (hasUp) neighborsCount++;
            if (hasDown) neighborsCount++;
            if (hasRight) neighborsCount++;
            if (hasLeft) neighborsCount++;

            if (!hasUp) room.NearWallTilesUp.Add(tilePosition);
            if (!hasDown) room.NearWallTilesDown.Add(tilePosition);
            if (!hasRight) room.NearWallTilesRight.Add(tilePosition);
            if (!hasLeft) room.NearWallTilesLeft.Add(tilePosition);

            if (neighborsCount <= 2) room.CornerTiles.Add(tilePosition);
            if (neighborsCount == 4) room.InnerTiles.Add(tilePosition);
        }

        room.NearWallTilesUp.ExceptWith(room.CornerTiles);
        room.NearWallTilesDown.ExceptWith(room.CornerTiles);
        room.NearWallTilesLeft.ExceptWith(room.CornerTiles);
        room.NearWallTilesRight.ExceptWith(room.CornerTiles);

        room.NearWallTilesUp.ExceptWith(pathTiles);
        room.NearWallTilesDown.ExceptWith(pathTiles);
        room.NearWallTilesLeft.ExceptWith(pathTiles);
        room.NearWallTilesRight.ExceptWith(pathTiles);
        room.CornerTiles.ExceptWith(pathTiles);
        room.InnerTiles.ExceptWith(pathTiles);
    }
}
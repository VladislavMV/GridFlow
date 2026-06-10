using System.Collections.Generic;
using UnityEngine;

public enum RoomType
{
    Normal,
    Start,
    Exit,
    Treasure,
    Enemy
}

public class Room
{
    public Vector2Int Center { get; set; }
    public HashSet<Vector2Int> FloorTiles { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> FreeFloorTiles { get; set; }
    public RoomType RoomRole { get; set; } = RoomType.Normal;
    public HashSet<Vector2Int> NearWallTilesUp { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> NearWallTilesDown { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> NearWallTilesLeft { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> NearWallTilesRight { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> CornerTiles { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> InnerTiles { get; set; } = new HashSet<Vector2Int>();

    public Room(Vector2Int center, HashSet<Vector2Int> floorTiles)
    {
        Center = center;
        FloorTiles = floorTiles;
        FreeFloorTiles = new HashSet<Vector2Int>(floorTiles);
    }
}
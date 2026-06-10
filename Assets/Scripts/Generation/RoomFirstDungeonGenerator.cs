using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomFirstDungeonGenerator : RandomWalkDungeonGenerator
{
    [SerializeField] private int minRoomWidth = 4, minRoomHeight = 4;
    [SerializeField] private int dungeonWidth = 20, dungeonHeight = 20;
    [SerializeField][Range(0, 10)] private int offset = 1;
    [SerializeField] private bool randomWalkRooms = false;
    [SerializeField][Range(1, 5)] private int corridorWidth = 2;
    [SerializeField] private PlayerSpawner playerSpawner;
    [SerializeField] private PropPlacementManager propPlacementManager;

    public List<Room> GeneratedRooms { get; private set; } = new List<Room>();
    public HashSet<Vector2Int> GeneratedCorridors { get; private set; } = new HashSet<Vector2Int>();

    protected override void RunProceduralGeneration()
    {
        GeneratedRooms.Clear();
        GeneratedCorridors.Clear();

        if (propPlacementManager != null)
        {
            propPlacementManager.ClearProps();
        }

        CreateRooms();

        RoomDataExtractor extractor = new RoomDataExtractor();
        foreach (var room in GeneratedRooms)
        {
            extractor.ProcessRoom(room, GeneratedCorridors);
        }

        if (!AssignRoomRoles())
        {
            Debug.LogWarning("Згенеровано менше 3 кімнат. Перестворюємо підземелля.");
            GenerateDungeon();
            return;
        }

        if (playerSpawner != null && GeneratedRooms.Count > 0)
        {
            playerSpawner.SpawnPlayerAtPosition(GeneratedRooms[0].Center);
        }

        if (propPlacementManager != null)
        {
            propPlacementManager.PlaceProps(GeneratedRooms);
        }
    }

    private void Start()
    {
        GenerateDungeon();
    }

    private void CreateRooms()
    {
        var roomsList = ProcGenerationAlgorithm.BinarySpacePartitioning(new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        if (randomWalkRooms)
        {
            floor = CreateRoomsRandomly(roomsList);
        }
        else
        {
            floor = CreateSimpleRooms(roomsList);
        }

        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (var room in roomsList)
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }

        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        GeneratedCorridors.UnionWith(corridors);
        floor.UnionWith(corridors);

        tilemapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);
    }

    private HashSet<Vector2Int> CreateRoomsRandomly(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        for (int i = 0; i < roomsList.Count; i++)
        {
            var roomBounds = roomsList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
            HashSet<Vector2Int> validRoomFloor = new HashSet<Vector2Int>();

            foreach (var position in roomFloor)
            {
                if (position.x >= (roomBounds.xMin + offset) && position.x <= (roomBounds.xMax - offset) && position.y >= (roomBounds.yMin - offset) && position.y <= (roomBounds.yMax - offset))
                {
                    floor.Add(position);
                    validRoomFloor.Add(position);
                }
            }

            GeneratedRooms.Add(new Room(roomCenter, validRoomFloor));
        }
        return floor;
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);

            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }
        return corridors;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;

        while (position.y != destination.y)
        {
            if (destination.y > position.y) position += Vector2Int.up;
            else if (destination.y < position.y) position += Vector2Int.down;

            AddCorridorBlock(corridor, position);
        }

        while (position.x != destination.x)
        {
            if (destination.x > position.x) position += Vector2Int.right;
            else if (destination.x < position.x) position += Vector2Int.left;

            AddCorridorBlock(corridor, position);
        }
        return corridor;
    }

    private void AddCorridorBlock(HashSet<Vector2Int> corridor, Vector2Int pos)
    {
        for (int x = 0; x < corridorWidth; x++)
        {
            for (int y = 0; y < corridorWidth; y++)
            {
                corridor.Add(pos + new Vector2Int(x, y));
            }
        }
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (var position in roomCenters)
        {
            float currentDistance = Vector2.Distance(position, currentRoomCenter);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }
        return closest;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (var roomBounds in roomsList)
        {
            HashSet<Vector2Int> roomFloor = new HashSet<Vector2Int>();

            for (int col = offset; col < roomBounds.size.x - offset; col++)
            {
                for (int row = offset; row < roomBounds.size.y - offset; row++)
                {
                    Vector2Int tilePos = (Vector2Int)roomBounds.min + new Vector2Int(col, row);
                    roomFloor.Add(tilePos);
                    floor.Add(tilePos);
                }
            }

            Vector2Int center = (Vector2Int)Vector3Int.RoundToInt(roomBounds.center);
            GeneratedRooms.Add(new Room(center, roomFloor));
        }
        return floor;
    }

    private bool AssignRoomRoles()
    {
        if (GeneratedRooms.Count < 3)
        {
            return false;
        }

        Room startRoom = GeneratedRooms[0];
        startRoom.RoomRole = RoomType.Start;

        Room exitRoom = FindFurthestRoom(startRoom, GeneratedRooms);
        exitRoom.RoomRole = RoomType.Exit;

        List<Room> availableRooms = new List<Room>();
        foreach (var room in GeneratedRooms)
        {
            if (room.RoomRole == RoomType.Normal) availableRooms.Add(room);
        }

        if (availableRooms.Count > 0)
        {
            Room treasureRoom = availableRooms[Random.Range(0, availableRooms.Count)];
            treasureRoom.RoomRole = RoomType.Treasure;
            availableRooms.Remove(treasureRoom);
        }

        foreach (var room in availableRooms)
        {
            room.RoomRole = RoomType.Enemy;
        }

        return true;
    }

    private Room FindFurthestRoom(Room startRoom, List<Room> allRooms)
    {
        Room furthestRoom = null;
        float maxDistance = 0f;

        foreach (var room in allRooms)
        {
            if (room == startRoom) continue;

            float distance = Vector2.Distance(startRoom.Center, room.Center);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                furthestRoom = room;
            }
        }
        return furthestRoom;
    }
}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PropPlacementManager : MonoBehaviour
{
    [SerializeField] private List<PropData> propsToPlace;
    [Header("Special Room Objects")]
    [SerializeField] private GameObject chestPrefab;
    [SerializeField] private GameObject exitPrefab;

    [Header("Debug Settings")]
    [SerializeField] private bool showDebugGizmos = true;

    private HashSet<Vector2Int> debugReachableTiles = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> debugBlockedTiles = new HashSet<Vector2Int>();

    public void PlaceProps(List<Room> rooms)
    {
        debugReachableTiles.Clear();
        debugBlockedTiles.Clear();

        float difficultyMultiplier = 1f;
        if (GameDataManager.Instance != null)
        {
            difficultyMultiplier += (GameDataManager.Instance.currentFloor - 1) * 0.2f;
        }

        foreach (var room in rooms)
        {
            if (room.RoomRole == RoomType.Start)
                continue;

            if (room.RoomRole == RoomType.Treasure)
            {
                if (chestPrefab != null)
                {
                    Vector3 spawnPos = new Vector3(room.Center.x + 0.5f, room.Center.y + 0.5f, 0);
                    Instantiate(chestPrefab, spawnPos, Quaternion.identity, transform);

                    if (showDebugGizmos)
                        debugBlockedTiles.Add(room.Center);
                }
                continue;
            }

            if (room.RoomRole == RoomType.Exit)
            {
                if (exitPrefab != null)
                {
                    Vector3 spawnPos = new Vector3(room.Center.x + 0.5f, room.Center.y + 0.5f, 0);
                    Instantiate(exitPrefab, spawnPos, Quaternion.identity, transform);

                    if (showDebugGizmos)
                        debugBlockedTiles.Add(room.Center);
                }
                continue;
            }

            if (room.RoomRole == RoomType.Enemy || room.RoomRole == RoomType.Normal)
            {
                PlacePropsInList(room, room.CornerTiles, propsToPlace.Where(x => x.corner).ToList(), difficultyMultiplier);

                HashSet<Vector2Int> allWallTiles = new HashSet<Vector2Int>(room.NearWallTilesUp);
                allWallTiles.UnionWith(room.NearWallTilesDown);
                allWallTiles.UnionWith(room.NearWallTilesLeft);
                allWallTiles.UnionWith(room.NearWallTilesRight);

                PlacePropsInList(room, allWallTiles, propsToPlace.Where(x => x.nearWall).ToList(), difficultyMultiplier);
                PlacePropsInList(room, room.InnerTiles, propsToPlace.Where(x => x.inner).ToList(), difficultyMultiplier);
            }
        }

        if (showDebugGizmos)
        {
            foreach (var room in rooms)
            {
                CalculateFinalReachableTiles(room);
            }
        }
    }

    private void PlacePropsInList(Room room, HashSet<Vector2Int> availableTiles, List<PropData> possibleProps, float difficultyMultiplier = 1f)
    {
        if (possibleProps.Count == 0) return;

        List<Vector2Int> tempTiles = new List<Vector2Int>(availableTiles);

        foreach (var tile in tempTiles)
        {
            if (!availableTiles.Contains(tile)) continue;

            var shuffledProps = possibleProps.OrderBy(x => Random.value).ToList();

            foreach (var prop in shuffledProps)
            {
                float finalChance = Mathf.Clamp01(prop.placementChance * difficultyMultiplier);

                if (Random.value < finalChance)
                {
                    if (CanPlaceObject(tile, prop.size, availableTiles))
                    {
                        HashSet<Vector2Int> intendedTiles = GetPropOccupiedTiles(tile, prop.size);

                        if (IsPathBlocked(room, intendedTiles))
                        {
                            continue;
                        }

                        Vector3 spawnPos = new Vector3(tile.x + prop.size.x / 2f, tile.y + prop.size.y / 2f, 0);
                        Instantiate(prop.propPrefab, spawnPos, Quaternion.identity, transform);

                        if (showDebugGizmos)
                        {
                            debugBlockedTiles.UnionWith(intendedTiles);
                        }

                        MarkTilesAsOccupied(tile, prop.size, availableTiles);
                        room.FreeFloorTiles.ExceptWith(intendedTiles);

                        break;
                    }
                }
            }
        }
    }

    private bool CanPlaceObject(Vector2Int origin, Vector2Int size, HashSet<Vector2Int> availableTiles)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int currentTile = origin + new Vector2Int(x, y);
                if (!availableTiles.Contains(currentTile))
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void MarkTilesAsOccupied(Vector2Int origin, Vector2Int size, HashSet<Vector2Int> availableTiles)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                availableTiles.Remove(origin + new Vector2Int(x, y));
            }
        }
    }

    public void ClearProps()
    {
        debugReachableTiles.Clear();
        debugBlockedTiles.Clear();

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if (Application.isPlaying)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            else
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
    }

    private HashSet<Vector2Int> GetPropOccupiedTiles(Vector2Int origin, Vector2Int size)
    {
        HashSet<Vector2Int> occupied = new HashSet<Vector2Int>();
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                occupied.Add(origin + new Vector2Int(x, y));
            }
        }
        return occupied;
    }

    private bool IsPathBlocked(Room room, HashSet<Vector2Int> intendedPropTiles)
    {
        Vector2Int startNode = Vector2Int.zero;
        bool foundStart = false;

        foreach (var tile in room.FreeFloorTiles)
        {
            if (!intendedPropTiles.Contains(tile))
            {
                startNode = tile;
                foundStart = true;
                break;
            }
        }

        if (!foundStart) return false;

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue(startNode);
        visited.Add(startNode);

        int reachableTiles = 1;
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            foreach (var dir in directions)
            {
                Vector2Int neighbor = current + dir;

                if (room.FreeFloorTiles.Contains(neighbor) && !intendedPropTiles.Contains(neighbor) && !visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                    reachableTiles++;
                }
            }
        }

        int expectedFreeTiles = room.FreeFloorTiles.Count - intendedPropTiles.Count;
        return reachableTiles < expectedFreeTiles;
    }

    private void CalculateFinalReachableTiles(Room room)
    {
        if (room.FreeFloorTiles.Count == 0) return;

        Vector2Int startNode = room.Center;
        if (!room.FreeFloorTiles.Contains(startNode))
        {
            foreach (var tile in room.FreeFloorTiles)
            {
                startNode = tile;
                break;
            }
        }

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue(startNode);
        visited.Add(startNode);

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            foreach (var dir in directions)
            {
                Vector2Int neighbor = current + dir;
                if (room.FreeFloorTiles.Contains(neighbor) && !visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        debugReachableTiles.UnionWith(visited);
    }

    private void OnDrawGizmos()
    {
        if (!showDebugGizmos || !Application.isPlaying) return;

        if (debugReachableTiles != null)
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.35f);
            foreach (var tile in debugReachableTiles)
            {
                Vector3 position = new Vector3(tile.x + 0.5f, tile.y + 0.5f, 0);
                Gizmos.DrawCube(position, new Vector3(0.9f, 0.9f, 0.1f));
            }
        }

        if (debugBlockedTiles != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.65f);
            foreach (var tile in debugBlockedTiles)
            {
                Vector3 position = new Vector3(tile.x + 0.5f, tile.y + 0.5f, 0);
                Gizmos.DrawCube(position, new Vector3(0.9f, 0.9f, 0.1f));
            }
        }
    }
}
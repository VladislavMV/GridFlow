using UnityEngine;

[CreateAssetMenu(fileName = "PropData_", menuName = "DungeonGeneration/PropData")]
public class PropData : ScriptableObject
{
    public GameObject propPrefab;
    public Vector2Int size = new Vector2Int(1, 1);

    [Range(0, 1)]
    public float placementChance = 0.5f;

    [Header("Placement Rules")]
    public bool corner = false;
    public bool nearWall = false;
    public bool inner = false;
}
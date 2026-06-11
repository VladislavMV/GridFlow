using UnityEngine;

public class EnemyReward : MonoBehaviour
{
    [Header("Score Settings")]
    [SerializeField] private int scoreValue = 75; 

    public int ScoreValue => scoreValue;
}
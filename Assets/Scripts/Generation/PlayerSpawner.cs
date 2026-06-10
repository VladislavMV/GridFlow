using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private CinemachineCamera cinemachineCamera;

    [Header("Weapons Database")]
    [SerializeField] private List<WeaponData> allWeaponConfigs;

    private GameObject currentPlayer;

    public void SpawnPlayerAtPosition(Vector2Int gridPosition)
    {
        Vector3 spawnPosition = new Vector3(gridPosition.x + 0.5f, gridPosition.y + 0.5f, 0);

        if (currentPlayer == null)
        {
            currentPlayer = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

            if (GameDataManager.Instance != null && GameDataManager.Instance.currentFloor > 1)
            {
                Health h = currentPlayer.GetComponent<Health>();
                if (h != null)
                {
                    int savedHP = GameDataManager.Instance.currentHealth;
                    h.SetHealth(savedHP);
                }
            }

            if (GameDataManager.Instance != null && GameDataManager.Instance.hasSavedInventory)
            {
                PlayerShooting ps = currentPlayer.GetComponentInChildren<PlayerShooting>();
                if (ps != null)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        string savedName = GameDataManager.Instance.savedWeaponNames[i];

                        if (!string.IsNullOrEmpty(savedName))
                        {
                            WeaponData foundData = allWeaponConfigs.Find(w => w.name == savedName);

                            if (foundData != null)
                            {
                                int savedAmmo = GameDataManager.Instance.savedAmmo[i];
                                ps.LoadSavedInventory(foundData, savedAmmo, i);
                            }
                        }
                    }

                    ps.UpdateWeapon(GameDataManager.Instance.savedActiveIndex);
                }
            }

            if (cinemachineCamera != null)
            {
                cinemachineCamera.Follow = currentPlayer.transform;
            }
            else
            {
                Debug.LogWarning("Cinemachine Camera не призначена у PlayerSpawner!");
            }
        }
        else
        {
            currentPlayer.transform.position = spawnPosition;
        }
    }
}
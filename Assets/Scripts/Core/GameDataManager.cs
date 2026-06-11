using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;

    [Header("Player Stats")]
    public int currentHealth;
    public int currentFloor = 1;

    [Header("Weapon Inventory Save")]
    public string[] savedWeaponNames = new string[3];
    public int[] savedAmmo = new int[3];
    public int savedActiveIndex = 0;
    public bool hasSavedInventory = false;

    [Header("Timer Save")]
    public float savedTimer = 0f;

    [Header("Score")]
    public int currentScore = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SavePlayerData(Health playerHealth, PlayerShooting playerShooting)
    {
        currentHealth = playerHealth.CurrentHealth;
        currentFloor++;

        if (UIManager.Instance != null)
        {
            savedTimer = UIManager.Instance.GetElapsedTime();
        }

        if (playerShooting != null)
        {
            hasSavedInventory = true;
            savedActiveIndex = playerShooting.GetCurrentIndex();

            for (int i = 0; i < 3; i++)
            {
                WeaponData data = playerShooting.weaponSlots[i];
                if (data != null)
                {
                    savedWeaponNames[i] = data.name;
                    savedAmmo[i] = playerShooting.GetAmmoInSlot(i);
                }
                else
                {
                    savedWeaponNames[i] = "";
                }
            }
        }
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScoreDisplay(currentScore);
        }
    }

    public void ResetData()
    {
        currentFloor = 1;
        currentHealth = 0;
        savedTimer = 0f;
        currentScore = 0;
        hasSavedInventory = false;
        savedActiveIndex = 0;

        for (int i = 0; i < savedWeaponNames.Length; i++)
        {
            savedWeaponNames[i] = "";
            savedAmmo[i] = 0;
        }
    }
}
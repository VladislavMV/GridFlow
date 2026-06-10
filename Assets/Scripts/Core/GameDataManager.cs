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
}
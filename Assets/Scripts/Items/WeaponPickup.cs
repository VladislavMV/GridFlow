using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public WeaponData weaponToGive;
    public int savedAmmo = -1;
    private bool isPlayerInRange = false;
    private PlayerShooting playerShooting;

    private void Start()
    {
        if (savedAmmo == -1 && weaponToGive != null)
        {
            savedAmmo = weaponToGive.maxAmmo;
        }

        if (weaponToGive != null)
            GetComponent<SpriteRenderer>().sprite = weaponToGive.weaponSprite;
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (playerShooting != null)
            {
                bool hadEmptySlot = false;
                for (int i = 0; i < 2; i++)
                {
                    if (playerShooting.weaponSlots[i] == null)
                    {
                        hadEmptySlot = true;
                        break;
                    }
                }

                int ammoToGive = savedAmmo;
                int ammoFromPlayer;

                WeaponData oldWeapon = playerShooting.HandlePickup(weaponToGive, ammoToGive, out ammoFromPlayer);

                if (oldWeapon != null)
                {
                    weaponToGive = oldWeapon;
                    savedAmmo = ammoFromPlayer;
                    GetComponent<SpriteRenderer>().sprite = oldWeapon.weaponSprite;
                }
                else
                {
                    if (hadEmptySlot || (playerShooting.GetCurrentIndex() != 2))
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            playerShooting = collision.GetComponentInChildren<PlayerShooting>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerShooting = null;
        }
    }
}
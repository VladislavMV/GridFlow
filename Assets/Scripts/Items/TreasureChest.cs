using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    [Header("Loot Prefabs")]
    [SerializeField] private GameObject[] weaponPickups;
    [SerializeField] private GameObject ammoPickupPrefab;
    [SerializeField] private GameObject potionPickupPrefab;

    private bool isOpened = false;
    private bool playerInRange = false;

    private void Update()
    {
        if (playerInRange && !isOpened && Input.GetKeyDown(KeyCode.E))
        {
            OpenChest();
        }
    }

    private void OpenChest()
    {
        isOpened = true;
        GetComponent<SpriteRenderer>().color = Color.gray;

        int lootScenario = Random.Range(0, 3);

        Vector3 leftPos = transform.position + new Vector3(-0.7f, -1f, 0);
        Vector3 rightPos = transform.position + new Vector3(0.7f, -1f, 0);

        GameObject randomWeapon = weaponPickups[Random.Range(0, weaponPickups.Length)];

        switch (lootScenario)
        {
            case 0:
                Instantiate(randomWeapon, leftPos, Quaternion.identity);
                Instantiate(ammoPickupPrefab, rightPos, Quaternion.identity);
                break;
            case 1:
                Instantiate(randomWeapon, leftPos, Quaternion.identity);
                Instantiate(potionPickupPrefab, rightPos, Quaternion.identity);
                break;
            case 2:
                Instantiate(potionPickupPrefab, leftPos, Quaternion.identity);
                Instantiate(ammoPickupPrefab, rightPos, Quaternion.identity);
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) playerInRange = false;
    }
}
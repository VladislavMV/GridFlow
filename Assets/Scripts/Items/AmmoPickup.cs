using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [SerializeField] private int ammoAmount = 15;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerShooting shooting = collision.GetComponentInChildren<PlayerShooting>();

            if (shooting != null)
            {
                shooting.AddAmmo(ammoAmount);
                Destroy(gameObject);
            }
        }
    }
}
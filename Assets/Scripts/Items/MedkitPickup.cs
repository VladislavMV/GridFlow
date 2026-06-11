using UnityEngine;

public class MedkitPickup : MonoBehaviour
{
    [SerializeField] private int healAmount = 25;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.Heal(healAmount);

                if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("Pickup_medkit");

                Destroy(gameObject);
            }
        }
    }
}
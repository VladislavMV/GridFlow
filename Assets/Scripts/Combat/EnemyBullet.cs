using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private int damage = 10;

    [Header("Visual Effects")]
    [Tooltip("Префаб ефекту розриву кулі (4 спрайти)")]
    [SerializeField] private GameObject impactEffectPrefab;

    private void Start()
    {
        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Vector2 impactPoint = collision.ClosestPoint(transform.position);

        if (collision.CompareTag("Player") || collision.CompareTag("Prop"))
        {
            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            if (impactEffectPrefab != null)
            {
                Instantiate(impactEffectPrefab, impactPoint, transform.rotation);
            }

            Destroy(gameObject);
            return;
        }

        if (collision.CompareTag("Pickup") || collision.CompareTag("Enemy") || collision.CompareTag("Bullet"))
        {
            return;
        }

        if (impactEffectPrefab != null)
        {
            Instantiate(impactEffectPrefab, impactPoint, transform.rotation);
        }

        Destroy(gameObject);
    }
}
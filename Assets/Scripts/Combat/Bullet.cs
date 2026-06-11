using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private int damage = 20;

    [Header("Visual Effects")]
    [Tooltip("Префаб ефекту розриву кулі")]
    [SerializeField] private GameObject impactEffectPrefab;

    private void Start()
    {
        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Prop"))
        {
            Health enemyHealth = collision.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }

        if (!collision.CompareTag("Player") && !collision.CompareTag("Bullet"))
        {
            Vector2 impactPoint = collision.ClosestPoint(transform.position);

            if (impactEffectPrefab != null)
            {
                Instantiate(impactEffectPrefab, impactPoint, transform.rotation);
            }

            Destroy(gameObject);
        }
    }
}
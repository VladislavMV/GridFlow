using UnityEngine;

public class RangedEnemyAI : MonoBehaviour
{
    [Header("Detection & Movement")]
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float speed = 2.5f;
    [SerializeField] private float stoppingDistance = 6f;
    [SerializeField] private float retreatDistance = 4f;

    [Header("Shooting")]
    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private Transform gunPivot;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletForce = 15f;
    [SerializeField] private float fireRate = 1f;

    [Header("Weapon Type Settings")]
    [SerializeField] private int bulletsPerShot = 1;
    [SerializeField] private float spreadAngle = 0f;

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private float nextFireTime;
    private bool hasDetectedPlayer = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        Vector2 direction = (player.position - transform.position).normalized;

        if (!hasDetectedPlayer)
        {
            if (distance <= detectionRadius)
            {
                hasDetectedPlayer = true;
            }
            else
            {
                SetMoving(false);
                return;
            }
        }

        if (distance > stoppingDistance)
        {
            rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
            SetMoving(true);
        }
        else if (distance < retreatDistance)
        {
            rb.MovePosition(rb.position - direction * speed * Time.fixedDeltaTime);
            SetMoving(true);
        }
        else
        {
            SetMoving(false);
        }

        GetComponent<SpriteRenderer>().flipX = direction.x < 0;

        if (gunPivot != null)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            gunPivot.rotation = Quaternion.Euler(0, 0, angle);

            if (angle > 90 || angle < -90) gunPivot.localScale = new Vector3(1, -1, 1);
            else gunPivot.localScale = new Vector3(1, 1, 1);
        }

        if (distance <= stoppingDistance && Time.time >= nextFireTime)
        {
            Shoot(direction);
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    private void Shoot(Vector2 direction)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("Enemy_Shoot");
        }

        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        for (int i = 0; i < bulletsPerShot; i++)
        {
            float randomSpread = Random.Range(-spreadAngle, spreadAngle);
            Quaternion bulletRotation = Quaternion.Euler(0, 0, baseAngle + randomSpread);

            GameObject bullet = Instantiate(enemyBulletPrefab, firePoint.position, bulletRotation);

            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                bulletRb.AddForce(bullet.transform.right * bulletForce, ForceMode2D.Impulse);
            }
        }
    }

    private void SetMoving(bool isMoving)
    {
        if (animator != null) animator.SetBool("isMoving", isMoving);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, retreatDistance);
    }
}
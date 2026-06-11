using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float detectionRange = 5f;

    [Header("Attack")]
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackRate = 1f;

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private float nextAttackTime = 0f;

    private bool isAttacking = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }
    private void ResetAttack()
    {
        isAttacking = false;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void FixedUpdate()
    {
        if (player == null)
        {
            SetMoving(false);
            return;
        }

        if (isAttacking)
        {
            SetMoving(false);
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            SetMoving(false);

            if (Time.time >= nextAttackTime)
            {
                AttackPlayer();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
        else if (distanceToPlayer < detectionRange)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);

            GetComponent<SpriteRenderer>().flipX = direction.x < 0;
            SetMoving(true);
        }
        else
        {
            SetMoving(false);
        }
    }

    private void AttackPlayer()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        Invoke("ResetAttack", 1f / attackRate);
    }

    private void SetMoving(bool moving)
    {
        if (animator != null)
        {
            animator.SetBool("isMoving", moving);
        }
    }

    public void DealDamage()
    {
        if (player != null && Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null) playerHealth.TakeDamage(attackDamage);
        }
    }

    private void OnDisable()
    {
        CancelInvoke();
        isAttacking = false;

        if (rb != null) rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
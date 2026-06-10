using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [Tooltip("На скільки збільшувати HP ворогів за кожен поверх")]
    [SerializeField] private int healthPerFloor = 20;

    private int currentHealth;
    private int currentMaxHealth;
    private Animator anim;
    private bool isDead = false;

    public int CurrentHealth => currentHealth;

    private void Start()
    {
        anim = GetComponent<Animator>();
        if (anim == null) anim = GetComponentInChildren<Animator>();

        int floor = 1;
        if (GameDataManager.Instance != null) floor = GameDataManager.Instance.currentFloor;

        if (CompareTag("Player"))
        {
            currentMaxHealth = maxHealth;
            if (currentHealth <= 0) currentHealth = currentMaxHealth;
            UIManager.Instance.UpdateHealth(currentHealth, currentMaxHealth);
        }
        else if (CompareTag("Prop"))
        {
            currentMaxHealth = maxHealth;
            currentHealth = currentMaxHealth;
        }
        else
        {
            currentMaxHealth = maxHealth + (floor - 1) * healthPerFloor;
            currentHealth = currentMaxHealth;
        }
    }

    public void SetHealth(int healthAmount)
    {
        currentHealth = healthAmount;
        if (currentHealth <= 0) currentHealth = 1;

        if (CompareTag("Player"))
        {
            UIManager.Instance.UpdateHealth(currentHealth, maxHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (CompareTag("Player"))
        {
            UIManager.Instance.UpdateHealth(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > currentMaxHealth) currentHealth = currentMaxHealth;

        if (CompareTag("Player"))
        {
            UIManager.Instance.UpdateHealth(currentHealth, maxHealth);
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (CompareTag("Prop"))
        {
            if (anim != null) anim.SetTrigger("Destroy");

            AudioManager.Instance.PlaySFX("PropDestroy");
            DisableComponents();

            Destroy(gameObject, 0.5f);
            return;
        }

        if (anim != null) anim.SetTrigger("Die");

        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null) pc.DisableController();

        PlayerShooting ps = GetComponentInChildren<PlayerShooting>();
        if (ps != null)
        {
            ps.DisableShooting();
            ps.gameObject.SetActive(false);
        }

        Transform enemyGun = transform.Find("GunPivot");
        if (enemyGun != null) enemyGun.gameObject.SetActive(false);

        MonoBehaviour ai = GetComponent<RangedEnemyAI>();
        if (ai == null) ai = GetComponent<EnemyAI>();
        if (ai != null) ai.enabled = false;

        DisableComponents();

        if (!CompareTag("Player"))
        {
            Destroy(gameObject, 3f);
        }
        else
        {
            UIManager.Instance.StopTimer();
            Invoke(nameof(TriggerGameOverUI), 1.5f);
        }
    }

    private void DisableComponents()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }
    }

    private void TriggerGameOverUI()
    {
        UIManager.Instance.ShowGameOverScreen();
    }
}
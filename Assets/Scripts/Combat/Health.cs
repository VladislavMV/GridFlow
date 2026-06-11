using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [Tooltip("На скільки збільшувати HP ворогів за кожен поверх")]
    [SerializeField] private int healthPerFloor = 20;

    [Header("Visual Effects")]
    [SerializeField] private GameObject destructionEffectPrefab;

    private int currentHealth;
    private int currentMaxHealth;
    private Animator anim;
    private bool isDead = false;
    private DamageFlash damageFlash;

    public int CurrentHealth => currentHealth;
    public int CurrentMaxHealth => currentMaxHealth;

    private void Start()
    {
        anim = GetComponent<Animator>();
        if (anim == null) anim = GetComponentInChildren<Animator>();

        int floor = 1;
        if (GameDataManager.Instance != null) floor = GameDataManager.Instance.currentFloor;

        damageFlash = GetComponent<DamageFlash>();

        if (CompareTag("Player"))
        {
            currentMaxHealth = maxHealth;
            if (currentHealth <= 0) currentHealth = currentMaxHealth;
            if (UIManager.Instance != null) UIManager.Instance.UpdateHealth(currentHealth, currentMaxHealth);
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

        if (CompareTag("Player") && UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealth(currentHealth, currentMaxHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth < 0) currentHealth = 0;

        if (damageFlash != null) damageFlash.PlayFlash();

        if (AudioManager.Instance != null)
        {
            if (CompareTag("Player")) AudioManager.Instance.PlaySFX("Hit_Player");
            else if (CompareTag("Enemy")) AudioManager.Instance.PlaySFX("Hit_Enemy");
            else if (CompareTag("Prop")) AudioManager.Instance.PlaySFX("Hit_Prop");
        }

        if (CompareTag("Player") && UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealth(currentHealth, currentMaxHealth);
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

        if (CompareTag("Player") && UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealth(currentHealth, currentMaxHealth);
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (destructionEffectPrefab != null)
        {
            Instantiate(destructionEffectPrefab, transform.position, Quaternion.identity);
        }

        if (CompareTag("Enemy"))
        {
            if (TryGetComponent<EnemyReward>(out EnemyReward reward))
            {
                if (GameDataManager.Instance != null)
                {
                    GameDataManager.Instance.AddScore(reward.ScoreValue);
                }
            }
        }

            if (CompareTag("Prop"))
        {
            if (anim != null) anim.SetTrigger("Destroy");

            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("Destroy_Prop");

            DisableComponents();

            Destroy(gameObject);
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
            if (UIManager.Instance != null) UIManager.Instance.StopTimer();
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
        if (UIManager.Instance != null) UIManager.Instance.ShowGameOverScreen();
    }
}
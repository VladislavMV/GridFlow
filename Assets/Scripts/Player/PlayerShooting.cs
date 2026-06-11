using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Inventory (0: Main, 1: Sub, 2: Melee)")]
    public WeaponData[] weaponSlots = new WeaponData[3];
    private WeaponData currentWeapon;
    private int[] ammoStorage = new int[3];
    private int currentWeaponIndex = 0;
    private bool isDead = false;

    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private SpriteRenderer gunRenderer;
    [SerializeField] private Animator gunAnimator;

    private Camera mainCam;
    private float nextFireTime;

    public int GetCurrentIndex() => currentWeaponIndex;
    public int GetAmmoInSlot(int index) => ammoStorage[index];
    public void DisableShooting() => isDead = true;

    private void Start()
    {
        mainCam = Camera.main;

        for (int i = 0; i < 2; i++)
        {
            if (weaponSlots[i] != null && ammoStorage[i] == 0)
            {
                ammoStorage[i] = weaponSlots[i].maxAmmo;
            }
        }

        ammoStorage[2] = 999;

        if (weaponSlots[currentWeaponIndex] != null)
        {
            UpdateWeapon(currentWeaponIndex);
        }
    }

    private void Update()
    {
        if (isDead) return;

        if (Time.timeScale == 0f) return;

        HandleRotation();
        HandleWeaponSwitch();
        HandleShooting();
    }

    private void HandleRotation()
    {
        if (mainCam == null) return;
        Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDir = mousePos - transform.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        transform.localScale = (angle > 90 || angle < -90) ? new Vector3(1, -1, 1) : new Vector3(1, 1, 1);
    }

    private void HandleWeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && weaponSlots[0] != null) UpdateWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2) && weaponSlots[1] != null) UpdateWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3) && weaponSlots[2] != null) UpdateWeapon(2);
    }

    public void UpdateWeapon(int index)
    {
        currentWeaponIndex = index;
        currentWeapon = weaponSlots[currentWeaponIndex];
        if (gunRenderer != null) gunRenderer.sprite = currentWeapon.weaponSprite;
        if (firePoint != null) firePoint.localPosition = currentWeapon.firePointOffset;

        UIManager.Instance.UpdateWeapon(currentWeapon.weaponUIIcon, ammoStorage[currentWeaponIndex], currentWeapon.isMelee);
    }

    private void HandleShooting()
    {
        if (currentWeapon == null) return;
        bool isFiring = currentWeapon.isAutomatic ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);

        if (isFiring && Time.time >= nextFireTime)
        {
            if (currentWeapon.isMelee)
            {
                MeleeAttack();
                nextFireTime = Time.time + (1f / currentWeapon.fireRate);
            }
            else if (ammoStorage[currentWeaponIndex] > 0)
            {
                Shoot();
                ammoStorage[currentWeaponIndex]--;
                UIManager.Instance.UpdateAmmoOnly(ammoStorage[currentWeaponIndex]);
                nextFireTime = Time.time + (1f / currentWeapon.fireRate);
            }
        }
    }

    private void Shoot()
    {
        if (currentWeapon != null && currentWeapon.shootSound != null)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayWeaponSFX(currentWeapon.shootSound);
            }
        }
        float baseAngle = firePoint.rotation.eulerAngles.z;
        for (int i = 0; i < currentWeapon.bulletsPerShot; i++)
        {
            float randomSpread = Random.Range(-currentWeapon.spreadAngle, currentWeapon.spreadAngle);
            Quaternion bulletRotation = Quaternion.Euler(0, 0, baseAngle + randomSpread);
            GameObject bullet = Instantiate(currentWeapon.bulletPrefab, firePoint.position, bulletRotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null) rb.AddForce(bullet.transform.right * currentWeapon.bulletForce, ForceMode2D.Impulse);
        }
    }

    private void MeleeAttack()
    {
        if (gunAnimator != null) gunAnimator.SetTrigger("MeleeAttack");

        float attackOffset = 0.5f;
        Vector3 attackPosition = firePoint.position + transform.right * attackOffset;

        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPosition, currentWeapon.meleeRadius);
        foreach (Collider2D obj in hitObjects)
        {
            if (obj.CompareTag("Enemy") || obj.CompareTag("Prop"))
            {
                Health objectHealth = obj.GetComponent<Health>();
                if (objectHealth != null) objectHealth.TakeDamage(currentWeapon.damage);
            }
        }
    }

    public void AddAmmo(int amount)
    {
        int indexToAdd = (currentWeaponIndex == 2) ? 0 : currentWeaponIndex;
        if (weaponSlots[indexToAdd] != null)
        {
            ammoStorage[indexToAdd] = Mathf.Min(ammoStorage[indexToAdd] + amount, weaponSlots[indexToAdd].maxAmmo);
            if (indexToAdd == currentWeaponIndex) UIManager.Instance.UpdateAmmoOnly(ammoStorage[currentWeaponIndex]);
        }
    }

    public WeaponData HandlePickup(WeaponData newWeapon, int ammoInPickup, out int ammoToReturn)
    {
        ammoToReturn = 0;

        for (int i = 0; i < 2; i++)
        {
            if (weaponSlots[i] == null)
            {
                weaponSlots[i] = newWeapon;
                ammoStorage[i] = ammoInPickup;
                UpdateWeapon(i);
                return null;
            }
        }

        if (currentWeaponIndex == 2)
        {
            Debug.Log("Всі слоти зайняті! Візьміть у руки зброю, яку хочете замінити.");
            return null;
        }

        WeaponData oldWeapon = weaponSlots[currentWeaponIndex];
        ammoToReturn = ammoStorage[currentWeaponIndex];

        weaponSlots[currentWeaponIndex] = newWeapon;
        ammoStorage[currentWeaponIndex] = ammoInPickup;

        UpdateWeapon(currentWeaponIndex);
        return oldWeapon;
    }

    public void LoadSavedInventory(WeaponData data, int ammo, int index)
    {
        weaponSlots[index] = data;
        ammoStorage[index] = ammo;
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint != null && currentWeapon != null && currentWeapon.isMelee)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(firePoint.position, currentWeapon.meleeRadius);
        }
    }
}
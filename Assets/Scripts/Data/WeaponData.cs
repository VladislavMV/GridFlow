using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon System/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName;

    [Header("Firing Settings")]
    public float fireRate = 0.5f;
    public float bulletForce = 20f;
    public int damage = 20;
    public bool isAutomatic = false;

    [Header("Audio Settings")]
    public AudioClip shootSound;
    [Range(0f, 1f)] public float volume = 1f;

    [Header("Shotgun Settings")]
    public int bulletsPerShot = 1;
    public float spreadAngle = 0f;

    [Header("Ammo Settings")]
    public int maxAmmo = 30;

    [Header("Visuals")]
    public GameObject bulletPrefab;
    public Sprite weaponSprite;
    public Sprite weaponUIIcon;
    public Vector2 firePointOffset;

    [Header("Melee Settings")]
    public bool isMelee = false;
    public float meleeRadius = 1f;
}
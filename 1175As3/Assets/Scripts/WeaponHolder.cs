using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    public Transform firePoint;               // Empty GameObject at the muzzle
    public GameObject bulletPrefab;           // Assign your bullet prefab here
    public SpriteRenderer weaponSpriteRenderer; // The weapon's sprite renderer

    public bool facingRight = true;            // Track player facing direction (default right)

    private WeaponData selectedWeapon;
    private float fireCooldown = 0f;

    void Start()
    {
        int index = PlayerPrefs.GetInt("SelectedWeaponIndex", 0);

        WeaponLoader loader = Object.FindFirstObjectByType<WeaponLoader>();
        if (loader != null && loader.weaponList != null && loader.weaponList.weapons.Length > index)
        {
            selectedWeapon = loader.weaponList.weapons[index];

            if (selectedWeapon != null)
            {
                weaponSpriteRenderer.sprite = selectedWeapon.weaponSprite;
            }
            else
            {
                Debug.LogError("WeaponHolder: Selected weapon is null!");
            }
        }
        else
        {
            Debug.LogError("WeaponHolder: Could not find WeaponLoader or weapon data.");
        }
    }

    void Update()
    {
        if (selectedWeapon == null)
            return;

        fireCooldown -= Time.deltaTime;

        // Shoot on left mouse button held down
        if (Input.GetMouseButton(0) && fireCooldown <= 0f)
        {
            Shoot();
            fireCooldown = selectedWeapon.fireRate;
        }
    }

    public void SetFacingDirection(bool isFacingRight)
    {
        facingRight = isFacingRight;

        // Flip the weapon sprite horizontally by scaling X
        weaponSpriteRenderer.flipX = !facingRight;

        // Optionally flip firePoint position if needed
        Vector3 localPos = firePoint.localPosition;
        localPos.x = Mathf.Abs(localPos.x) * (facingRight ? 1 : -1);
        firePoint.localPosition = localPos;
    }

    void Shoot()
    {
        int bullets = selectedWeapon.bulletsPerShot;
        float spread = selectedWeapon.spreadAngle;

        for (int i = 0; i < bullets; i++)
        {
            float angleOffset = (bullets > 1) ? Random.Range(-spread / 2f, spread / 2f) : 0f;

            Quaternion baseRotation = facingRight
                ? firePoint.rotation
                : Quaternion.Euler(0, 0, 180) * firePoint.rotation;

            Quaternion rotation = baseRotation * Quaternion.Euler(0, 0, angleOffset);

            Instantiate(bulletPrefab, firePoint.position, rotation);
        }
    }
}

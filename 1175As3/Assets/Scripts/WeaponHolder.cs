using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public SpriteRenderer weaponSpriteRenderer;

    // This 'facingRight' will be updated by your PlayerMovement script
    // when the player's visual direction changes.
    public bool facingRight = true;

    private WeaponData selectedWeapon;
    private float fireCooldown = 0f;

    void Start()
    {
        WeaponLoader loader = Object.FindFirstObjectByType<WeaponLoader>();
        if (loader == null)
        {
            Debug.LogError("[ERROR] WeaponHolder: WeaponLoader not found in the scene. Weapon system will not function.");
            return;
        }
        Debug.Log("[DEBUG] WeaponHolder: Found WeaponLoader.");

        if (loader.weaponList == null || loader.weaponList.weapons == null || loader.weaponList.weapons.Length == 0)
        {
            Debug.LogError("[ERROR] WeaponHolder: WeaponLoader reports no weapon data loaded. Check WeaponLoader configuration and script execution order.");
            return;
        }
        Debug.Log($"[DEBUG] WeaponHolder: WeaponLoader has {loader.weaponList.weapons.Length} weapons loaded.");

        int index = PlayerPrefs.GetInt("SelectedWeaponIndex", 0); // Default to 0 if not found
        Debug.Log($"[DEBUG] WeaponHolder: Retrieved index '{index}' from PlayerPrefs 'SelectedWeaponIndex'.");

        if (index < 0 || index >= loader.weaponList.weapons.Length)
        {
            Debug.LogWarning($"[WARNING] WeaponHolder: Invalid SelectedWeaponIndex ({index}). Defaulting to weapon at index 0.");
            index = 0; // Fallback to the first weapon
        }

        selectedWeapon = loader.weaponList.weapons[index];

        if (selectedWeapon != null)
        {
            Debug.Log($"[DEBUG] WeaponHolder: Successfully assigned selected weapon: {selectedWeapon.weaponName}");

            if (weaponSpriteRenderer != null)
            {
                weaponSpriteRenderer.sprite = selectedWeapon.weaponSprite;
                if (selectedWeapon.weaponSprite == null)
                {
                    Debug.LogWarning($"[WARNING] WeaponHolder: Selected weapon '{selectedWeapon.weaponName}' has a NULL sprite. Check WeaponLoader's sprite assignments and CSV spriteName for this weapon.");
                }
                else
                {
                    Debug.Log($"[DEBUG] WeaponHolder: Assigned sprite '{selectedWeapon.weaponSprite.name}' to WeaponSpriteRenderer.");
                }
            }
            else
            {
                Debug.LogError("[ERROR] WeaponHolder: weaponSpriteRenderer is NULL in the Inspector. Cannot display weapon sprite.");
            }
        }
        else
        {
            Debug.LogError($"[ERROR] WeaponHolder: selectedWeapon is NULL after assignment at index {index}. This indicates a critical data loading issue.");
        }
    }

    void Update()
    {
        if (selectedWeapon == null)
        {
            return;
        }

        fireCooldown -= Time.deltaTime;

        // Assuming right mouse button (1) for shooting as per your previous request
        if (Input.GetMouseButton(1) && fireCooldown <= 0f)
        {
            Debug.Log("[DEBUG] WeaponHolder: Right mouse button pressed and cooldown ready. Attempting to shoot.");
            Shoot();
            fireCooldown = selectedWeapon.fireRate;
        }
    }

    // This method is called by your PlayerMovement script to flip the weapon's visual and firePoint position.
    // Ensure your PlayerMovement script is calling this!
    public void SetFacingDirection(bool isFacingRight)
    {
        facingRight = isFacingRight;

        // Flips the weapon's visual sprite
        if (weaponSpriteRenderer != null)
        {
            weaponSpriteRenderer.flipX = !facingRight; // Assuming weapon sprite faces right by default
        }
        else
        {
            Debug.LogWarning("WeaponHolder: weaponSpriteRenderer is null when trying to flip. Assign it in Inspector!");
        }

        // Adjusts the fire point's LOCAL position so it's always on the "front" side
        if (firePoint != null)
        {
            Vector3 localPos = firePoint.localPosition;
            localPos.x = Mathf.Abs(localPos.x) * (facingRight ? 1 : -1);
            firePoint.localPosition = localPos;
        }
        else
        {
            Debug.LogWarning("WeaponHolder: firePoint is null when trying to set local position. Assign it in Inspector!");
        }
    }

    void Shoot()
    {
        if (selectedWeapon == null || bulletPrefab == null || firePoint == null)
        {
            Debug.LogError("[ERROR] WeaponHolder: Missing references for shooting (weapon, bulletPrefab, or firePoint).");
            return;
        }

        int bullets = selectedWeapon.bulletsPerShot;
        float spread = selectedWeapon.spreadAngle;

        // --- CRITICAL LOGIC FOR BULLET DIRECTION ---
        float horizontalInput = Input.GetAxisRaw("Horizontal"); // Get raw input to avoid smoothing

        Vector3 baseDirection;

        if (horizontalInput > 0.1f) // Check for a small positive value (moving right)
        {
            baseDirection = Vector3.right;
            Debug.Log($"[WeaponHolder] Determined baseDirection: RIGHT (from horizontal input: {horizontalInput})");
        }
        else if (horizontalInput < -0.1f) // Check for a small negative value (moving left)
        {
            baseDirection = Vector3.left;
            Debug.Log($"[WeaponHolder] Determined baseDirection: LEFT (from horizontal input: {horizontalInput})");
        }
        else
        {
            // If no significant horizontal input, fall back to the player's last known facing direction.
            // This 'facingRight' variable MUST be correctly updated by your PlayerMovement script.
            baseDirection = facingRight ? Vector3.right : Vector3.left;
            Debug.Log($"[WeaponHolder] Determined baseDirection: {baseDirection} (from facingRight, no significant horizontal input. facingRight: {facingRight})");
        }
        // --- END CRITICAL LOGIC ---

        for (int i = 0; i < bullets; i++)
        {
            float angleOffset = 0f;
            if (bullets > 1)
            {
                angleOffset = Random.Range(-spread / 2f, spread / 2f);
            }

            Quaternion spreadRotation = Quaternion.Euler(0, 0, angleOffset);
            Vector3 finalBulletDirection = spreadRotation * baseDirection;

            Debug.Log($"[WeaponHolder] Bullet {i} finalBulletDirection calculated as: {finalBulletDirection}");

            Quaternion bulletInitialRotation = Quaternion.FromToRotation(Vector3.right, finalBulletDirection);

            GameObject newBullet = Instantiate(bulletPrefab, firePoint.position, bulletInitialRotation);
            Debug.Log($"[WeaponHolder] Instantiated new bullet: {newBullet.name}");

            Bullet bulletScript = newBullet.GetComponent<Bullet>();

            if (bulletScript != null)
            {
                bulletScript.SetDirection(finalBulletDirection); // Set direction first
                bulletScript.Initialize(selectedWeapon.damage); // Then initialize
            }
            else
            {
                Debug.LogError("[ERROR] WeaponHolder: Instantiated bullet prefab does not have a 'Bullet' script!");
            }
        }
    }
}
using UnityEditor.U2D.Animation;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    private SpriteRenderer sr;
    private Transform weaponTransform;

    public string weaponName;
    public float damage;
    public float fireRate;
    public int bulletsPerShot;
    public float spreadAngle;

    public GameObject bulletPrefab;
    public Transform firePoint;

    public Player player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        player = GetComponentInParent<Player>();
        weaponTransform = GetComponent<Transform>();

        WeaponData weaponData = SelectedWeapon.instance.selectedWeapon;

        if (weaponData == null)
        {
            Debug.LogError("No selected weapon found");
        }

        weaponName = weaponData.weaponName;
        damage = weaponData.damage;
        fireRate = weaponData.fireRate;
        bulletsPerShot = weaponData.bulletsPerShot;
        spreadAngle = weaponData.spreadAngle;

        sr.sprite = weaponData.weaponSprite;

    }

    // Update is called once per frame
    void Update()
    {
        WeaponUI();
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void WeaponUI()
    {

        if (player.moveDir.x < -0.01f) // walking left
        {
            weaponTransform.localRotation = Quaternion.Euler(0, 0, 90);
        }
        else if (player.moveDir.x > 0.01f) // walking right
        {
            weaponTransform.localRotation = Quaternion.Euler(0, 0, -90);
        }
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }


}

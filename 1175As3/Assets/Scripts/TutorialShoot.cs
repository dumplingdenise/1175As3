using UnityEngine;

public class TutorialShoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left-click
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}

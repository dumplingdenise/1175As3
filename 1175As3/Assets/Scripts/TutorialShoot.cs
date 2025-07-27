using UnityEngine;

public class TutorialShoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;

    public SpriteRenderer weaponSprite;

    public bool canShoot = false;

    private TutorialCharMovement playerScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        playerScript = GetComponent<TutorialCharMovement>();
        weaponSprite = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        if (!canShoot)
        {
            return;
        }
        if (DialogueManager.instance != null && DialogueManager.instance.isDialogueOpen()) // prevent shooting when dialog is open
        {
            return;
        }
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

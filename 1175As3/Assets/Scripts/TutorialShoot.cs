using UnityEngine;

public class TutorialShoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;

    /*public SpriteRenderer weaponSprite;*/
    public Transform weapon;

    public bool canShoot = false;

    private TutorialCharMovement playerScript;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        playerScript = GetComponent<TutorialCharMovement>();
        /*weaponSprite = GetComponentInChildren<SpriteRenderer>();*/
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

        HandleFlip();

        /*if (Input.GetKeyDown(KeyCode.Space)) // Left-click
        {
            Shoot();
        }*/
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }
    void HandleFlip()
    {
        if (playerScript.input.x < -0.01f)
        {
            weapon.localRotation = Quaternion.Euler(0, 0, 90);
        }
        else if (playerScript.input.x > 0.01f)
        {
            weapon.localRotation = Quaternion.Euler(0, 0, -90);
        }
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}

// denise
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private SpriteRenderer sr;
    private BoxCollider2D bc;

    // player stats
    public string characterName;
    public float movementSpeed;
    public int maxHealth;
    public int maxArmorRating;

    public int currentHealth;
    public int curArmorRating;

    // for playermovement
    private Vector2 moveDir;
    private Rigidbody2D rb;

    //walking animation    
    private Animator animator; // get reference to animator
    public RuntimeAnimatorController baseController;

    // ref gameUI
    private GameUIManager gameUIManager;

    private WeaponHolder playerWeaponHolder;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        gameUIManager = FindFirstObjectByType<GameUIManager>();

        // update player stats to stats of the selected character
        Characters.Character characterData = SelectedCharacterManager.instance.selectedCharacter;

        if (characterData == null)
        {
            Debug.LogError("No selected character found");
        }

        characterName = characterData.characterName;
        movementSpeed = characterData.movementSpeed;
        maxHealth = characterData.maxHealth;
        maxArmorRating = characterData.armorRating;

        currentHealth = maxHealth;
        curArmorRating = maxArmorRating;

        sr.sprite = characterData.defaultCharacterSprite;

        bc.size = sr.sprite.bounds.size; // change the size of the box collider to match the size of the character sprite

        // Override the animation clips
        AnimatorOverrideController overrideController = new AnimatorOverrideController(baseController);
        overrideController["Idle"] = characterData.idleAnimation;
        overrideController["Run"] = characterData.runAnimation;
        animator.runtimeAnimatorController = overrideController;

        // Ensure WeaponHolder knows initial facing direction (defaulting to true)
        if (playerWeaponHolder != null)
        {
            playerWeaponHolder.SetFacingDirection(true); // Player starts facing right by default
        }

        /*Debug.Log("Using override controller:");
        foreach (var pair in overrideController.animationClips)
        {
            Debug.Log("Clip: " + pair.name);
        }

        Debug.Log("Base controller: " + baseController?.name);
        Debug.Log("Override idle: " + characterData.idleAnimation?.name);
        Debug.Log("Override run: " + characterData.runAnimation?.name);*/
    }

    // Update is called once per frame
    void Update()
    {
        playerMovement();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyController enemyController = collision.GetComponent<EnemyController>();

            int dmgTaken = 0;

            dmgTaken = enemyController.enemyData.contactDamage;
            int remainDmg;


            if (curArmorRating > 0)
            {
                if (dmgTaken > curArmorRating)
                {
                    remainDmg = dmgTaken - curArmorRating;
                    curArmorRating -= dmgTaken;

                    currentHealth -= remainDmg;
                }
                else
                {
                    curArmorRating -= dmgTaken;
                }
            }
            else
            {
                currentHealth -= dmgTaken;
            }

            if (currentHealth <= 0)
            {
                SceneManager.LoadScene("GameOverMenu");
            }
            gameUIManager.UpdateHealth(currentHealth, maxHealth);
            gameUIManager.UpdateArmor(curArmorRating, maxArmorRating);
        }
    }

    void playerMovement()
    {
        // get movement inputs (WASD)
        float vert = Input.GetAxisRaw("Vertical"); 
        float hori = Input.GetAxisRaw("Horizontal");

        moveDir = new Vector2(hori, vert);

        Vector2 clampedmoveDir = moveDir.normalized;
        Vector2 displacement = clampedmoveDir * movementSpeed * Time.fixedDeltaTime;
         Vector2 newPos = rb.position + displacement;
        //Vector2 newPos = rb.position + clampedmoveDir * movementSpeed * Time.fixedDeltaTime; // rb.position gets the current position of the RigidBody2d component. moveDir is the direction of movement. moveSpeed determines how fast the player moves. 
                                                                                         // moveDir * moveSpeed * Time.fixedDeltaTime calculates the changes in the position
        rb.MovePosition(newPos);

        //Track distance traveled -> dynamic data
        //only add distance is player moved,  And ensure DynamicDataManager.Instance exists before trying to use it.
        if (displacement.magnitude >0 && DynamicDataManager.Instance != null)
        {
            DynamicDataManager.Instance.AddDistanceTraveled(displacement.magnitude);

        }

        // flip sprite based on direction
        if (moveDir.x < -0.01f) // walking left
        {
            sr.flipX = true;

            // inform weaponholder to change direction for shooting
            if(playerWeaponHolder != null && playerWeaponHolder.facingRight == true) // only update if the direction actually change
            {
                playerWeaponHolder.SetFacingDirection(false);
            }
        }
        else if (moveDir.x > 0.01f) // walking right
        { 
            sr.flipX = false;

            // inform weaponholder to change direction for shooting
            if (playerWeaponHolder != null && playerWeaponHolder.facingRight == false) // only update if the direction actually change
            {
                playerWeaponHolder.SetFacingDirection(true);
            }
        }

        // for animation
        if (vert != 0 || hori != 0)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }
}

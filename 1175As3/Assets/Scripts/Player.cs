using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;

public class Player : MonoBehaviour
{
    public SpriteRenderer sr;
    public BoxCollider2D bc;

    // player stats
    public string characterName;
    public float movementSpeed;
    public int maxHealth;
    public int maxArmorRating;

    public int currentHealth;
    public int curArmorRating;

    // for playermovement
    private Vector2 moveDir;
    public Rigidbody2D rb;

    //walking animation
    private Sprite idleSprite;
    private List<Sprite> walkFrames;
    private int currentFrame = 0;
    private float frameTimer = 0f;
    public float frameRate = 0.1f; // Time between frames (adjust as needed)

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();

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

        idleSprite = characterData.defaultCharacterSprite;
        sr.sprite = idleSprite;

        if (characterData.movementSprite.ContainsKey("walk"))
        {
            walkFrames = characterData.movementSprite["walk"];
        }
        else
        {
            walkFrames = new List<Sprite>();
        }

        bc.size = sr.sprite.bounds.size; // change the size of the box collider to match the size of the character sprite


        // checking for sprites

        Debug.Log("Selected character: " + characterData.characterName);

        if (characterData.movementSprite.ContainsKey("walk"))
        {
            Debug.Log("Walk animation has " + characterData.movementSprite["walk"].Count + " frames");
        }
        else
        {
            Debug.LogWarning("No 'walk' animation found for this character!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        playerMovement();
        AnimateMovement();
    }

    void playerMovement()
    {
        // get movement inputs (WASD)
        float vert = Input.GetAxisRaw("Vertical"); 
        float hori = Input.GetAxisRaw("Horizontal");

        moveDir = new Vector2(hori, vert);

        Vector2 clampedmoveDir = moveDir.normalized;
        Vector2 newPos = rb.position + clampedmoveDir * movementSpeed * Time.fixedDeltaTime; // rb.position gets the current position of the RigidBody2d component. moveDir is the direction of movement. moveSpeed determines how fast the player moves. 
                                                                                         // moveDir * moveSpeed * Time.fixedDeltaTime calculates the changes in the position
        rb.MovePosition(newPos);

    }

    void AnimateMovement()
    {
        if (moveDir.sqrMagnitude > 0.001f && walkFrames.Count > 0)
        {
            frameTimer += Time.deltaTime;
            if (frameTimer >= frameRate)
            {
                currentFrame = (currentFrame + 1) % walkFrames.Count;
                sr.sprite = walkFrames[currentFrame];
                frameTimer = 0f;
            }

            // Flip sprite based on horizontal direction
            if (moveDir.x < -0.01f)
            {
                sr.flipX = true;
            }
            else if (moveDir.x > 0.01f)
            {
                sr.flipX = false;
            }
        }
        else
        {
            // Not moving → show idle sprite
            sr.sprite = idleSprite;
            currentFrame = 0;
            frameTimer = 0f;
        }
    }

}

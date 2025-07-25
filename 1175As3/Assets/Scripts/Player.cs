﻿using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

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

        // flip sprite based on direction
        if (moveDir.x < -0.01f) // walking left
        {
            sr.flipX = true;
        }
        else if (moveDir.x > 0.01f) // walking right
        { 
            sr.flipX = false; 
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

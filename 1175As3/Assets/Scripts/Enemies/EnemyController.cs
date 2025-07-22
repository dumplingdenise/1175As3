using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Enemy enemyData; // data loaded for this specific enemy
    private float currentHealth;
    private Transform playerTransform; // reference to the player's position

    // use layer mask to define what layers the enemy can detect for collision
    public LayerMask playerLayer;

    // initialize the enemy with its data
    public void Initialize(Enemy data)
    {
        enemyData = data;
        currentHealth = enemyData.health;
        Debug.Log($"[EnemyController] Initialized enemy: {enemyData.name} with Health: {currentHealth} and Speed: {enemyData.movementSpeed}");

        // find player object using its tag (make sure your player GameObject has the "Player" tag)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("[EnemyController] Player object not found. Enemy might not be able to track.");
        }

        // add SpriteRenderer and set sprite (simplified, you'd load from Resources/AssetBundles)
        SpriteRenderer sr = gameObject.AddComponent<SpriteRenderer>();
        // placeholder for loading sprite: assuming sprites are in resources/sprites/enemies
        // sprite enemySprite = Resources.Load<Sprite>($"Sprites/Enemies/{enemyData.spritePath}");
        /*
        if (enemySprite != null)
        {
            sr.sprite = enemySprite;
        }
        else
        {
            Debug.LogWarning($"[EnemyController] Sprite not found for path: Sprites/Enemies/{enemyData.spritePath}");
            // fallback to basic shape or default sprite
        }
        */

        // add Rigidbody2D and Collider2D for physics and collision detection
        Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0; // Top-down game, no gravity
        rb.constraints = RigidbodyConstraints2FreezeRotation; // prevent rotation

        // add collider, e.g., CircleCollider2D or BoxCollider2D
        CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
        collider.radius = 0.5f; // adjust size based on your sprite
        collider.isTrigger = false; // actual collision or just trigger?
    }

    void Update()
    {
        if (enemyData == null) return; // ensure data is loaded

        // handle movement based on the loaded data
        HandleMovement();

        // handle attack behavior (e.g., ranged attacks)
        HandleAttack();
    }

    private void HandleMovement()
    {
        switch (enemyData.movementPattern)
        {
            case "chasing_player":
                if (playerTransform != null)
                {
                    Vector2 direction = (playerTransform.position - transform.position).normalized;
                    transform.Translate(direction * enemyData.movementSpeed * Time.deltaTime);
                }
                break;
            case "straight":
                // eg: move downwards (adjust as per game's direction)
                transform.Translate(Vector2.down * enemyData.movementSpeed * Time.deltaTime);
                break;
            case "random":
                // basic random movement: continuously pick a random direction and move
                // more sophisticated would involve pathfinding or finite state machines
                // for now, make them wiggle
                transform.Translate(new Vector2(Mathf.Sin(Time.time * 2f), Mathf.Cos(Time.time * 2f)) * enemyData.movementSpeed * 0.1f * Time.deltaTime);
                break;
            default:
                // no specific movement pattern or unknown pattern
                break;
        }
    }

    private float _nextFireTime;

    public RigidbodyConstraints2D RigidbodyConstraints2FreezeRotation { get; private set; }

    private void HandleAttack()
    {
        if (enemyData.behaviour == "ranged" && enemyData.bulletFiringRate > 0)
        {
            if (Time.time >= _nextFireTime)
            {
                // implement bullet spawning/firing logic here
                Debug.Log($"[EnemyController] {enemyData.name} fired a bullet!");
                // eg instantiate a projectile prefab
                // instantiate(enemyBulletPrefab, transform.position, Quaternion.identity);
                _nextFireTime = Time.time + (1f / enemyData.bulletFiringRate);
            }
        }
    }

    // method to take damage (called by player projectiles, etc.)
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"[EnemyController] {enemyData.name} took {damage} damage. Remaining Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // method to handle enemy defeat
    private void Die()
    {
        Debug.Log($"[EnemyController] {enemyData.name} defeated!");
        // notify the WaveManager that an enemy has been defeated
        // WaveManager.Instance?.OnEnemyDefeated(); // using the singleton instance of WaveManager  -> error so commented
        Destroy(gameObject); // remove the enemy from the scene
    }

    // handle contact damage with player
    void OnCollisionEnter2D(Collision2D collision)
    {
        // make sure the Player GameObject has the "Player" tag
        if (collision.gameObject.CompareTag("Player"))
        {
            /*
            // assuming player has a component with a TakeDamage method (e.g., PlayerHealth)
            // PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(_enemyData.contactDamage);
                Debug.Log($"[EnemyController] {_enemyData.name} dealt {_enemyData.contactDamage} contact damage to Player.");
            }
            */
            Debug.Log($"[EnemyController] {enemyData.name} contacted Player.");
        }
    }
}

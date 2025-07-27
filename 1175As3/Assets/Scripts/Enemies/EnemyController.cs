using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private EnemyData enemyData; // Data loaded for this specific enemy is EnemyData
    private float currentHealth;
    private Transform playerTransform; // reference to the player's position

    // use layer mask to define what layers the enemy can detect for collision (currently unused)
    public LayerMask playerLayer;

    // reference to the sprite renderer
    private SpriteRenderer sr; // Declare it here

    void Awake()
    {
        // Get existing SpriteRenderer or add one if not present
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            sr = gameObject.AddComponent<SpriteRenderer>();
            Debug.LogWarning($"[EnemyController] Added missing SpriteRenderer to {gameObject.name}.", this);
        }
    }

    // initialize the enemy with its data
    public void Initialize(EnemyData data) // Correctly expects EnemyData
    {
        enemyData = data;
        currentHealth = enemyData.health;
        Debug.Log($"Initialized enemy: {enemyData.name} with Health: {currentHealth}");

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

        // Load and assign the sprite from the loaded EnemyData
        if (enemyData.loadedSprite != null) // Use the pre-loaded sprite from DataManager (preferred)
        {
            sr.sprite = (Sprite)enemyData.loadedSprite;
        }
        else if (!string.IsNullOrEmpty(enemyData.spritePath)) // Fallback if not pre-loaded, or if you prefer to load here
        {
            Sprite enemySprite = Resources.Load<Sprite>(enemyData.spritePath);
            if (enemySprite != null)
            {
                sr.sprite = enemySprite;
            }
            else
            {
                Debug.LogWarning($"[EnemyController] Sprite not found for path: Resources/{enemyData.spritePath} for enemy: {enemyData.name}.");
            }
        }
        else
        {
            Debug.LogWarning($"[EnemyController] No spritePath defined or loaded sprite for enemy: {enemyData.name}.");
        }
    }

    // Placeholder for enemy behavior (e.g., movement towards player)
    void Update()
    {
        if (playerTransform != null && enemyData != null)
        {
            // Example: Simple chasing behavior
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            transform.position += direction * enemyData.movementSpeed * Time.deltaTime;
        }
    }

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
        // If you had WaveManager.Instance.OnEnemyDefeated(); you would uncomment and implement it in WaveManager
        // WaveManager.Instance?.OnEnemyDefeated();
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
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(enemyData.contactDamage); // Use enemyData.contactDamage
                Debug.Log($"[EnemyController] {enemyData.name} dealt {enemyData.contactDamage} contact damage to Player.");
            }
            */
            Debug.Log($"[EnemyController] {enemyData.name} contacted Player.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet")) // Assuming player bullets have "Bullet" tag
        {
            // Example: Get damage from bullet or define here
            TakeDamage(10); // Replace with actual bullet damage if applicable
            Destroy(other.gameObject); // Destroy the bullet
        }
        else if (other.CompareTag("Player")) // If you use trigger for player contact instead of collision
        {
            Debug.Log("Enemy touched the player (via trigger)!");
            // Add damage logic here if player is a trigger or if enemy deals damage on trigger touch
            // Example: other.GetComponent<PlayerHealth>()?.TakeDamage(enemyData.contactDamage);
        }
    }
}

using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 3f;
    private float bulletDamage;

    private Vector3 moveDirection;

    public void Initialize(float damage)
    {
        bulletDamage = damage;
        Destroy(gameObject, lifetime);

        Debug.Log($"[Bullet - {gameObject.name}] Initialize called. Initial speed: {speed}, lifetime: {lifetime}. Current moveDirection (before fallback): {moveDirection}");
        if (moveDirection == Vector3.zero) // This condition will be true if SetDirection wasn't called before Initialize finishes
        {
            moveDirection = transform.right; // Fallback to bullet's local right
            Debug.Log($"[Bullet - {gameObject.name}] moveDirection was zero in Initialize, defaulted to transform.right: {moveDirection}");
        }
    }

    public void SetDirection(Vector3 direction)
    {
        if (direction == Vector3.zero) // Check if the direction being set is already zero
        {
            Debug.LogWarning($"[Bullet - {gameObject.name}] SetDirection called with a ZERO vector! This will cause the bullet to be static.");
        }
        moveDirection = direction.normalized;
        Debug.Log($"[Bullet - {gameObject.name}] SetDirection called. New moveDirection: {moveDirection}");
    }

    void Update()
    {
        // Keep this log for continuous check
        Debug.Log($"[Bullet - {gameObject.name}] Update running. moveDirection: {moveDirection}, speed: {speed}, DeltaTime: {Time.deltaTime}");
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[Bullet - {gameObject.name}] Hit: {other.name}");
        Destroy(gameObject);
    }
}
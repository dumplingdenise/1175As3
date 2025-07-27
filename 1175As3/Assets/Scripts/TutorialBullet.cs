using UnityEngine;

public class TutorialBullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }
}

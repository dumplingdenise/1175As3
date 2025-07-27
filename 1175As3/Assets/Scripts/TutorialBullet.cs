using UnityEngine;

public class TutorialBullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f;

    /*public TutorialCharMovement playerScript;*/

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        /*playerScript = GetComponent<TutorialCharMovement>();*/
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
/*        if (playerScript.input.x < -0.01f)
        {
            transform.Translate(Vector2.up * speed * Time.deltaTime);
        }
        else if (playerScript.input.x > 0.01f)
        {
            transform.Translate(Vector2.down * speed * Time.deltaTime);
        }*/
    }
}


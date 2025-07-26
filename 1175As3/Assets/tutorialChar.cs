using UnityEditor.U2D.Animation;
using UnityEngine;

public class tutorialChar : MonoBehaviour
{
    private BoxCollider2D bc;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private Vector2 moveDir;
    public int movementSpeed = 7;

    private Animator animator; // get reference to animator
    public RuntimeAnimatorController baseController;

    public AnimationClip[] animationClips;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bc = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        AnimatorOverrideController overrideController = new AnimatorOverrideController(baseController);
        overrideController["Idle"] = animationClips[0];
        overrideController["Run"] = animationClips[1];
        animator.runtimeAnimatorController = overrideController;
    }

    // Update is called once per frame
    void Update()
    {
        movement();
    }

    void movement()
    {
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

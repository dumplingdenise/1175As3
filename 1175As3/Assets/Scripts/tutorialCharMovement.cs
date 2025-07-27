using System.Collections;
using UnityEditor.U2D.Animation;
using UnityEngine;

public class tutorialCharMovement : MonoBehaviour
{
    private BoxCollider2D bc;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private Vector2 moveDir;
    private Vector2 input;
    public int movementSpeed = 7;
    private bool isMoving;

    private Animator animator; // get reference to animator
    public RuntimeAnimatorController baseController;

    public AnimationClip[] animationClips;

    public LayerMask solidObjectsLayer;
    public LayerMask interactablesLayer;

/*    private bool isNearNPC = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("NPC")) // Make sure NPCs have the tag
        {
            isNearNPC = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("NPC"))
        {
            isNearNPC = false;
        }
    }*/
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
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input != Vector2.zero)
            {
                /*var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;*/

                /*StartCoroutine(Move(targetPos));*/

                movement();
                animation();
            }
            else
            {
                animation();
            }
        }

       /* movement();
        if (!isNearNPC)
        {
            movement();
        }*/
    }



    void movement()
    {
        moveDir = input;

        Vector2 clampedmoveDir = moveDir.normalized;
        Vector2 newPos = rb.position + clampedmoveDir * movementSpeed * Time.fixedDeltaTime; // rb.position gets the current position of the RigidBody2d component. moveDir is the direction of movement. moveSpeed determines how fast the player moves. 
                                                                                             // moveDir * moveSpeed * Time.fixedDeltaTime calculates the changes in the position
        rb.MovePosition(newPos);
    }

    void animation()
    {
        // flip sprite based on direction
        if (input.x < -0.01f) // walking left
        {
            sr.flipX = true;
        }
        else if (input.x > 0.01f) // walking right
        {
            sr.flipX = false;
        }

        if (input.y != 0 || input.x != 0)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }
    /*IEnumerator Move(Vector3 targetPos)
{
    isMoving = true;
    while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, movementSpeed * Time.deltaTime);
        yield return null;
    }

    transform.position = targetPos;

    isMoving = false;
}*/

    /*    void movement()
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
        }*/
}

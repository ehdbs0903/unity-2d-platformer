using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator animator;

    public float maxSpeed = 1.0f;
    float h;
    bool horizontalReleased = false;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        //Direction Sprite
        h = Input.GetAxisRaw("Horizontal");

        if (h != 0)
        {
            spriteRenderer.flipX = (h < 0);
        }

        // Stop
        if (Input.GetButtonUp("Horizontal"))
        {
            horizontalReleased = true;
        }

        if (rigid.velocity.magnitude < 0.3f)
        {
            animator.SetBool("isWalking", false);
        }
        else
        {
            animator.SetBool("isWalking", true);
        }
    }
    void FixedUpdate()
    {
        // Stop
        if (horizontalReleased)
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
            horizontalReleased = false;
        }

        // Move
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        // Max Speed
        if (Mathf.Abs(rigid.velocity.x) > maxSpeed)
        {
            float sign = Mathf.Sign(rigid.velocity.x);
            rigid.velocity = new Vector2(maxSpeed * sign, rigid.velocity.y);
        }
    }
}
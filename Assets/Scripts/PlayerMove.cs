using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator animator;

    public float maxSpeed = 1.0f;
    public float jumpPower = 1.0f;
    public float reactPower = 1.0f;
    float h;
    bool horizontalReleased = false;
    bool jumpPressed = false;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        //Jump
        if (Input.GetButtonDown("Jump") && !animator.GetBool("isJumping"))
        {
            jumpPressed = true;
        }

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

        if (Mathf.Abs(rigid.velocity.x) < 0.3f)
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
        // Jump
        if (jumpPressed)
        {
            animator.SetBool("isJumping", true);
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            jumpPressed = false;
        }

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

        //Landing Platform
        if (rigid.velocity.y < 0) {
            Debug.DrawRay(rigid.position, Vector3.down, Color.yellow);

            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

            if (rayHit.collider != null && rayHit.distance < 0.5f)
            {
                animator.SetBool("isJumping", false);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Vector2 contactPoint = collision.contacts[0].point;
            OnDamaged(contactPoint);
        }
    }

    IEnumerator Invincibility(float duration)
    {
        gameObject.layer = 9;

        // View Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        yield return new WaitForSeconds(duration);

        gameObject.layer = 8;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    void OnDamaged(Vector2 targetPos)
    {
        // Reaction Force
        int direction = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(direction, 1) * reactPower, ForceMode2D.Impulse);

        //Animation
        animator.SetTrigger("Damaged");

        StartCoroutine(Invincibility(2.0f));
    }
}
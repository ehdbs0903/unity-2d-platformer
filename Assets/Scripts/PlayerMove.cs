using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManger gameManger;
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator animator;
    BoxCollider2D boxCollider;
    AudioSource audioSource;

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
        boxCollider = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        //Jump
        if (Input.GetButtonDown("Jump") && !animator.GetBool("isJumping"))
        {
            animator.SetBool("isJumping", true);
            PlaySound("JUMP");
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

            //Attack
            if (collision.gameObject.layer == 7 && rigid.velocity.y < 0 && transform.position.y > contactPoint.y)
            {
                OnAttack(collision.transform);
            }
            else //Damgaed
            {
                OnDamaged(contactPoint);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            // Point
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            if (isBronze)
            {
                gameManger.stagePoint += 50;
            }
            else if (isSilver)
            {
                gameManger.stagePoint += 100;
            }
            else if (isGold)
            {
                gameManger.stagePoint += 300;
            }

            // Deactive Item
            collision.gameObject.SetActive(false);
            // Sound
            PlaySound("ITEM");
        }
        else if (collision.gameObject.tag == "Finish")
        {
            // Next Stage
            gameManger.NextStage();

            // Sound
            PlaySound("FINISH");
        }
    }

    void OnAttack(Transform enemy)
    {
        // Point
        gameManger.stagePoint += 100;

        // Reaction Force
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        // Enemy Die
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();

        // Sound
        PlaySound("ATTACK");
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
        // Health Down
        gameManger.HealthDown();

        // Reaction Force
        int direction = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(direction, 1) * reactPower, ForceMode2D.Impulse);

        //Animation
        animator.SetTrigger("Damaged");
        StartCoroutine(Invincibility(2.0f));

        // Sound
        PlaySound("DAMAGED");
    }

    public void OnDie()
    {
        //Sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //Sprite Flip Y
        spriteRenderer.flipY = true;
        //Collider Disable
        boxCollider.enabled = false;
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        // Sound
        PlaySound("DIE");
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }

    void PlaySound(string action)
    {
        switch (action)
        {
            case "JUMP":
                audioSource.clip = audioJump;
                break;
            case "ATTACK":
                audioSource.clip = audioAttack;
                break;
            case "DAMAGED":
                audioSource.clip = audioDamaged;
                break;
            case "ITEM":
                audioSource.clip = audioItem;
                break;
            case "DIE":
                audioSource.clip = audioDie;
                break;
            case "FINISH":
                audioSource.clip = audioFinish;
                break;
        }
        audioSource.Play();
    }
}
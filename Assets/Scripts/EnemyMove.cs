using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator animator;
    BoxCollider2D boxCollider;

    public int nextMove;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        StartCoroutine(ThinkRoutine());
    }

    void Update()
    {
        //Direction Sprite
        if (nextMove != 0)
        {
            spriteRenderer.flipX = (nextMove > 0);
        }
    }

    void FixedUpdate()
    {
        //Move
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        //Platform Check
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.5f, rigid.position.y);

        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

        if (rayHit.collider == null)
        {
            ChangeDirection();
        }
    }

    IEnumerator ThinkRoutine()
    {
        while (true)
        {
            nextMove = Random.Range(-1, 2);
            animator.SetInteger("WalkSpeed", nextMove);
            yield return new WaitForSeconds(Random.Range(2f, 5f));
        }
    }

    void ChangeDirection()
    {
        nextMove = -nextMove;
    }

    IEnumerator DieRoutine()
    {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }

    public void OnDamaged()
    {
        //Sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //Sprite Flip Y
        spriteRenderer.flipY = true;
        //Collider Disable
        boxCollider.enabled = false;
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        //Destroy
        StartCoroutine(DieRoutine());
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator animator;

    public int nextMove;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
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
        StopAllCoroutines();
        StartCoroutine(ThinkRoutine());
    }
}

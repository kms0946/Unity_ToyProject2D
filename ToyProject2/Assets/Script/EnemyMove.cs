using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    public int nextMove;
    Animator anim;
    SpriteRenderer spriteRenderer;
    public int jumpPower;
    public int jumpCount;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        Invoke("MonsterAI", 5);
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //적 움직임
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        //지형 체크
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove*0.2f, rigid.position.y);
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 2, LayerMask.GetMask("Platform"));
        if (rayHit.collider == null)
        {
            Turn();
        }
        //이벤트 발생
        Vector2 jumpVec = new Vector2(rigid.position.x + nextMove, rigid.position.y);
        RaycastHit2D rayHit2 = Physics2D.Raycast(jumpVec, Vector3.right, 2, LayerMask.GetMask("Player"));
        Debug.DrawRay(jumpVec, Vector3.down, new Color(1, 0, 0));
        if (rayHit2.collider != null && jumpCount < 1)
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            jumpCount++;
        }
        else
            jumpCount = 0;


    }
    void MonsterAI()
    {
        nextMove = Random.Range(-1, 2);//최대값은 포함이 되지 않음. 따라서 2를 넣음
        anim.SetInteger("WalkSpeed", nextMove);// 애니메이션 적용
        //방향 전환
        if (nextMove != 0)
        {
            spriteRenderer.flipX = nextMove == 1;
        }

        //MonsterAI(); 재귀 사용시 딜레이 적용해야함. Recursive
        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("MonsterAI", nextThinkTime);//invoke를 통해 랜덤 시간의 딜레이 적용

    }

    void Turn()
    {
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == 1;
        CancelInvoke();
        Invoke("MonsterAI", 2);
    }
}

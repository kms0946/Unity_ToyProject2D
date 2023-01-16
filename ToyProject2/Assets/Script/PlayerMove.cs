using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float maxSpeed;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        //이동 중지시 멈춤
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.1f, rigid.velocity.y);
        }
        //방향 전환
        if (Input.GetButtonDown("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }
        //애니메이션 동작
        if (Mathf.Abs(rigid.velocity.x) <0.3)//절댓값 적용
        {
            anim.SetBool("isWalking", false);
        }
        else
            anim.SetBool("isWalking", true);
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");//GetAxis 적용시 위의 중지 코드가 적용 안됨
        rigid.AddForce(Vector2.right *h, ForceMode2D.Impulse);

        //Move 스피드 조절
        if (rigid.velocity.x > maxSpeed)//Right 스피드 제한
        {
            rigid.velocity = new Vector2(maxSpeed,rigid.velocity.y);
        }
        else if(rigid.velocity.x < maxSpeed * (-1))//Left 스피드 제한
        {
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
        }
    }
}

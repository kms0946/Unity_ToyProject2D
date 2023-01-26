using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gameManager;
    public float maxSpeed;
    public float jumpPower;
    public float jumpCount;

    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsuleCollider2;
    Animator anim;
    AudioSource audioSource;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsuleCollider2 = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
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

    void Update()
    {
        //점프
        if (Input.GetButtonDown("Jump")&& jumpCount<2){
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            jumpCount++;
            anim.SetBool("isJumping", true);
            PlaySound("JUMP");
        }
        //이동 중지시 멈춤
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.1f, rigid.velocity.y);
        }
        //방향 전환
        if (Input.GetButton("Horizontal"))
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

        //땅에 닿은 것을 인식 by RayCast
        //Debug.DrawRay(rigid.position, Vector3.down, new Color(1, 0, 0));
        if (rigid.velocity.y < 0)
        {
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.7f)
                {
                    anim.SetBool("isJumping", false);
                    jumpCount = 0;
                }

            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            //공격
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                Attack(collision.transform);
                gameManager.stagePoint += 100;
                PlaySound("ATTACK");

            }
            else
            {
                OnDamaged(collision.transform.position);
                PlaySound("DAMAGED");
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            if (isBronze)
                gameManager.stagePoint += 50;
            else if (isSilver)
                gameManager.stagePoint += 100;
            else if (isGold)
                gameManager.stagePoint += 200;

            collision.gameObject.SetActive(false);
            PlaySound("ITEM");
        }
        else if(collision.gameObject.tag == "Finish")
        {
            gameManager.NextStage();
            PlaySound("FINISH");
        }
    }
    void OnDamaged(Vector2 targetPos)
    {
        //레이어 변경
        gameObject.layer = 9;

        gameManager.HealthDown();
        //무적 표시
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //팅겨 나감
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc,1)*4, ForceMode2D.Impulse);

        anim.SetTrigger("doDamaged");
        Invoke("OffDamaged", 2);

    }
    void OffDamaged()
    {
        gameObject.layer = 7;
 
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }
    void Attack(Transform enemy)
    {
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        EnemyMove enemyMove= enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();

    }
    public void OnDie()
    {
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        spriteRenderer.flipY = true;
        capsuleCollider2.enabled = false;
        rigid.AddForce(Vector2.up*1.2f, ForceMode2D.Impulse);
        PlaySound("DIE");
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }
}

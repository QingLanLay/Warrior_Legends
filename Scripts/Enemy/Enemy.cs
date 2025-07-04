using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(PhysicsCheck))]
public class Enemy : MonoBehaviour{
#region 组件

    [HideInInspector] public Rigidbody2D rb;

    [HideInInspector]
    public Animator anim;

    [HideInInspector]
    public PhysicsCheck physicsCheck;

    private Character character;

#endregion


#region 基本参数

    [Header("基本参数")]
    public float normalSpeed;

    public float chaseSpeed;
    public float currentSpeed;

    public Vector3 faceDir;

    protected float maxSpeed;

    public float hurtForce;

    public Transform attacker;
    public Vector3 spwanPoint;

    [Header("检测")]
    public Vector2 centerOffset;

    public Vector2 checkSize;
    public float checkDistance;
    public LayerMask attackLayer;

    [Header("计时器")]
    public float waitTime;

    public float waitTimeCounter;
    public bool wait;

    public float lostTime;
    public float lostTimeCounter;

    [Header("状态")]
    public bool isHurt;

    public bool isDead;

    private BaseState currentState;
    protected BaseState patrolState;
    protected BaseState chaseState;
    protected BaseState skillState;
    
 

#endregion

#region 周期函数

    protected virtual void Awake(){
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentSpeed = normalSpeed;
        physicsCheck = GetComponent<PhysicsCheck>();
        character = GetComponent<Character>();
        spwanPoint = transform.position+ (Vector3)centerOffset;
    }

    private void OnEnable(){
        character.NewGame();
        currentState = patrolState;
        currentState.OnEnter(this);
    }

    private void OnDisable(){
        currentState.OnExit();
    }


    private void Update(){
        faceDir = new Vector3(-transform.localScale.x, 0, 0);

        currentState.LogicUpdate();
        TimeCounter();
    }

    private void FixedUpdate(){
        currentState.PhysicsUpdate();

        if (!isHurt && !isDead && physicsCheck.isGround){
            Move();
        }
    }

#endregion

#region 移动、计数器

    public virtual void Move(){
        //如果不是蜗牛的pre动画就移动
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("snailPreMove")
            && !anim.GetCurrentAnimatorStateInfo(0).IsName("snailRecover")){
            rb.velocity = new Vector2(currentSpeed * faceDir.x * Time.deltaTime, rb.velocity.y);

            if (rb.velocity.x < -maxSpeed){
                rb.velocity = new Vector2(-maxSpeed, rb.velocity.y);
            }
            else if (rb.velocity.x > maxSpeed){
                rb.velocity = new Vector2(maxSpeed, rb.velocity.y);
            }
        }
    }

    /// <summary>
    /// 计时器
    /// </summary>
    public void TimeCounter(){
        if (wait){
            if (FoundPlayer()){
                waitTimeCounter = 0;
            }

            waitTimeCounter -= Time.deltaTime;
            if (waitTimeCounter <= 0){
                wait = false;
                waitTimeCounter = waitTime;
                transform.localScale = new Vector3(faceDir.x, 1, 1);
            }
        }


        if (!FoundPlayer() && lostTimeCounter > 0){
            lostTimeCounter -= Time.deltaTime;
        }
    }

    public virtual bool FoundPlayer(){
        return Physics2D.BoxCast(transform.position + (Vector3)centerOffset, checkSize, 0, -faceDir, checkDistance,
            attackLayer);
    }

    public void SwitchState(NPCState state){
        var newState = state switch{
            NPCState.Patrol => patrolState,
            NPCState.Chase => chaseState,
            NPCState.Skill => skillState,
            _ => null
        };

        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter(this);
    }

#endregion


#region 事件执行

    public void OnTakeDamage(Transform attackTrans){
        attacker = attackTrans;
        // 转身
        if (attackTrans.position.x - transform.position.x > 0){
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if (attackTrans.position.x - transform.position.x < 0){
            transform.localScale = new Vector3(1, 1, 1);
        }

        // 受伤被击退
        isHurt = true;
        anim.SetTrigger("hurt");

        Vector2 dir = new Vector2(transform.position.x - attacker.transform.position.x, 0).normalized;
        rb.velocity = new Vector2(0, rb.velocity.y);

        StartCoroutine(OnHurt(dir));
    }

    private IEnumerator OnHurt(Vector2 dir){
        rb.AddForce( dir * hurtForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(character.invulnerableTime);
        isHurt = false;
    }

    public void OnDie(){
        gameObject.layer = 2;
        anim.SetBool("dead", true);
        anim.SetBool("walk",false);
        
        isDead = true;
    }

#endregion

#region Bee专属

    public virtual Vector3 GetNewPoint(){
        return transform.position+ (Vector3)centerOffset;
        
    }

#endregion
    
    

#region 销毁

    public void DestroySelfAfterAnimation(){
        Destroy(gameObject);
    }

#endregion


#region 绘制

    public virtual void OnDrawGizmosSelected(){
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(
            transform.position + new Vector3(centerOffset.x * faceDir.x, centerOffset.y, 0) +
            new Vector3(checkDistance * -faceDir.x, 0, 0), checkSize);
    }

#endregion
}
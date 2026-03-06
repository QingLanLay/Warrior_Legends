using System;
using UnityEngine;


namespace Player{
    public class PhysicsCheck : MonoBehaviour{
    #region 检测参数

        private CapsuleCollider2D coll;
        private PlayerController player;
        private Rigidbody2D rb;

        [Header("检测参数")]
        public bool manual; // 手动

        public bool isPlayer;

        public Vector2 bottomOffset; // 脚底位移插值

        public float checkRadius; // 检测半径
        public LayerMask groundLayer; // 图层

        public Vector2 leftOffset; // 左侧偏移
        public Vector2 rightOffset; // 右侧偏移

        public Enemy enemy;

    #endregion

    #region 状态

        [Header("状态")]
        public bool isGround; // 是否在地面上

        public bool touchLeftWall;
        public bool touchRightWall;

        public bool onWall;

    #endregion

    #region 周期函数

        private void Awake(){
            coll = GetComponent<CapsuleCollider2D>();
            rb = GetComponent<Rigidbody2D>();
            if (!manual){
                rightOffset = new Vector2((coll.size.x + coll.offset.x) / 2, coll.size.y / 2);
                leftOffset = new Vector2(-rightOffset.x, rightOffset.y);
            }

            if (this.GetComponent<Enemy>() is not null){
                enemy = GetComponent<Enemy>();
            }

            if (isPlayer){
                player = GetComponent<PlayerController>();
            }
        }

        private void Update(){
            Check();
        }

    #endregion

    #region 检测地面

        private void Check(){
            

            // 检测地面
            if (onWall){
                isGround = Physics2D.OverlapCircle((new Vector2(transform.position.x,transform.position.y+0.4f)), checkRadius,
                    groundLayer);
            }
            else{
                isGround = Physics2D.OverlapCircle(new Vector2(
                        transform.position.x+bottomOffset.x * this.transform.localScale.x,transform.position.y+bottomOffset.y
                        ), checkRadius,
                    groundLayer);
            }


            // 墙体判断
            touchLeftWall =
                Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, checkRadius, groundLayer);
            touchRightWall =
                Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, checkRadius, groundLayer);

            // 在墙上
            if (isPlayer){
                onWall = (touchLeftWall && player.inputDirection.x < 0f
                          || touchRightWall && player.inputDirection.x > 0f)
                         && !isGround && rb.velocity.y < 0f;
            }
        }

        /// <summary>
        /// Unity自带函数，当物体被选中时，绘制
        /// </summary>
        private void OnDrawGizmosSelected(){
            // 参数1：中心点 参数2：绘制范围
            Gizmos.DrawWireSphere(
                (new Vector2(transform.position.x+bottomOffset.x * this.transform.localScale.x,transform.position.y+bottomOffset.y))
                    ,checkRadius);
            Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, checkRadius);
            Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, checkRadius);
        }

    #endregion
    }
}
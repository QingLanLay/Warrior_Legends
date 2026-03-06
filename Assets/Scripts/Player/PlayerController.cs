using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;


namespace Player{
    public class PlayerController : MonoBehaviour{
        [FormerlySerializedAs("loadEvent")]
        [Header("监听事件")]
        public SceneLoadEventSO sceneLoadEvent;
        public VoidEventSO newGameEvent;
        public VoidEventSO backToMenuEvent;
        public VoidEventSO loadDataEvent;

        public VoidEventSO afterSceneLoaded;

        // 在创建时使用Lambda表达赋值会导致每次调用时都调用Lambda方法，这里是导致speed出问题的关键
        public PlayerInputControl inputControl;
        private Rigidbody2D rb;
        private CapsuleCollider2D coll;
        public Vector2 inputDirection;
        public Character character;
        private PhysicsCheck physicsCheck => GetComponent<PhysicsCheck>();
        private PlayerAnimation playerAnimation;
        private AudioDefination audioPlayer;

        [Header("基本参数")]
        public float speed;

        public float jumpForce;
        public float wallJumpForce;
        public float hurtForce;

        private float runSpeed;
        private float walkSpeed => speed / 2.5f;

        private Vector2 originalOffset;
        private Vector2 originalSize;

        public float slideDistance;
        public float slideSpeed;
        public int slidePowerCost;

        [Header("状态")]
        public bool isCrouch;

        public bool isHurt;
        public bool isDead;
        public bool isAttack;
        public bool isSlide;


        public bool wallJump;

        [Header("地图材质")]
        private PhysicsMaterial2D smoothMtr;

        private PhysicsMaterial2D normalMtr;

    #region 启动

        private void OnEnable(){
            sceneLoadEvent.LoadRequestEvent += OnLoadEvent;
            afterSceneLoaded.OnEventRaised += OnAfterSceneLoaded;
            loadDataEvent.OnEventRaised += OnLoadDataEvent;
            backToMenuEvent.OnEventRaised += OnAfterSceneLoaded;
        }




        private void OnDisable(){
            inputControl.Disable();
            sceneLoadEvent.LoadRequestEvent -= OnLoadEvent;
            afterSceneLoaded.OnEventRaised -= OnAfterSceneLoaded;
            loadDataEvent.OnEventRaised -= OnLoadDataEvent;
            backToMenuEvent.OnEventRaised -= OnAfterSceneLoaded;
        }

    #endregion

    #region 周期函数

        private void Awake(){
            rb = GetComponent<Rigidbody2D>();
            inputControl = new PlayerInputControl();
            inputControl.Gameplay.Jump.started += Jump;
            inputControl.Enable();
            coll = GetComponent<CapsuleCollider2D>();
            playerAnimation = GetComponent<PlayerAnimation>();
            character = GetComponent<Character>();
            smoothMtr = Resources.Load<PhysicsMaterial2D>("PhysicsMaterial/Smooth");
            normalMtr = Resources.Load<PhysicsMaterial2D>("PhysicsMaterial/Normal");
            audioPlayer = GetComponent<AudioDefination>();

            originalOffset = coll.offset;
            originalSize = coll.size;

        #region 强制走路

            // 防止runSpeed被修改
            runSpeed = speed;
            // 回调函数，将walkSpeed与speed绑定，后续按下shift会将walkSpeed赋值给speed
            // 这里类似于封装者暴露一个接口出来，让使用者写逻辑进去，然后在按键的时候调用函数
            inputControl.Gameplay.WalkButton.performed += ctx => {
                if (physicsCheck.isGround){
                    speed = walkSpeed;
                }
            };

            inputControl.Gameplay.WalkButton.canceled += ctx => {
                if (physicsCheck.isGround){
                    speed = runSpeed;
                }
            };

        #endregion

            // 攻击
            inputControl.Gameplay.Attack.started += PlayerAttack;

            // 滑铲
            inputControl.Gameplay.Slide.started += Slide;
        }


        private void Update(){
            inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();
        }

        private void FixedUpdate(){
            if (!isHurt){
                Move();
            }
        }

    #endregion

    #region 移动、跳跃等

        // 场景加载过程停止控制
        private void OnLoadEvent(GameSceneSO arg0, Vector3 arg1, bool arg2){
            inputControl.Gameplay.Disable();
            
        }

        // 加载结束之后启动控制
        private void OnAfterSceneLoaded(){
            if (GameObject.FindWithTag("SaveManager").GetComponent<SceneLoader>().ToMenu){
                inputControl.Gameplay.Disable();
                gameObject.layer = LayerMask.NameToLayer("BackToMenu");
                isDead = false;
                transform.localScale = new Vector3(1, 1, 1);
            } else{
                inputControl.Gameplay.Enable();
            }

            if (GameObject.FindWithTag("SaveManager").GetComponent<SceneLoader>().loadAfterState){
                DataManager.instance.LoadAfterScreen(GameObject.FindWithTag("SaveManager").GetComponent<SceneLoader>().GetDataID());
            }
            GameObject.FindWithTag("SaveManager").GetComponent<SceneLoader>().loadAfterState = false;
        }

        // 读取游戏进度
        private void OnLoadDataEvent(){
            isDead = false;
            inputControl.Gameplay.Enable();

        }
        
        public void Move(){
            // * Time.deltaTime ：时间修正，确保在任意机器上得到相同的位移
            // 保持原有y轴速度，重量不变
            if (!isCrouch && !isAttack && !wallJump){
                rb.AddForce(new Vector2(inputDirection.x * speed * 10 * Time.deltaTime, rb.velocity.y));

                if (rb.velocity.x < -5){
                    rb.velocity = new Vector2(-5f, rb.velocity.y);
                }
                else if (rb.velocity.x > 5){
                    rb.velocity = new Vector2(5f, rb.velocity.y);
                }
            }

            // 人物朝向
            int faceDir = (int)transform.localScale.x;
            if (inputDirection.x > 0 && !isSlide){
                faceDir = 1;
            }

            if (inputDirection.x < 0 && !isSlide){
                faceDir = -1;
            }

            // 人物翻转
            transform.localScale = new Vector3(faceDir, 1, 1);

            // 下蹲
            isCrouch = inputDirection.y <= -0.7f && physicsCheck.isGround;

            if (isCrouch){
                // 修改碰撞体大小和位移
                coll.offset = new Vector2(-0.05f, 0.85f);
                coll.size = new Vector2(0.7f, 1.7f);
            }
            else{
                // 还原之前的碰撞体
                coll.offset = originalOffset;
                coll.size = originalSize;
            }

            // 如果在空中，摩擦力改变
            if (!physicsCheck.isGround){
                rb.sharedMaterial = smoothMtr;
            }
            else{
                rb.sharedMaterial = normalMtr;
            }

            if (physicsCheck.onWall){
                // 修改碰撞体大小和位移
                coll.offset = new Vector2(-0.05f, 1.2f);
                coll.size = new Vector2(0.7f, 1.7f);
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2f);
            }

            if (wallJump && rb.velocity.y < 0){
                wallJump = false;
            }
        }

        private void Jump(InputAction.CallbackContext obj){
            // Debug.Log("JUMP");
            // 添加向上的力
            if (physicsCheck.isGround){
                rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);

                // 打断滑铲协程
                StopAllCoroutines();
                isSlide = false;
                audioPlayer.PlayAudioClip();
            } // 挂墙跳
            else if (physicsCheck.onWall){
                rb.AddForce(new Vector2(-inputDirection.x, 2f) * wallJumpForce, ForceMode2D.Impulse);
                wallJump = true;
                audioPlayer.PlayAudioClip();
            }
        }

        private void PlayerAttack(InputAction.CallbackContext obj){
            if (physicsCheck.isGround){
                playerAnimation.PlayAttack();
                isAttack = true;
            }
        }

        private void Slide(InputAction.CallbackContext obj){
            if (physicsCheck.isGround && !isSlide && character.currentPower >= slidePowerCost){
                isSlide = true;
                // Debug.Log("slide");
                float distance = 0;
                gameObject.layer = LayerMask.NameToLayer("Enemy");
                StartCoroutine(TriggerSlide(distance));
                character.OnSlide(slidePowerCost);
            }
        }

        private IEnumerator TriggerSlide(float distance){
            do{
                yield return null;
                if (!physicsCheck.isGround){
                    break;
                }

                // 滑动过程中撞墙
                if (physicsCheck.touchLeftWall && transform.localScale.x < 0f ||
                    physicsCheck.touchRightWall && transform.localScale.x > 0f){
                    isSlide = false;
                    break;
                }

                distance += slideSpeed;
                rb.MovePosition(new Vector2(transform.position.x + transform.lossyScale.x * slideSpeed,
                    transform.position.y));
                // Debug.Log(distance - slideDistance);
            } while (Mathf.Abs(distance - slideDistance) > 0.1f);

            isSlide = false;
            gameObject.layer = LayerMask.NameToLayer("Player");
        }

    #endregion

    #region UnityEvent事件

        public void GetHurt(Transform attacker){
            isHurt = true;
            rb.velocity = Vector2.zero;
            Vector2 dir = new Vector2((transform.position.x - attacker.position.x), 0).normalized;
            rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
        }

        public void PlayerDead(){
            isDead = true;
            this.gameObject.layer = LayerMask.NameToLayer("BackToMenu");
            inputControl.Gameplay.Disable();
        }

    #endregion

        // 测试
        private void OnTriggerStay2D(Collider2D other){
            // Debug.Log(other.name);
        }
    }
}
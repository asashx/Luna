using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    #region 角色状态
    public PlayerStateMachine StateMachine { get; private set; }

    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerInAirState InAirState { get; private set; }
    public PlayerSlideState SlideState { get; private set; }
    public PlayerLedgeState LedgeState { get; private set; }
    public PlayerLedgeClimbState LedgeClimbState { get; private set; }
    #endregion

    #region 角色组件
    public Animator Anim { get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }
    public Rigidbody2D RB { get; private set; }
    #endregion

    public Vector2 CurrentVelocity { get; private set; }

    [SerializeField] 
    private PlayerData playerData;

    private Vector2 jumpPressedPos;

    #region 生命周期
    private void Awake()
    {
        StateMachine = new PlayerStateMachine();

        IdleState = new PlayerIdleState(this, StateMachine, playerData, "idle");
        MoveState = new PlayerMoveState(this, StateMachine, playerData, "move");
        JumpState = new PlayerJumpState(this, StateMachine, playerData, "inAir");
        InAirState = new PlayerInAirState(this, StateMachine, playerData, "inAir");
        SlideState = new PlayerSlideState(this, StateMachine, playerData, "slide");
        LedgeState = new PlayerLedgeState(this, StateMachine, playerData, "ledge");
        LedgeClimbState = new PlayerLedgeClimbState(this, StateMachine, playerData, "ledgeClimb");
    }

    private void Start()
    {
        Anim = GetComponent<Animator>();
        InputHandler = GetComponent<PlayerInputHandler>();
        RB = GetComponent<Rigidbody2D>();

        StateMachine.Initialize(IdleState);
    }

    private void Update()
    {
        CurrentVelocity = RB.velocity;
        jumpPressedPos = InputHandler.JumpPressedPos;
        StateMachine.LogicUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.PhysicsUpdate();
    }
    #endregion

    #region 设置函数
    // 设置角色速度
    public void SetVelocityX(float velocity)
    {
        // Anim.SetFloat("velocityX", Mathf.Abs(velocity));
        RB.velocity = new Vector2(velocity, CurrentVelocity.y);
    }
    public void SetVelocityZero()
    {
        RB.velocity = Vector2.zero;
    }
    public void OnJump()
    {
        // 先重置y轴速度
        RB.velocity = new Vector2(CurrentVelocity.x, 0);
        RB.AddForce(Vector2.up * playerData.jumpForce, ForceMode2D.Impulse);
    }

    // 设置角色朝向
    public void CheckFlip(int xInput)
    {
        if (xInput > 0)
        {
            Flip(true);
        }
        else if (xInput < 0)
        {
            Flip(false);
        }
    }
    private void Flip(bool faceRight)
    {
        Vector3 scale = transform.localScale;

        if (faceRight)
        {
            scale.x = Mathf.Abs(scale.x);
        }
        else
        {
            scale.x = -Mathf.Abs(scale.x);
        }

        transform.localScale = scale;
    }
    #endregion

    #region 检测函数
    public bool CheckIfGrounded()
    {
        return Physics2D.OverlapCircle((Vector2)transform.position + playerData.groundCheckOffset, playerData.groundCheckRadius, playerData.groundLayer);
    }
    public bool CheckSlope()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, playerData.slopeCheckLength, playerData.slopeLayer);
        return hit;
    }
    public bool CheckLedge()
    {
        // 朝着面对的方向发射射线
        RaycastHit2D hit1 = Physics2D.Raycast((Vector2)transform.position + playerData.ledgeCheckOffset1, transform.right * transform.localScale.x, playerData.ledgeCheckLength, playerData.groundLayer);
        RaycastHit2D hit2 = Physics2D.Raycast((Vector2)transform.position + playerData.ledgeCheckOffset2, transform.right * transform.localScale.x, playerData.ledgeCheckLength, playerData.groundLayer);
        return !hit1 && hit2;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + playerData.groundCheckOffset, playerData.groundCheckRadius); // 绘制地面检测圆
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(Vector2.down * playerData.slopeCheckLength)); // 绘制斜坡检测线
        Gizmos.DrawWireSphere(jumpPressedPos, 0.05f); // 绘制跳跃按下的位置
        Gizmos.DrawLine((Vector2)transform.position + playerData.ledgeCheckOffset1, 
            (Vector2)transform.position + playerData.ledgeCheckOffset1 + (Vector2)(transform.right * transform.localScale.x * playerData.ledgeCheckLength)); // 绘制悬崖检测线
        Gizmos.DrawLine((Vector2)transform.position + playerData.ledgeCheckOffset2, 
            (Vector2)transform.position + playerData.ledgeCheckOffset2 + (Vector2)(transform.right * transform.localScale.x * playerData.ledgeCheckLength)); // 绘制悬崖检测线
    }
    #endregion

    #region 旧代码
    /*
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private PlayerInput playerInput;
    private Vector2 moveInput;

    [Header("角色属性")]
    public float speed = 200f;
    public float jumpForce = 15f;

    [Header("角色状态")]
    public bool isGrounded = false;
    public bool isLanded = false;
    public bool isJumping = false;
    public float coyoteTime = 0.1f;
    public float coyoteTimeCounter = 0f;
    public float jumpBufferTime = 0.2f;
    public float jumpBufferTimeCounter = 0f;

    [Header("检测参数")]
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;
    public Vector2 groundCheckOffset;
    private Vector2 footPos;

    private void Awake()
    {
        playerInput = new PlayerInput();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 触发跳跃
        playerInput.Player.Jump.started += Jump;
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void Start()
    {
        
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    private void Update()
    {
        // 检测是否
        moveInput = playerInput.Player.Move.ReadValue<Vector2>(); // 获取移动输入
    }

    private void FixedUpdate()
    {
        Move();
        CheckGround();
        // Fall();
        JumpBuffer();
    }

    #region 角色移动
    private void Move()
    {
        // 移动
        rb.velocity = new Vector2(moveInput.x * speed * Time.fixedDeltaTime, rb.velocity.y);

        // 翻转
        if (moveInput.x > 0)
        {
            Flip(true);
        }
        else if (moveInput.x < 0)
        {
            Flip(false);
        }
    }

    
    #endregion

    #region 角色跳跃
    private void Jump(InputAction.CallbackContext ctx)
    {
        if (isLanded && isGrounded)
        {
            Debug.Log("地面跳跃");
            OnJump();
        }
        else if(isLanded && !isJumping && coyoteTimeCounter > 0)
        {
            Debug.Log("土狼跳跃");
            OnJump();
        }
        else if(rb.velocity.y < 0)
        {
            jumpBufferTimeCounter = Time.fixedTime + jumpBufferTime;
        }
        
        DrawFootPos();
    }

    private void OnJump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        coyoteTimeCounter = 0;
        isJumping = true;
        isLanded = false;
        jumpBufferTimeCounter = 0;
    }

    private void DrawFootPos()
    {
        // 绘制角色脚下的点
        footPos = new Vector2(transform.position.x, transform.position.y - 0.4f);
    }

    private void JumpBuffer()
    {
        if (jumpBufferTimeCounter > Time.fixedTime && isGrounded)
        {
            Debug.Log("缓冲跳跃");
            OnJump();
        }
    }

    // private void Fall()
    // {
        // if (rb.velocity.y < 0)
        // {
        //     rb.gravityScale = 2f;
        // }
        // else
        // {
        //     rb.gravityScale = 1f;
        // }
    // }

    private void CheckGround()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle((Vector2)transform.position + groundCheckOffset, groundCheckRadius, groundLayer);

        if (!wasGrounded && isGrounded)
        {
            isLanded = true;
        }

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            isJumping = false;
        }
        else
        {
            coyoteTimeCounter -= Time.fixedDeltaTime;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + groundCheckOffset, groundCheckRadius);
        Gizmos.DrawWireSphere(footPos, 0.05f);
    }
    #endregion
    */
    #endregion
}

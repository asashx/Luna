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
    public int FacingDirection { get; private set; } = 1;

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
            FacingDirection = 1;
        }
        else
        {
            scale.x = -Mathf.Abs(scale.x);
            FacingDirection = -1;
        }

        transform.localScale = scale;
    }
    #endregion

    #region 检测函数
    // public bool CheckIfGrounded()
    // {
    //     Vector2 LeftEnd = (Vector2)transform.position + playerData.groundCheckOffset;

    //     return Physics2D.OverlapCapsule(LeftEnd, new Vector2(playerData.groundCheckRadius * 2,
    //      (playerData.groundCheckRadius + playerData.groundCheckOffset.x) * 2), CapsuleDirection2D.Horizontal, 0, playerData.groundLayer);
    // }
    public bool CheckIfGrounded()
    {
        Vector2 leftEnd = (Vector2)transform.position + playerData.groundCheckOffset;

        if (Physics2D.OverlapCapsule(leftEnd, new Vector2(playerData.groundCheckRadius * 2,
            (playerData.groundCheckRadius + playerData.groundCheckOffset.x) * 2),
            CapsuleDirection2D.Horizontal, 0, playerData.groundLayer))
        {
            return true;
        }

        // 遍历检测附加层
        foreach (LayerMask layer in playerData.extraGroundLayers)
        {
            if (Physics2D.OverlapCapsule(leftEnd, new Vector2(playerData.groundCheckRadius * 2,
                (playerData.groundCheckRadius + playerData.groundCheckOffset.x) * 2),
                CapsuleDirection2D.Horizontal, 0, layer))
            {
                return true;
            }
        }

        return false;
    }
    public bool CheckSlope()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, playerData.slopeCheckLength, playerData.slopeLayer);
        return hit;
    }
    public bool CheckLedge()
    {
        // 朝着面对的方向发射射线
        RaycastHit2D hit1 = Physics2D.Raycast((Vector2)transform.position + playerData.ledgeCheckOffset1, Vector2.right * FacingDirection, playerData.ledgeCheckLength, playerData.groundLayer);
        RaycastHit2D hit2 = Physics2D.Raycast((Vector2)transform.position + playerData.ledgeCheckOffset2, Vector2.right * FacingDirection, playerData.ledgeCheckLength, playerData.groundLayer);
        return !hit1 && hit2;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // 绘制地面检测胶囊
        // Gizmos.DrawWireSphere((Vector2)transform.position + playerData.groundCheckOffset, playerData.groundCheckRadius);
        Vector2 LeftEnd = (Vector2)transform.position + playerData.groundCheckOffset;
        Vector2 RightEnd = (Vector2)transform.position + new Vector2(-playerData.groundCheckOffset.x, playerData.groundCheckOffset.y);
        Gizmos.DrawWireSphere(LeftEnd, playerData.groundCheckRadius);
        Gizmos.DrawWireSphere(RightEnd, playerData.groundCheckRadius);
        // 绘制斜坡检测线
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(Vector2.down * playerData.slopeCheckLength));
        // 绘制跳跃按下的位置
        Gizmos.DrawWireSphere(jumpPressedPos, 0.05f);
        // 绘制悬崖检测线
        Gizmos.DrawLine(transform.position + (Vector3)playerData.ledgeCheckOffset1, transform.position + (Vector3)(playerData.ledgeCheckOffset1 + Vector2.right * FacingDirection * playerData.ledgeCheckLength));
        Gizmos.DrawLine(transform.position + (Vector3)playerData.ledgeCheckOffset2, transform.position + (Vector3)(playerData.ledgeCheckOffset2 + Vector2.right * FacingDirection * playerData.ledgeCheckLength));
    }
    #endregion

    #region 动画触发函数
    public void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();
    public void AnimationFinishTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();
    #endregion
}

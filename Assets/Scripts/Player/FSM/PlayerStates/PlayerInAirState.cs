using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInAirState : PlayerState
{
    private bool isGrounded;
    private bool isSlope;
    private bool isLedge;
    private int xInput;
    private bool jumpInput;
    private float jumpBufferTime;
    private float jumpBufferTimer;
    private bool coyoteJump;
    private float coyoteTime;
    public PlayerInAirState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        jumpBufferTime = playerData.jumpBufferTime;
        jumpBufferTimer = 0f;

        coyoteTime = playerData.coyoteTime;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        xInput = player.InputHandler.NormInputX;
        jumpInput = player.InputHandler.JumpInput;
        jumpBufferTimer = player.InputHandler.JumpBufferTimer;

        CheckJumpBuffer(); // 检测跳跃缓冲
        CheckCoyoteTime(); // 检测土狼跳跃

        // 空中落地时进入idle或move状态
        if (isGrounded && player.CurrentVelocity.y < 0.01f)
        {
            if (xInput == 0)
            {
                stateMachine.ChangeState(player.IdleState);
            }
            else
            {
                stateMachine.ChangeState(player.MoveState);
            }
        }

        // 空中跳跃时进入jump状态
        if (jumpInput && player.JumpState.CanJump())
        {
            player.InputHandler.UseJumpInput();
            stateMachine.ChangeState(player.JumpState);
        }

        // 空中落到滑坡进入slide状态
        if (isSlope)
        {
            stateMachine.ChangeState(player.SlideState);
        }

        // 空中碰到悬崖进入ledge状态
        if (isLedge)
        {
            stateMachine.ChangeState(player.LedgeState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // 空中可左右移动角色
        player.CheckFlip(xInput);
        player.SetVelocityX(playerData.moveSpeed * xInput);
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isGrounded = player.CheckIfGrounded();
        isSlope = player.CheckSlope();
        isLedge = player.CheckLedge();
    }

    public override void Exit()
    {
        base.Exit();
    }

    private void CheckJumpBuffer()
    {
        if (Time.time > jumpBufferTimer + jumpBufferTime)
        {
            player.InputHandler.UseJumpInput();
        }
    }

    private void CheckCoyoteTime()
    {
        if (coyoteJump && Time.time > startTime + coyoteTime)
        {
            coyoteJump = false;
            player.JumpState.DecreaseAmountOfJumpsLeft();
        }
    }

    public void StartCoyoteTime() => coyoteJump = true;
}

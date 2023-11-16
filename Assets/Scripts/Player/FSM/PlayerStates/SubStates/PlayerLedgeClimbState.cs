using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLedgeClimbState : PlayerLedgeState
{
    private float originalGravityScale;
    private Vector2 climbOffset;
    private Vector3 startPos;
    private bool isGrounded;
    public PlayerLedgeClimbState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }

    public override void Enter()
    {
        base.Enter();

        player.SetVelocityZero();
        originalGravityScale = player.RB.gravityScale;
        player.RB.gravityScale = 0f;
        startPos = player.transform.position;

        climbOffset = playerData.climbOffset;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // 后续调整攀爬状态结束条件
        if (isAnimationFinished)
        {
            // 修正攀爬状态结束时的位置
            player.transform.position = new Vector3(startPos.x + climbOffset.x * player.FacingDirection, startPos.y + climbOffset.y, startPos.z);

            player.RB.gravityScale = originalGravityScale;

            stateMachine.ChangeState(player.IdleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isGrounded = player.CheckIfGrounded();
    }

    public override void Exit()
    {
        base.Exit();

        player.InputHandler.UseJumpInput();
    }
}

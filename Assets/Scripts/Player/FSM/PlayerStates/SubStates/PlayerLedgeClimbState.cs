using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLedgeClimbState : PlayerLedgeState
{
    // private float originalGravityScale;
    // private Vector2 climbOffset;
    public PlayerLedgeClimbState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // player.SetVelocityZero();
        // originalGravityScale = player.RB.gravityScale;
        // player.RB.gravityScale = 0f;

        // climbOffset = playerData.climbOffset;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // // 后续调整攀爬状态结束条件
        // if (Time.time >= startTime + 0.1f)
        // {
        //     stateMachine.ChangeState(player.IdleState);
        // }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Exit()
    {
        base.Exit();

        // player.RB.gravityScale = originalGravityScale;

        // // 修正攀爬状态结束时的位置
        // player.transform.position = new Vector3(player.transform.position.x + climbOffset.x * player.transform.localScale.x, player.transform.position.y + climbOffset.y);
    }
}

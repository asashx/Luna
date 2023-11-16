using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLedgeState : PlayerState
{
    private int yInput;
    private float originalGravityScale;
    public PlayerLedgeState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.SetVelocityZero();
        originalGravityScale = player.RB.gravityScale;

        player.RB.gravityScale = 0f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        yInput = player.InputHandler.NormInputY;

        // 按下向上键，进入攀爬状态
        if (yInput == 1)
        {
            stateMachine.ChangeState(player.LedgeClimbState);
        }
        // 按下向下键，进入下落状态
        else if (yInput == -1)
        {
            stateMachine.ChangeState(player.InAirState);
        }
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

        player.RB.gravityScale = originalGravityScale;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    private float currentSpeed;
    public PlayerMoveState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        currentSpeed = player.CurrentVelocity.x;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if(xInput == 0)
        {
            stateMachine.ChangeState(player.IdleState);
        }

        currentSpeed = Mathf.MoveTowards(currentSpeed, playerData.moveSpeed * xInput, playerData.acceleration * Time.deltaTime);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        player.CheckFlip(xInput);

        player.SetVelocityX(currentSpeed);
    }
}

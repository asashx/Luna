using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public int NormInputX { get; private set; }
    public int NormInputY { get; private set; }
    public bool JumpInput { get; private set; }
    public Vector2 JumpPressedPos { get; private set; }
    public float JumpBufferTimer { get; private set; }


    private void Awake()
    {
        JumpPressedPos = new Vector2(transform.position.x, transform.position.y - 0.4f);
    }
    private void Update()
    {
        
    }

    public void OnMoveInput(InputAction.CallbackContext ctx)
    {
        MoveInput = ctx.ReadValue<Vector2>();

        NormInputX = (int)(MoveInput * Vector2.right).normalized.x;
        NormInputY = (int)(MoveInput * Vector2.up).normalized.y;
    }

    public void OnJumpInput(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            JumpInput = true;
            JumpBufferTimer = Time.time;

            // 绘制跳跃按下的位置
            JumpPressedPos = new Vector2(transform.position.x, transform.position.y - 0.4f);
        }
    }

    public void UseJumpInput() => JumpInput = false;

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Data/PlayerData", order = 0)]
public class PlayerData : ScriptableObject 
{
    [Header("移动属性")]
    public float moveSpeed = 6f;
    public float acceleration = 30f;

    [Header("跳跃属性")]
    public float jumpForce = 16f; // 测试16时可轻松跳上两格，差大半个身位跳上三格，后续人物高度增加后再调整
    public float jumpBufferTime = 0.1f; // 跳跃输入缓冲时间
    public float coyoteTime = 0.1f; // 土狼时间
    public int amountOfJumps = 1; // 可跳跃次数

    [Header("地面检测")]
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;
    public List<LayerMask> extraGroundLayers;
    public Vector2 groundCheckOffset;

    [Header("滑坡检测")]
    public float slopeCheckLength = 0.8f;
    public LayerMask slopeLayer;
    public float slideMaxSpeed = 8f;

    [Header("悬崖检测")]
    public Vector2 ledgeCheckOffset1;
    public Vector2 ledgeCheckOffset2;
    public float ledgeCheckLength = 0.8f;
    public Vector2 climbOffset;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private PlayerInput playerInput;
    private Vector2 moveInput;

    [Header("角色属性")]
    public float speed = 200f;
    public float jumpForce = 50f;

    [Header("角色状态")]
    public bool isGrounded = false;
    public bool isLanded = false;
    public float coyoteTime = 0.1f;
    public float coyoteTimeCounter = 0f;

    [Header("检测参数")]
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;
    public Vector2 groundCheckOffset;

    private void Awake()
    {
        playerInput = new PlayerInput();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 触发跳跃
        playerInput.Player.Jump.performed += Jump;
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
        Fall();
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

    #region 角色跳跃
    private void Jump(InputAction.CallbackContext ctx)
    {
        if (isLanded && (isGrounded || coyoteTimeCounter > 0))
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            coyoteTimeCounter = 0;
            isLanded = false;
        }
    }

    private void Fall()
    {
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = 2f;
        }
        // else if (rb.velocity.y > 0 && !playerInput.Player.Jump.ReadValue<float>().Equals(1))
        // {
        //     rb.gravityScale = 2f;
        // }
        else
        {
            rb.gravityScale = 1f;
        }
    }

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
    }
    #endregion
}

using System.Collections;
using UnityEngine;

public class InteractiveBall : MonoBehaviour
{
    public enum STATE
    {   
        idle,               //围绕玩家旋转
        chasingPlayer,      //回到玩家身边
        //chasingMouse,     //回到鼠标位置
        controlled,         //由鼠标控制
    }

    public GameObject player;
    private STATE state = STATE.controlled;
    private bool canChangeState = true;

    [SerializeField][Header("每一帧旋转角度")]
    private float rotateAnglePerFrame;
    private float rotateAngle = 0f;
    [SerializeField][Header("旋转半径")]
    private float rotateRadius;
    //[Header("正在追随的目标")]
    private ObservableValue<Vector3, InteractiveBall> target;
    //[Header("小于这个值判定为已接近")]
    private float nearDistance = 0.01f;
    [SerializeField][Header("追随速度")]
    private float chaseSpeed;
    private SpriteRenderer spriteRenderer;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        target = new(Vector3.zero, this);
        target.Value = player.transform.position + new Vector3(rotateRadius, 0f, 0f);
    }
    private void FixedUpdate()
    {
        switch(state)
        {
            case(STATE.idle):
            {
                MyRotateAround();
                break;
            }
            default:
                break;
        }
    }
    private void Update()
    {
        //按下E时，切换target
        if (Input.GetKeyDown(KeyCode.E))
        {
            canChangeState = true;
            if (state == STATE.controlled)
                target.Value = player.transform.position + new Vector3(rotateRadius, 0f, 0f);
            else if(state == STATE.idle || state == STATE.chasingPlayer)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                target.Value = new(mousePos.x, mousePos.y, 0f);
            }
        }
    }
    //idle状态：围绕玩家旋转
    void MyRotateAround()
    {
        rotateAngle += rotateAnglePerFrame;
        transform.position = new Vector3(player.transform.position.x + rotateRadius * Mathf.Cos(rotateAngle * Mathf.Deg2Rad),
            player.transform.position.y, player.transform.position.z - rotateRadius * Mathf.Sin(rotateAngle * Mathf.Deg2Rad));
        //transform.SetPositionAndRotation(new(transform.position.x,player.transform.position.y,0), Quaternion.identity);
    }
    //chasingPlayer状态：向target移动，到达后切换到idle状态
    IEnumerator ChasePlayer()
    {
        while(!IsNear())
        {
            target.Value = player.transform.position + new Vector3(rotateRadius, 0f, 0f);
            transform.position = Vector3.MoveTowards(transform.position, target.Value, chaseSpeed);
            yield return new WaitForFixedUpdate();
        }
        state = STATE.idle;
        yield break;
    }
    //controlled状态：向target移动，到达后保持controlled状态
    IEnumerator ChaseMouse()
    {
        transform.rotation = Quaternion.identity;
        while (true)
        {
            //获取鼠标位置的世界坐标
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.Value = new(mousePos.x,mousePos.y,0f);
            transform.position = Vector3.MoveTowards(transform.position, target.Value, chaseSpeed);
            yield return new WaitForFixedUpdate();
        }
    }
    bool IsNear()
    {
        if ((transform.position - target.Value).magnitude <= nearDistance)
            return true;
        return false;
    }
    //target变化时调用，并切换状态
    public void OnTargetChange()
    {
        if(!canChangeState)
            return;
        StopAllCoroutines();
        Debug.Log("Target changed ! oldState : " + state);
        if (state == STATE.controlled)
        {
            state = STATE.chasingPlayer;
            StartCoroutine(nameof(ChasePlayer));
        }
        else if(state == STATE.idle || state == STATE.chasingPlayer)
        {
            state = STATE.controlled;
            StartCoroutine(nameof(ChaseMouse));
        }
        
        canChangeState = false;
    }
}

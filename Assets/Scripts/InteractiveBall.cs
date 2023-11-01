using System.Collections;
using UnityEngine;

public class InteractiveBall : MonoBehaviour
{
    public enum STATE
    {   
        idle,           //围绕玩家旋转
        chasing,        //回到玩家身边
        //chasingMouse,   //回到鼠标位置
        controlled,     //由鼠标控制
    }

    public GameObject player;
    public STATE state;

    [Header("每一帧旋转角度")]
    public float rotateAngle = 1f;
    [Header("旋转半径")]
    public float rotateRadius = 0.6f;
    //[Header("正在追随的目标")]
    private ObservableValue<Vector3, InteractiveBall> target;
    //[Header("小于这个值判定为已接近")]
    private float nearDistance = 0.01f;
    [SerializeField][Header("追随速度")]
    private float chaseSpeed = 0.015f;
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
            case (STATE.chasing):
            {
                target.Value = player.transform.position + new Vector3(rotateRadius, 0f, 0f);
                break;
            }
            default:
                break;
        }
    }
    void MyRotateAround()
    {
        transform.RotateAround(player.transform.position, new(0, 1, 0), rotateAngle);
    }
    IEnumerator ChaseTarget()
    {
        while(!IsNear())
        {
            transform.position = Vector3.MoveTowards(transform.position, target.Value, chaseSpeed);
            yield return new WaitForFixedUpdate();
        }
        state = STATE.idle;
        yield break;
    }

    bool IsNear()
    {
        if ((transform.position - target.Value).magnitude <= nearDistance)
            return true;
        return false;
    }
    public void OnTargetChange()
    {
        if(state!= STATE.chasing)
        {
            state = STATE.chasing;
            StartCoroutine(nameof(ChaseTarget));
        }
            
        Debug.Log("Target changed to pos :" + target.Value);
    }
}

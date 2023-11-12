using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveBall : MonoBehaviour
{
    public static InteractiveBall Instance { get; private set; }

    public enum STATE
    {   
        none,                   //不移动
        rotatingAroundPlayer,   //围绕玩家旋转
        chasingPlayer,          //回到玩家身边的过程中
        followingMouse,         //跟随鼠标

        Idling,                 //原地待机
        GuildingAhead,          //引领玩家前进
        OnTrail,                //按照既定轨迹前进
    }

    public GameObject player;
    private ObservableValue<STATE,InteractiveBall> state;
    private bool forcedByTrigger = false;
    private bool canChangeState = true;

    [SerializeField][Header("每一帧旋转角度")]
    private float rotateAnglePerFrame;
    private float rotateAngle = 0f;
    [SerializeField][Header("旋转半径")]
    private float rotateRadius;

    //[Header("正在追随的目标")]
    private Vector3 target;
    //[Header("小于这个值判定为已接近")]
    private float nearDistance = 0.01f;
    [SerializeField][Header("追随速度")]
    private float chaseSpeed;

    private int index_lastStation = -1;
    private List<GameObject> list_passStations = new();
    private List<float> list_passSpeed = new();
    private Vector2 delta_ahead;

    private void Awake()
    {
        Instance = this;
        state = new(STATE.followingMouse, this);
    }
    private void Update()
    {
        switch (state.Value)
        {
            case (STATE.rotatingAroundPlayer):
                Input_when_RotateAroundPlayer();
                break;
            case (STATE.followingMouse):
                Input_when_FollowMouse();
                break;
            case STATE.chasingPlayer:
                Input_when_RotateAroundPlayer();
                break;
            default:
                break;
        }
    }
    private void FixedUpdate()
    {
        switch (state.Value)
        {
            case (STATE.rotatingAroundPlayer):
                RotateAroundPlayer();
                break;
            case (STATE.followingMouse):
                FollowMouse();
                break;
            case STATE.chasingPlayer:
                ChasePlayer();
                break;
            case STATE.Idling:
                Idle();
                break;
            case STATE.OnTrail:
                GoOnTrail();
                break;
            case STATE.GuildingAhead:
                GuildAhead();
                break;
            default:
                break;
        }
    }
    void Input_when_RotateAroundPlayer()
    {
        if (forcedByTrigger)
            return;
        if (Input.GetKeyDown(KeyCode.E))
        {
            canChangeState = true;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target = new(mousePos.x, mousePos.y, 0f);
            state.Value = STATE.followingMouse;
        }
    }
    //void Input_when_ChasePlayer()
    //{
    //    if (forcedByTrigger)
    //        return;
    //    //按下E时，切换target
    //    if (Input.GetKeyDown(KeyCode.E))
    //    {
    //        //canChangeState = true;
    //        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //        target = new(mousePos.x, mousePos.y, 0f);
    //        state.Value = STATE.followingMouse;
    //    }
    //}
    void Input_when_FollowMouse()
    {
        if (forcedByTrigger)
            return;
        //按下E时，切换target
        if (Input.GetKeyDown(KeyCode.E))
        {
            //canChangeState = true;
            target = player.transform.position + new Vector3(rotateRadius, 0f, 0f);
            state.Value = STATE.chasingPlayer;
        }
    }
    void RotateAroundPlayer()
    {
        rotateAngle += rotateAnglePerFrame;
        transform.position = new Vector3(player.transform.position.x + rotateRadius * Mathf.Cos(rotateAngle * Mathf.Deg2Rad),
            player.transform.position.y, player.transform.position.z - rotateRadius * Mathf.Sin(rotateAngle * Mathf.Deg2Rad));
    }
    //chasingPlayer状态：向target移动，到达后切换到rotatingAroundPlayer状态
    void ChasePlayer()
    {
        target = player.transform.position + new Vector3(rotateRadius, 0f, 0f);
        if (!IsNear())
            transform.position = Vector3.MoveTowards(transform.position, target, chaseSpeed);
        else
            
            state.Value = STATE.rotatingAroundPlayer;
    }
    //followingMouse状态：向target移动，到达后保持followingMouse状态
    void FollowMouse()
    {
        transform.rotation = Quaternion.identity;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        target = new(mousePos.x,mousePos.y,0f);
        transform.position = Vector3.MoveTowards(transform.position, target, chaseSpeed);
    }
    void Idle()
    {
        //上下浮动
        transform.position = new Vector3(transform.position.x, transform.position.y + Mathf.Sin(Time.time) * 0.01f, transform.position.z);
    }
    //距离小于等于nearDistance时返回true
    void GoOnTrail()
    {
        //Debug.Log(index_lastStation);
        target = list_passStations[index_lastStation+1].transform.position;
        if(IsNear())
        {
            index_lastStation++;
            if (index_lastStation == list_passStations.Count - 1)
                state.Value = STATE.Idling;
        }
        else
        {
            if(index_lastStation + 1 >= list_passSpeed.Count)
                transform.position = Vector3.MoveTowards(transform.position, target, chaseSpeed);
            else
                transform.position = Vector3.MoveTowards(transform.position, target, list_passSpeed[index_lastStation + 1]);
        }
    }
    void GuildAhead()
    {
        target = player.transform.position + (Vector3)delta_ahead;
        //if (IsNear())
        //    Idle();
        //else
            transform.position = Vector3.MoveTowards(transform.position, target, chaseSpeed);
    }
    bool IsNear()
    {

        if ((transform.position - target).magnitude <= nearDistance)
            return true;
        return false;
    }

    public bool HandleTrigger(MyTrigger myTrigger)
    {
        Debug.Log(myTrigger.enterType + " Enter :" + myTrigger.effectType);
        if(myTrigger.effectType == MyTrigger.EffectType.None)
        {
            forcedByTrigger = false;
            return true;
        }
        if (myTrigger.enterType == MyTrigger.EnterType.Ball && state.Value == STATE.followingMouse)
            return false;
        forcedByTrigger = true;
        switch (myTrigger.effectType)
        {
            case MyTrigger.EffectType.GuildAhead:
                delta_ahead = myTrigger.delta_ahead;
                state.Value = STATE.GuildingAhead;
                break;
            case MyTrigger.EffectType.ChasePlayer:
                forcedByTrigger = false;
                state.Value = STATE.chasingPlayer;
                break;
            case MyTrigger.EffectType.Idle:
                state.Value = STATE.Idling;
                break;
            case MyTrigger.EffectType.OnTrail:
                list_passStations.Clear();
                //深拷贝
                for(int i=0;i<myTrigger.list_passStations.Count;i++)
                {
                    list_passStations.Add(myTrigger.list_passStations[i]);
                }
                list_passSpeed.Clear();
                for(int i = 0; i < myTrigger.list_passSpeed.Count; i++)
                {
                    list_passSpeed.Add(myTrigger.list_passSpeed[i]);
                }
                index_lastStation = -1;

                state.Value = STATE.OnTrail;
                break;
            default:
                break;
        }
        return true;
    }
    
    public void OnEnterState(STATE state)
    {
        switch (state)
        {
            case STATE.rotatingAroundPlayer:
                rotateAngle = 0f;
                break;
            default:
                break;
        }
    }
}

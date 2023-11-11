using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTrigger : MonoBehaviour
{
    [Header("可视")]
    public bool visable;

    [Header("是否只触发一次")]
    public bool usedOnce;
    [SerializeField][Header("是否已经触发过")]
    private bool used = false;
    public enum EnterType
    {
        player,
        ball,
    };
    public EnterType enterType;
    public enum EffectType
    {
        None,//无效果
        GuildAhead,//引领玩家前进
        ChasePlayer,//跟随玩家
        Idle,//停在原地
        OnTrail,//按照既定轨迹前进
    }
    public EffectType effectType;
    
    [Header("GuildAhead")]
    [Tooltip("离玩家的相对坐标")] public Vector2 delta_ahead;

    
    
    public GameObject PassStations;
    [Header("Ontrail")]
    [Tooltip("既定轨迹的中间站点")] public List<GameObject> list_passStations;
    [Tooltip("站点间的移动速度")] public List<float> list_speed;

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (used)
            return;
        if(collision.CompareTag("Player") && enterType == EnterType.player)
        {
            CallHandleTrigger();
        }
        if (collision.CompareTag("Ball") && enterType == EnterType.ball)
        {
            CallHandleTrigger();
        }
    }
    void CallHandleTrigger()
    {
        bool whetherUse = InteractiveBall.Instance.HandleTrigger(this);
        if (usedOnce && whetherUse)
            used = true;
        visable = !used;
        OnValidate();
    }
    private void Awake()
    {
        list_passStations.Clear();
        for (int i=0;i< PassStations.transform.childCount;i++)
        {
            GameObject child = PassStations.transform.GetChild(i).gameObject;
            if (child.activeSelf)
                list_passStations.Add(child);
        }
    }

    public void OnValidate()
    {
        PassStations = gameObject.transform.Find("PassStations").gameObject;
        GetComponent<SpriteRenderer>().enabled = visable;
        for (int i = 0; i < PassStations.transform.childCount; i++)
        {
            GameObject child = PassStations.transform.GetChild(i).gameObject;
            child.GetComponent<SpriteRenderer>().enabled = visable;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;


public class MyTrigger : MonoBehaviour
{
    [Header("可视")]
    public bool visable;

    [Header("只使用一次")]
    public bool usedOnce;
    [SerializeField][Header("是否已使用")]
    private bool used = false;
    public enum EnterType
    {
        Player,
        Ball,
    };
    public EnterType enterType;
    public enum EffectType
    {
        None,           //???
        GuildAhead,     //??????
        ChasePlayer,    //????
        Idle,           //????
        OnTrail,        //????????
    }
    public EffectType effectType;

    [Header("GuildAhead")]
    [Tooltip("带领玩家时的位移偏移")] public Vector2 delta_ahead;


    [HideInInspector]
    public GameObject PassStations;
    [HideInInspector]
    [Tooltip("途径站点")] public List<GameObject> list_passStations;
    [Header("Ontrail")]
    [Tooltip("每站之间的速度")]public List<float> list_passSpeed = new();

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (used)
            return;
        if(collision.CompareTag("Player") && enterType == EnterType.Player)
        {
            CallHandleTrigger();
        }
        if (collision.CompareTag("Ball") && enterType == EnterType.Ball)
        {
            CallHandleTrigger();
        }
    }
    void CallHandleTrigger()
    {
        bool whetherUse = InteractiveBall.Instance.HandleTrigger(this);
        if (usedOnce && whetherUse)
        {
            used = true;
            visable = false;
        }
        OnValidate();
    }
    private void Awake()
    {
        list_passStations.Clear();
        for (int i = 0; i < PassStations.transform.childCount; i++)
        {
            GameObject child = PassStations.transform.GetChild(i).gameObject;
            if (child.activeSelf)
                list_passStations.Add(child);
        }
    }

    public void OnValidate()
    {
        #region visable
        PassStations = transform.Find("PassStations").gameObject;
        GetComponent<SpriteRenderer>().enabled = visable;
        for (int i = 0; i < PassStations.transform.childCount; i++)
        {
            PassStations.transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = visable;
        }
        #endregion

        #region name
        gameObject.name = "Trigger_" + enterType + "_"+  effectType.ToString();
        #endregion

        #region onTrail
        if (effectType == EffectType.OnTrail)
            PassStations.SetActive(true);
        else
            PassStations.SetActive(false);
        #endregion
    }
}

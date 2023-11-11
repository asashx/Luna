using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerManager : MonoBehaviour
{
    public static TriggerManager Instance { get; private set; }
    [Header("全部可视")]
    public bool allVisable;
    //[HideInInspector]
    public List<MyTrigger> prefab_triggers = new();
    private void Awake()
    {
        Instance = this;
    }

    void Initialize()
    {
        prefab_triggers.Clear();
        for (int i=0;i<transform.childCount;i++)
        {
            if(transform.GetChild(i).GetComponent<MyTrigger>())
                prefab_triggers.Add(transform.GetChild(i).GetComponent<MyTrigger>());
        }
    }

    private void OnValidate()
    {
        Initialize();
        for (int i = 0; i < prefab_triggers.Count; i++)
        {
            prefab_triggers[i].visable = allVisable;
            prefab_triggers[i].OnValidate();
        }
    }
}

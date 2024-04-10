using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TriggerManager : Singleton<TriggerManager>
{
    [Label("全部可视")]
    public bool allVisable;
    //[HideInInspector]
    List<MyTriggerBase> existing_triggers = new();
    void Initialize()
    {
        existing_triggers.Clear();
        for (int i=0;i<transform.childCount;i++)
        {
            if(transform.GetChild(i).GetComponent<MyTriggerBase>())
                existing_triggers.Add(transform.GetChild(i).GetComponent<MyTriggerBase>());
        }
    }

    private void OnValidate()
    {
        Initialize();
        for (int i = 0; i < existing_triggers.Count; i++)
        {
            existing_triggers[i].visable = allVisable;
            existing_triggers[i].OnValidate();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static MyTriggerBase;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class MyTriggerEnter : MonoBehaviour
{
    MyTriggerBase m_TriggerBase => transform.parent.GetComponent<MyTriggerBase>();
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_TriggerBase.used)
            return;
        if (collision.CompareTag("Player") && m_TriggerBase.enterType == EnterType.Player)
        {
            m_TriggerBase.CallHandleTrigger();
        }
        if (collision.CompareTag("Ball") && m_TriggerBase.enterType == EnterType.Ball)
        {
            m_TriggerBase.CallHandleTrigger();
        }
    }
}

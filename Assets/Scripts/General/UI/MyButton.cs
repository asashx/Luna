using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class MyButton : Button
{
    [SerializeField] private UnityEvent onPointerDown = new();
    [SerializeField]
    private UnityEvent onPointerUp = new();
    [SerializeField]
    private float longPressTime = 0.6f;
    private float longPressTimer = 0;
    [SerializeField]
    private UnityEvent onStartLongPress = new();
    public ref UnityEvent get_onPointerDown() => ref onPointerDown;
    private bool isDown = false;
    private bool isPress = false;
    protected override void Awake()
    {
        base.Awake();
        navigation = new() { mode = Navigation.Mode.None };
    }
    void Update()
    {
        if (isDown)
        {
            if (isPress)
            {
                return;
            }
            longPressTimer += Time.deltaTime;
            if (longPressTimer >= longPressTime)
            {
                isPress = true;
                onStartLongPress.Invoke();
            }
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        isDown = true;
        longPressTimer = 0;
        onPointerDown.Invoke();
        //print("down");
    }

    // Button is released
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        isDown = isPress = false;
        onPointerUp.Invoke();
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        isDown = isPress = false;
    }
    public void Test()
    {
        print("Long Press");
    }
}

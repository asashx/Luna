using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
[Serializable]
public class ObservableValue<T,TCLASS>
{
    [SerializeField]
    private T value;
    private readonly TCLASS valueClass;
    delegate void OnValueChangeDelegate(T oldValue, T newValue, TCLASS valueClass);
    event OnValueChangeDelegate OnValueChangeEvent;
    public ObservableValue(T value, TCLASS valueClass)
    {
        this.value = value;
        this.valueClass = valueClass;
        this.OnValueChangeEvent += OnValueChange;
    }
    public T Value
    {
        get => value;
        set
        {
            T oldValue = this.value;
            if (this.value.Equals(value))
                return;
            this.value = value;
            OnValueChangeEvent?.Invoke(oldValue, value, this.valueClass);
        }
    }
    public void OnValueChange(T oldValue, T newValue, TCLASS valueClass)
    {
        if (valueClass is InteractiveBall)
        {
            ((InteractiveBall)((object)valueClass)).OnExitState((InteractiveBall.STATE)(object)oldValue);
            ((InteractiveBall)((object)valueClass)).OnEnterState((InteractiveBall.STATE)(object)newValue);
        }
    }
}

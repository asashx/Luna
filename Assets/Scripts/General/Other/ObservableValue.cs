using System;
using UnityEngine;
[Serializable]
public class ObservableValue<T>
{
    [SerializeField]
    private T value;
    public delegate void OnValueChangeDelegate(T oldValue, T newValue);
    public event OnValueChangeDelegate OnValueChangeEvent;
    public ObservableValue(T value,OnValueChangeDelegate onValueChange)
    {
        this.value = value;
        this.OnValueChangeEvent += onValueChange;
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
            OnValueChangeEvent?.Invoke(oldValue, value);
        }
    }
}

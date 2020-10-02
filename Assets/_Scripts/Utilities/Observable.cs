using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observable<T> where T : struct
{
    private T value_;
    public T value { get => value_; set => SetValue(value); }

    public event EventHandler<ValueChangedArgs> ValueChanged;

    public Observable(T t)
    {
        value_ = t;
    }
    public Observable() : this(default) { }

    public void SetValue(T value)
    {
        value_ = value;

        ValueChanged?.Invoke(this, new ValueChangedArgs());
    }

    public void SetValueWithoutNotify(T value)
    {
        value_ = value;
    }

    public override string ToString()
    {
        return value.ToString();
    }

    public class ValueChangedArgs : EventArgs
    {
        
    }
}

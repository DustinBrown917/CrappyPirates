using System;

public class ValueChangedArgs<T> : EventArgs
{
    public T oldValue = default;
    public T newValue = default;

    public ValueChangedArgs(T oldValue, T newValue)
    {
        this.oldValue = oldValue;
        this.newValue = newValue;
    }
}

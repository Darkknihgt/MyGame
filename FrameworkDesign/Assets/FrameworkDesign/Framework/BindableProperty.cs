using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BindableProperty<T> where T : IEquatable<T>
{
    private T mValue = default(T);
    public T Value

    {
        get => mValue;
        set
        {
            if (!mValue.Equals(value))
            {
                mValue = value;
                OnValueChanged?.Invoke(value);
            }
        }
    }

    public Action<T> OnValueChanged;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Health : NetworkBehaviour
{
    [SyncVar(hook = "HandleHealthChanged")]
    [SerializeField] private float current_ = 100.0f;
    public float Current { get => current_; }

    [SerializeField] private float max_ = 100.0f;
    public float Max { get => max_; }

    public event Action<float, float> OnHealthChanged;

    [ServerCallback]
    public void Damage(float amount)
    {
        current_ -= amount;
    }

    private void HandleHealthChanged(float oldValue, float newValue)
    {
        OnHealthChanged?.Invoke(oldValue, newValue);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkNavigationUI : MonoBehaviour
{
    private static NetworkNavigationUI instance_ = null;
    public static NetworkNavigationUI Instance { get => instance_; }

    public static event Action InstanceUpdated;

    public int accelerate = 0;
    public int turn = 0;

    private void Awake()
    {
        if (instance_ == null) {
            instance_ = this;
            InstanceUpdated?.Invoke();
        } else {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        if (instance_ == this) {
            instance_ = null;
            InstanceUpdated?.Invoke();
        }
    }

    public void HandleAccelerationChanged(float value)
    {
        accelerate = Mathf.RoundToInt(value);
    }

    public void HandleTurnChanged(float value)
    {
        turn = Mathf.RoundToInt(value);
    }
}

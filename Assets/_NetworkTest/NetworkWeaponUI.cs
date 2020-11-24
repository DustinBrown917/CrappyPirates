using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkWeaponUI : MonoBehaviour
{
    private static NetworkWeaponUI instance_ = null;
    public static NetworkWeaponUI Instance { get => instance_; }

    public static event Action InstanceUpdated;

    public event Action FireForward;
    public event Action FirePort;
    public event Action FireStarboard;
    public event Action FireRear;


    private void Awake()
    {
        if(instance_ == null) {
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
        if(instance_ == this) {
            instance_ = null;
            InstanceUpdated?.Invoke();
        }
    }

    public void FireForwardBattery() {
        FireForward?.Invoke();
    }

    public void FirePortBattery()
    {
        FirePort?.Invoke();
    }

    public void FireStarboardBattery()
    {
        FireStarboard?.Invoke();
    }

    public void FireRearBattery()
    {
        FireRear?.Invoke();
    }

}

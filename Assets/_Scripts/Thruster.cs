using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : MonoBehaviour
{
    [SerializeField] private ThrusterGroups thrusterGroup_ = 0;
    public ThrusterGroups ThrusterGroup { get => thrusterGroup_; }
    [SerializeField] private float throttle = 0.0f;

    [SerializeField] private ParticleSystem ps = null;
    private ParticleSystem.MainModule mm = default;

    [SerializeField] private float minStartSpeed = 0.0f;
    [SerializeField] private float maxStartSpeed = 5.0f;

    private void Awake()
    {
        mm = ps.main;
    }

    private void Start()
    {
        SetThrottle(throttle);
    }

    public void SetThrottle(float amount)
    {
        throttle = Mathf.Clamp01(amount);

        float speed = Mathf.Lerp(minStartSpeed, maxStartSpeed, throttle);

        mm.startSpeed = speed;
    }
}

[Flags]
public enum ThrusterGroups : byte
{
    REAR = 1,
    FORWARD = 2,
    STARBOARD = 4,
    PORT = 8,
    ROT_CW = 16,
    ROT_CCW = 32
}


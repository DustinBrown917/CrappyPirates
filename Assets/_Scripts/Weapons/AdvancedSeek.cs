﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;



public class AdvancedSeek : MonoBehaviour
{
    private Rigidbody Body;
    [SerializeField] private float compensationStrength = 1.0f;
    private Vector3 compensation = new Vector3();
    private float acceleration = 2.0f;

    private void Awake()
    {
        Body = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Compensated seek.
    /// </summary>
    /// <returns></returns>
    public Vector3 GetSteering(Rigidbody target)
    {
        Vector3 dir = (target.position - transform.position).normalized;

        compensation = GetVelocityCompensation(dir, Body.velocity.normalized, target);
        return (dir * acceleration) + compensation;
    }

    public Vector3 GetSteering(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;

        return dir * acceleration;
    }

    public Vector3 GetDeceleration()
    {
        Vector3 decel = Body.velocity.normalized * -1;    

        return decel;
    }

    /// <summary>
    /// Plain ol' seek. Orbits galore.
    /// </summary>
    /// <returns></returns>
    public Vector3 GetSteering_Boring(Rigidbody target)
    {
        Vector3 dir = (target.position - transform.position).normalized;

        return dir * acceleration;
    }



    private Vector3 GetVelocityCompensation(Vector3 directionNormal, Vector3 velocityNormal, Rigidbody target)
    {
        Vector3 velDifference = (Body.velocity - target.velocity);
        velocityNormal = velDifference.normalized;

        Vector2 dir2d = new Vector2(directionNormal.x, directionNormal.z);
        Vector2 vel2d = new Vector2(velocityNormal.x, velocityNormal.z);
        float compFactor = Vector2.Angle(vel2d, dir2d) / 180.0f;

        Vector2 comp2d = Vector2.LerpUnclamped(dir2d, vel2d, -compFactor * 2);

        return new Vector3(comp2d.x, 0, comp2d.y) * acceleration * compensationStrength;
    }
}

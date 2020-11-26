using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class Missile : Projectile
{
    [SerializeField] protected float acceleration = 1.0f;
    public float Acceleration { get => acceleration; }
    [SerializeField] protected float maxSpeed = 3.0f;
    public float MaxSpeed { get => maxSpeed; }
    [SerializeField] private float lifeTime = 10.0f;
    public float LifeTime { get => lifeTime; }
    private float lifeTimeRemaining = 10.0f;
    public float LifeTimeRemaining { get => lifeTimeRemaining; }
    private bool shouldAccelerate = false;

    public override void Arm(Rigidbody target, params Collider[] launcherColliders)
    {
        base.Arm(target, launcherColliders);
        shouldAccelerate = true;
        lifeTimeRemaining = lifeTime;
    }

    protected virtual void Update()
    {
        lifeTimeRemaining -= Time.deltaTime;
        if(lifeTimeRemaining <= 0) {
            HandleImpact();
        }
    }

    protected virtual void FixedUpdate()
    {
        if (shouldAccelerate) {
            Body.velocity += transform.forward * acceleration * Time.fixedDeltaTime;

            if(maxSpeed > 0 && Body.velocity.magnitude > maxSpeed) {
                Body.velocity = Vector3.ClampMagnitude(Body.velocity, maxSpeed);
                shouldAccelerate = false;
                Debug.Log("Stop Accelerating");
            }

        } else {
            if(maxSpeed > 0 && Body.velocity.magnitude < maxSpeed) {
                shouldAccelerate = true;
            }
        }
    }


}

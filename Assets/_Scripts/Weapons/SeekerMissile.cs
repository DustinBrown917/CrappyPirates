using System.Collections;
using UnityEngine;

public class SeekerMissile : Missile
{
    private Rigidbody target;
    [SerializeField] private float delayBeforeSteering = 1.0f;
    [SerializeField] private float compensationStrength = 1.0f;
    private bool shouldSteer = false;
    private Coroutine cr_SteeringDelay = null;

    private Vector3 compensation = new Vector3();

    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, transform.position + compensation);
    }

    public override void Arm(Rigidbody target, params Collider[] launcherColliders)
    {
        base.Arm(target, launcherColliders);

        this.target = target;

        StartDelayBeforeSteering();
    }

    /// <summary>
    /// Compensated seek.
    /// </summary>
    /// <returns></returns>
    public Vector3 GetSteering()
    {
        if(target == null) { HandleImpact(); }

        Vector3 dir = (target.position - transform.position).normalized;

        compensation = GetVelocityCompensation(dir, Body.velocity.normalized);
        return (dir * acceleration) + compensation;
    }


    /// <summary>
    /// Plain ol' seek. Orbits galore.
    /// </summary>
    /// <returns></returns>
    public Vector3 GetSteering_Boring()
    {
        if (target == null) { HandleImpact(); }

        Vector3 dir = (target.position - transform.position).normalized;

        Vector3 velNormal = Body.velocity.normalized;

        return dir * acceleration;
    }



    private Vector3 GetVelocityCompensation(Vector3 directionNormal, Vector3 velocityNormal)
    {
        Vector3 velDifference = (Body.velocity - target.velocity);
        velocityNormal = velDifference.normalized;

        Vector2 dir2d = new Vector2(directionNormal.x, directionNormal.z);
        Vector2 vel2d = new Vector2(velocityNormal.x, velocityNormal.z);
        float compFactor = Vector2.Angle(vel2d, dir2d) / 180.0f;

        Vector2 comp2d = Vector2.LerpUnclamped(dir2d, vel2d, -compFactor * 2);

        //compensation = GetVelocityCompensation(dir.normalized, velNormal);

        return new Vector3(comp2d.x, 0, comp2d.y) * acceleration * compensationStrength;
    }

    protected override void HandleImpact()
    {
        base.HandleImpact();
        shouldSteer = false;
        StopDelayBeforeSteering();
    }

    protected override void FixedUpdate()
    {
        if(!shouldSteer) { return; }
        Vector3 steering = GetSteering();
        Body.velocity += steering * Time.fixedDeltaTime;
        transform.forward = steering.normalized;

        if(maxSpeed > 0 && Body.velocity.magnitude > maxSpeed) {
            Body.velocity = Vector3.ClampMagnitude(Body.velocity, maxSpeed);
        }
    }

    public void StartDelayBeforeSteering()
    {
        StopDelayBeforeSteering();
        cr_SteeringDelay = StartCoroutine(DelayBeforeSteering());
    }

    private void StopDelayBeforeSteering()
    {
        if(cr_SteeringDelay != null) {
            StopCoroutine(cr_SteeringDelay);          
        }
        cr_SteeringDelay = null;
    }

    private IEnumerator DelayBeforeSteering()
    {
        yield return new WaitForSeconds(delayBeforeSteering);
        shouldSteer = true;
        cr_SteeringDelay = null;
    }
}

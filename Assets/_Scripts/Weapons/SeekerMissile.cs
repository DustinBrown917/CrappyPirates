using System.Collections;
using UnityEngine;

public class SeekerMissile : Missile
{
    private Rigidbody target;
    [SerializeField] private float delayBeforeSteering = 1.0f;
    private bool shouldSteer = false;
    private Coroutine cr_SteeringDelay = null;

    private AdvancedSeek seeker = null;

    protected override void Awake()
    {
        base.Awake();
        seeker = GetComponent<AdvancedSeek>();
    }

    public override void Arm(Rigidbody target, params Collider[] launcherColliders)
    {
        base.Arm(target, launcherColliders);

        this.target = target;

        StartDelayBeforeSteering();
    }

    public override void HandleImpact()
    {
        base.HandleImpact();
        shouldSteer = false;
        StopDelayBeforeSteering();
    }

    protected override void FixedUpdate()
    {
        if (target == null) { 
            HandleImpact();
            return;
        }
        if (!shouldSteer) { return; }
        Vector3 steering = seeker.GetSteering(target);
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

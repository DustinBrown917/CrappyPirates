using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartMissile : Missile
{
    private Stack<NavigationNode> path = new Stack<NavigationNode>();
    private Rigidbody target = null;
    public Rigidbody Target { get => target; }

    private Animator animator = null;

    private bool canSeeTarget_ = false;
    public bool CanSeeTarget
    {
        get => canSeeTarget_;
        private set {
            canSeeTarget_ = value;
            if(animator != null) {
                animator.SetBool("canSeeTarget", value);
            }
        }
    }

    public event Action FUpdate;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
    }

    protected override void FixedUpdate()
    {
        if (armed_) {
            if (target == null) {
                HandleImpact();
                return;
            }

            RaycastHit hitInfo = new RaycastHit();
            Physics.Raycast(transform.position, (target.transform.position - transform.position).normalized, out hitInfo);
            if (hitInfo.rigidbody == target) {
                CanSeeTarget = true;
            } else {
                CanSeeTarget = false;
            }

            if (maxSpeed > 0 && Body.velocity.magnitude > maxSpeed) {
                Body.velocity = Vector3.ClampMagnitude(Body.velocity, maxSpeed);
            }
        }

        FUpdate?.Invoke();
    }

    public override void Arm(Rigidbody target, params Collider[] launcherColliders)
    {
        base.Arm(target, launcherColliders);
        this.target = target;       
    }
}

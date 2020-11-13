using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartMissile : Missile
{
    private Stack<NavigationNode> path = new Stack<NavigationNode>();
    Rigidbody target = null;
    private Vector3 navTarget = new Vector3();
    private bool followPath = false;

    private float prevDistanceToNavNode = float.MaxValue;

    private AdvancedSeek seeker = null;

    protected override void Awake()
    {
        base.Awake();
        seeker = GetComponent<AdvancedSeek>();
    }

    protected override void FixedUpdate()
    {
        if (target == null) {
            HandleImpact();
            return;
        }

        if (followPath) {
            float currDistToNavNode = Vector3.Distance(navTarget, transform.position);
            if(currDistToNavNode < prevDistanceToNavNode) {
                prevDistanceToNavNode = currDistToNavNode;
            } else {
                HandleNodeReached();
            }

            RaycastHit hitInfo = new RaycastHit();
            Physics.Raycast(transform.position, (target.transform.position - transform.position).normalized, out hitInfo);
            if(hitInfo.rigidbody == target) {
                followPath = false;
            }

            transform.forward = Body.velocity;
        }

        if (!followPath) {
            Vector3 steering = seeker.GetSteering(target);
            Body.velocity += steering * Time.fixedDeltaTime;
            transform.forward = steering.normalized;
        }

        if (maxSpeed > 0 && Body.velocity.magnitude > maxSpeed) {
            Body.velocity = Vector3.ClampMagnitude(Body.velocity, maxSpeed);
        }

        
    }

    public override void Arm(Rigidbody target, params Collider[] launcherColliders)
    {
        base.Arm(target, launcherColliders);
        this.target = target;
        BeginNavigatePath();
    }

    private void FindPath(Vector3 targetPos)
    {
        NavigationNode start = VoronoiGraphBuilder.Instance.GetNearestNavNode(transform.position);
        NavigationNode end = VoronoiGraphBuilder.Instance.GetNearestNavNode(targetPos);

        Debug.Log($"Start: {start.name}, End: {end.name}");

        path = AstarNavigator.GetPath(start, end);
        Debug.Log(path.Count);
    }

    private void BeginNavigatePath()
    {
        FindPath(target.transform.position);
        HandleNodeReached();
        followPath = true;
    }

    private void HandleNodeReached()
    {
        if(path.Count > 0) {
            navTarget = path.Pop().transform.position;
            Body.velocity = (navTarget - transform.position).normalized * maxSpeed;
            prevDistanceToNavNode = Vector3.Distance(navTarget, transform.position);
        } else {
            followPath = false;
        }

    }
}

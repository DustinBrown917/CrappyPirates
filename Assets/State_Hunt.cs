using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Hunt : StateMachineBehaviour
{
    private GameObject myObject = null;
    private SmartMissile missile = null;
    private Stack<NavigationNode> path = new Stack<NavigationNode>();
    private Vector3 navTarget = new Vector3();
    private float prevDistanceToNavNode = float.MaxValue;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        myObject = animator.gameObject;
        missile = myObject.GetComponent<SmartMissile>();
        missile.FUpdate += Missile_FUpdate;
        BeginNavigatePath();
    }

    private void Missile_FUpdate()
    {
        float currDistToNavNode = Vector3.Distance(navTarget, missile.transform.position);
        if (currDistToNavNode < prevDistanceToNavNode) {
            prevDistanceToNavNode = currDistToNavNode;
        } else {
            HandleNodeReached();
        }

        missile.transform.forward = missile.Body.velocity;
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(missile != null) {
            missile.FUpdate -= Missile_FUpdate;
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}

    public void BeginNavigatePath()
    {
        FindPath(missile.Target.transform.position);
        HandleNodeReached();
    }

    private void FindPath(Vector3 targetPos)
    {
        NavigationNode start = VoronoiGraphBuilder.Instance.GetNearestNavNode(myObject.transform.position);
        NavigationNode end = VoronoiGraphBuilder.Instance.GetNearestNavNode(targetPos);

        path = AstarNavigator.GetPath(start, end);
    }

    private void HandleNodeReached()
    {
        if (path.Count > 0) {
            navTarget = path.Pop().transform.position;
            missile.Body.velocity = (navTarget - missile.transform.position).normalized * missile.MaxSpeed;
            prevDistanceToNavNode = Vector3.Distance(navTarget, missile.transform.position);
        } else if(missile.Target != null) {
            BeginNavigatePath();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Seek : StateMachineBehaviour
{
    private AdvancedSeek seeker = null;
    private Rigidbody body = null;
    private Rigidbody target = null;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        seeker = animator.GetComponent<AdvancedSeek>();
        body = animator.GetComponent<Rigidbody>();
        target = seeker.GetComponent<SmartMissile>().Target;
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector3 steering = seeker.GetSteering(target);
        body.velocity += steering * Time.fixedDeltaTime;
        seeker.transform.forward = steering.normalized;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
}

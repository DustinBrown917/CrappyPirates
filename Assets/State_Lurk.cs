using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Lurk : StateMachineBehaviour
{
    [SerializeField] private float lurkDistance = 4.0f;
    [SerializeField] private float lurkRange = 1.0f;
    [SerializeField] private float decelerationRate = 0.97f;

    private AdvancedSeek seeker = null;
    private Rigidbody body = null;
    private Rigidbody target = null;

    private DefensesModule targetDefenseModule = null;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        seeker = animator.GetComponent<AdvancedSeek>();
        body = animator.GetComponent<Rigidbody>();
        target = seeker.GetComponent<SmartMissile>().Target;
        targetDefenseModule = target.GetComponent<DefensesModule>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float distanceToTarget = Vector3.Distance(seeker.transform.position, target.transform.position);
        Vector3 steering = new Vector3();

        //If we are outside of lurk range, steer towards lurk range
        if(distanceToTarget > lurkDistance + (lurkRange * 0.5f) || distanceToTarget < lurkDistance - (lurkRange * 0.5f)) {
            Vector3 nrm = (seeker.transform.position - target.transform.position).normalized;
            steering = seeker.GetSteering(target.transform.position + (nrm * lurkDistance));
        } else if(body.velocity.magnitude > 0) {
            body.velocity *= decelerationRate;
        }

        body.velocity += steering * Time.fixedDeltaTime;

        if(steering == Vector3.zero) {
            seeker.transform.forward = (target.transform.position - seeker.transform.position).normalized;
        } else {
            seeker.transform.forward = steering;
        }


        animator.SetBool("targetDefensesAreUp", targetDefenseModule.DefensesViable);
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

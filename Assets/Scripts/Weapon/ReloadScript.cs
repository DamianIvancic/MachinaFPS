using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadScript : StateMachineBehaviour {

    public float ReloadTime = 0.9f;
    private bool Reloaded;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Reloaded = false;
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.normalizedTime >= ReloadTime)
        {
            if (!Reloaded) //only updates the amount of ammo once the animation is 90% complete
            {
                animator.GetComponent<Weapon>().Reload();
                Reloaded = true;
            }

            if (stateInfo.normalizedTime >= 1.0f)
            {         
                animator.SetBool("Fire", false);
                animator.SetBool("Reload", false);
            }
        }	
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}

using UnityEngine;
using System.Collections;

public class PlayerAnimationStateVarsProvider : MonoBehaviour {

    PlayerStatusProvider myStatusProvider;
    Animator myAnimator;

	// Use this for initialization
	void Start () {
        myStatusProvider = GetComponent<PlayerStatusProvider>();
        myAnimator = GetComponentInChildren<Animator>();
        if(myStatusProvider == null)
        {
            Debug.Log("PlayerStatusProvider n'a pas pu etre trouve dans le joueur " + name);
        }
        if (myAnimator == null)
        {
            Debug.Log("Animator n'a pas pu etre trouve dans le joueur " + name);
        }

        myStatusProvider.OnAxisHChanged += AxisHChangedAction;
        myStatusProvider.OnVelocityYChanged += VelocityYChangedAction;
        myStatusProvider.OnGroundedStatusChanged += GroundedAction;
        myStatusProvider.OnOnWallStatusChanged += OnWallAction;
        myStatusProvider.OnDashForwardAction += DashForwardAction;
        myStatusProvider.OnDashUpAction += DashUpAction;
        myStatusProvider.OnDashDownAction += DashDownAction;
        myStatusProvider.OnKnockBackAction += KnockBackAction;
        myStatusProvider.OnDieAction += DieAction;
        myStatusProvider.OnRespawnWarningAction += RespawnWarningAction;
	}

    public void AxisHChangedAction(float axisHValue)
    {
        myAnimator.SetFloat("VelocityX", axisHValue);
    }
   
    public void VelocityYChangedAction(float velocityYValue)
    {
        myAnimator.SetFloat("VelocityY", velocityYValue);
    }


    public void GroundedAction(bool isGrounded)
    {
        myAnimator.SetBool("Grounded", isGrounded);
    }


    public void OnWallAction(bool isOnWall)
    {
        myAnimator.SetBool("Riding", isOnWall);
    }


    public void DashForwardAction()
    {
        myAnimator.SetTrigger("ForwardDash");
    }


    public void DashUpAction()
    {
        myAnimator.SetTrigger("UpwardDash");
    }


    public void DashDownAction()
    {
        myAnimator.SetTrigger("DownwardDash");
    }


    public void KnockBackAction()
    {
        myAnimator.SetTrigger("KnockBack");
    }


    public void DieAction()
    {
        myAnimator.SetTrigger("Die");
    }

    public void RespawnWarningAction()
    {
        myAnimator.SetTrigger("Rewpawn");
    }
}

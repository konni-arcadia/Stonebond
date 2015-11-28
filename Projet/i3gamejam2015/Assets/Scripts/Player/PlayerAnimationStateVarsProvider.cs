using UnityEngine;
using System.Collections;

public class PlayerAnimationStateVarsProvider : MonoBehaviour {

    PlayerStatusProvider myStatusProvider;
    Animator myAnimator;

	// Use this for initialization
	void Awake () {
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
        myStatusProvider.OnAttackForwardAction += AttackForwardAction;
        myStatusProvider.OnAttackUpAction += AttackUpAction;
        myStatusProvider.OnAttackDownAction += AttackDownAction;
		myStatusProvider.OnAttackSpecialAction += AttackSpecialAction;
        myStatusProvider.OnKnockBackAction += KnockBackAction;
        myStatusProvider.OnDieAction += DieAction;
        myStatusProvider.OnRespawnWarningAction += RespawnWarningAction;
	}

    public void AxisHChangedAction(float axisHValue)
    {
        if(myAnimator != null)
            myAnimator.SetFloat("VelocityX", axisHValue);
    }
   
    public void VelocityYChangedAction(float velocityYValue)
    {
        if (myAnimator != null)
            myAnimator.SetFloat("VelocityY", velocityYValue);
    }


    public void GroundedAction(bool isGrounded)
    {
        if (myAnimator != null)
            myAnimator.SetBool("Grounded", isGrounded);
    }


    public void OnWallAction(bool isOnWall)
    {
        if (myAnimator != null)
            myAnimator.SetBool("Riding", isOnWall);
    }


    public void AttackForwardAction()
    {
        if (myAnimator != null)
			myAnimator.SetTrigger("SpecialAttack");
    }


    public void AttackUpAction()
    {
        if (myAnimator != null)
            myAnimator.SetTrigger("UpwardAttack");
    }


    public void AttackDownAction()
    {
        if (myAnimator != null)
            myAnimator.SetTrigger("DownwardAttack");
    }

	public void AttackSpecialAction()
	{
		if (myAnimator != null)
			myAnimator.SetTrigger("SpecialAttack");
	}

    public void KnockBackAction()
    {
        if (myAnimator != null)
            myAnimator.SetTrigger("KnockBack");
    }


    public void DieAction(Vector2 deathVector)
    {
        if (myAnimator != null)
            myAnimator.SetTrigger("Die");
    }

    public void RespawnWarningAction()
    {
        if (myAnimator != null)
            myAnimator.SetTrigger("Respawn");
    }

    public void OnInvincibleStatusChangedAction(bool isInvicible)
    {
        if (myAnimator != null)
            myAnimator.SetBool("Invincible", isInvicible);
    }
}

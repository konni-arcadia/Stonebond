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
        myStatusProvider.OnGrindingStatusChanged += OnWallAction;
        myStatusProvider.OnAttackStartAction += HandleOnAttackStartAction;
        myStatusProvider.OnAttackStopAction += HandleOnAttackStopAction;
		myStatusProvider.OnChargeStartAction += ChargeStartAction;
		myStatusProvider.OnChargeStopAction += ChargeStopAction;		
        myStatusProvider.OnVerticalKnockbackAction += VerticalKnockbackAction;
		myStatusProvider.OnHorizontalKnockbackAction += HorizontalKnockbackAction;
        myStatusProvider.OnDieAction += DieAction;
        myStatusProvider.OnRespawnWarningAction += RespawnAction;
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

    private void HandleOnAttackStartAction(PlayerStatusProvider.AttackType attackType, Vector2 direction)
    {
        if (myAnimator == null)
        {
            return;
        }

        switch (attackType)
        {
            case PlayerStatusProvider.AttackType.FORWARD:
                myAnimator.SetTrigger("ForwardAttack");
                break;
            case PlayerStatusProvider.AttackType.UP:
                myAnimator.SetTrigger("UpwardAttack");
                break;
            case PlayerStatusProvider.AttackType.DOWN:
                myAnimator.SetTrigger("DownwardAttack");
                break;
            case PlayerStatusProvider.AttackType.SPECIAL:
                myAnimator.SetTrigger("SpecialAttack");
                break;
        }
    }

    private void HandleOnAttackStopAction (PlayerStatusProvider.AttackType attackType, bool cancelled)
    {
        if (myAnimator == null)
        {
            return;
        }

        if(cancelled)
        {           
            myAnimator.SetTrigger("CancelAttack");
        }
    }

	public void ChargeStartAction()
	{
		if (myAnimator != null)
			myAnimator.SetBool("Charge", true);
	}

	public void ChargeStopAction(bool complete)
	{
		if (myAnimator != null)
			myAnimator.SetBool("Charge", false);
	}       

    public void HorizontalKnockbackAction()
    {
        if (myAnimator != null)
            myAnimator.SetTrigger("KnockBack");
    }

	public void VerticalKnockbackAction()
	{
		if (myAnimator != null)
			myAnimator.SetTrigger("KnockBack");
	}

    public void DieAction(Transform source, Vector2 deathVector)
    {
        if (myAnimator != null)
            myAnimator.SetTrigger("Die");
    }

    public void RespawnAction(bool initial)
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

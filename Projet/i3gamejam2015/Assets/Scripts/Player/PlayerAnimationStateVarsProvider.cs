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
        myStatusProvider.OnAttackForwardAction += AttackForwardAction;
        myStatusProvider.OnAttackUpAction += AttackUpAction;
        myStatusProvider.OnAttackDownAction += AttackDownAction;
		myStatusProvider.OnChargeStartAction += ChargeStartAction;
		myStatusProvider.OnChargeStopAction += ChargeStopAction;
		myStatusProvider.OnAttackSpecialAction += AttackSpecialAction;
        myStatusProvider.OnVerticalKnockbackAction += VerticalKnockbackAction;
		myStatusProvider.OnHorizontalKnockbackAction += HorizontalKnockbackAction;
        myStatusProvider.OnDieAction += DieAction;
        myStatusProvider.OnRespawnAction += RespawnAction;
        myStatusProvider.OnHitGroundAction += HandleOnHitGroundAction;
        myStatusProvider.OnHitWallAction += HandleOnHitWallAction;
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
			myAnimator.SetTrigger("ForwardAttack");
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

	public void AttackSpecialAction()
	{
		if (myAnimator != null)
			myAnimator.SetTrigger("SpecialAttack");
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

    private void HandleOnHitWallAction (PlayerStatusProvider.WallCollisionType collisionType, Vector2 velocity)
    {
        if (collisionType != PlayerStatusProvider.WallCollisionType.NORMAL)
        {
            if (myAnimator != null)
            {
                myAnimator.SetTrigger("CancelAttack");
            }
        }
    }
    
    private void HandleOnHitGroundAction (PlayerStatusProvider.GroundCollisionType collisionType, Vector2 velocity)
    {
        if (collisionType != PlayerStatusProvider.GroundCollisionType.NORMAL)
        {
            if (myAnimator != null)
            {
                myAnimator.SetTrigger("CancelAttack");
            }
        }
    }

}

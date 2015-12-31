using UnityEngine;
using System.Collections;

public class PlayerStatusProvider : MonoBehaviour {

    public delegate void AxisHChangedAction(float axisHValue);
    public event AxisHChangedAction OnAxisHChanged;
    public void setAxisHValue(float value)
    {
        if (OnAxisHChanged != null) OnAxisHChanged(value);
    }

    public delegate void VelocityYChangedAction(float velocityYValue);
    public event VelocityYChangedAction OnVelocityYChanged;
    public void setVelocityYValue(float velocityY)
    {
        if (OnVelocityYChanged != null) OnVelocityYChanged(velocityY);
    }

    public delegate void GroundedAction(bool isGrounded);
    public event GroundedAction OnGroundedStatusChanged;
    public void setGroundedStatus(bool isGrounded)
    {
        if (OnGroundedStatusChanged != null) OnGroundedStatusChanged(isGrounded);
    }

    public delegate void GridingAction(bool isGrinding);
    public event GridingAction OnGrindingStatusChanged;
    public void setGrindingStatus(bool isGrinding)
    {
        if (OnGrindingStatusChanged != null) OnGrindingStatusChanged(isGrinding);
    }

	public delegate void JumpAction();
	public event JumpAction onJumpAction;
	public void setJump()
	{
		if (onJumpAction != null) onJumpAction();
	}

	public delegate void WallJumpAction();
	public event JumpAction onWallJumpAction;
	public void setWallJump()
	{
		if (onWallJumpAction != null) onWallJumpAction();
	}

	public delegate void AttackForwardAction();
    public event AttackForwardAction OnAttackForwardAction;
    public void setAttackForward()
    {
        if (OnAttackForwardAction != null) OnAttackForwardAction();
    }

    public delegate void AttackUpAction();
    public event AttackUpAction OnAttackUpAction;
    public void setAttackUp()
    {
        if (OnAttackUpAction != null) OnAttackUpAction();
    }

    public delegate void AttackDownAction();
    public event AttackDownAction OnAttackDownAction;
    public void setAttackDown()
    {
        if (OnAttackDownAction != null) OnAttackDownAction();
    }

	public delegate void ChargeStartAction();
	public event ChargeStartAction OnChargeStartAction;
	public void setChargeStart()
	{
		if (OnChargeStartAction != null) OnChargeStartAction();
	}

	public delegate void ChargeStopAction(bool complete);
	public event ChargeStopAction OnChargeStopAction;
	public void setChargeStop(bool complete)
	{
		if (OnChargeStopAction != null) OnChargeStopAction(complete);
	}

	public delegate void AttackSpecialAction();
	public event AttackForwardAction OnAttackSpecialAction;
	public void setAttackSpecial()
	{
		if (OnAttackSpecialAction != null) OnAttackSpecialAction();
	}

	public delegate void AttackFailedAction();
	public event AttackFailedAction OnAttackFailedAction;
	public void setAttackFailed()
	{
		if (OnAttackFailedAction != null) OnAttackFailedAction();
	}

	public delegate void VerticalKnockbackAction();
	public event VerticalKnockbackAction OnVerticalKnockbackAction;
    public void setVerticalKnockback()
    {
		if (OnVerticalKnockbackAction != null) OnVerticalKnockbackAction();
    }

	public delegate void HorizontalKnockbackAction();
	public event HorizontalKnockbackAction OnHorizontalKnockbackAction;
	public void setHorizontalKnockback()
	{
		if (OnHorizontalKnockbackAction != null) OnHorizontalKnockbackAction();
	}

	public delegate void DieAction(Vector2 attackDirection);
    public event DieAction OnDieAction;
	public void setDie(Vector2 attackDirection)
    {
		if (OnDieAction != null) OnDieAction(attackDirection);
    }

    public delegate void RespawnAction(bool initial);
    public event RespawnAction OnRespawnAction;
    public void setRespawn(bool initial)
    {
		if (OnRespawnAction != null) OnRespawnAction(initial);
    }

    public delegate void InvinsibleStateAction(bool isInvinsible);
    public event InvinsibleStateAction OnInvinsibleChangedAction;
    public void setInvincibleStatus(bool isInvincible)
    {
		if (OnInvinsibleChangedAction != null) OnInvinsibleChangedAction(isInvincible);
    }

	public delegate void BoundStateAction(bool isBounded);
	public event BoundStateAction OnBoundChangedAction;
	public void setBoundStatus(bool isBounded)
	{
		if (OnBoundChangedAction != null) OnBoundChangedAction(isBounded);
	}
}

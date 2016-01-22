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

    public delegate void OnWallAction(bool onWall);
    public event OnWallAction OnOnWallStatusChanged;
    public void setOnWall(bool onWall)
    {
        if (OnOnWallStatusChanged != null) OnOnWallStatusChanged(onWall);
    }

    public delegate void GridingAction(bool isGrinding);
    public event GridingAction OnGrindingStatusChanged;
    public void setGrindingStatus(bool isGrinding)
    {
        if (OnGrindingStatusChanged != null) OnGrindingStatusChanged(isGrinding);
    }

    public enum CollisionType
    {
        GROUND,
        GROUND_ATTACK,
        WALL,
        WALL_ATTACK,
        SPECIAL_ATTACK
    }
    public delegate void CollisionAction(CollisionType collisionType, Vector2 velocity);
    public event CollisionAction OnCollisionAction;
    public void setCollision(CollisionType collisionType, Vector2 velocity)
    {
        if (OnCollisionAction != null) OnCollisionAction(collisionType, velocity);
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

    public enum AttackType
    {
        FORWARD,
        UP,
        DOWN,
        SPECIAL
    }

    public delegate void AttackStartAction(AttackType attackType, Vector2 direction);
    public event AttackStartAction OnAttackStartAction;
    public void setAttackStart(AttackType attackType, Vector2 direction)
    {
        if (OnAttackStartAction != null) OnAttackStartAction(attackType, direction);
    }

    public delegate void AttackStopAction(AttackType attackType, bool cancelled);
    public event AttackStopAction OnAttackStopAction;
    public void setAttackStop(AttackType attackType, bool cancelled)
    {
        if (OnAttackStopAction != null) OnAttackStopAction(attackType, cancelled);
    }

	public delegate void ChargeStartAction();
	public event ChargeStartAction OnChargeStartAction;
	public void setChargeStart()
	{
		if (OnChargeStartAction != null) OnChargeStartAction();
	}

	public delegate void ChargeReadyAction();
	public event ChargeReadyAction OnChargeReadyAction;
	public void setChargeReady()
	{
		if (OnChargeReadyAction != null) OnChargeReadyAction();
	}

	public delegate void ChargeFullAction();
	public event ChargeFullAction OnChargeFullAction;
	public void setChargeFull()
	{
		if (OnChargeFullAction != null) OnChargeFullAction();
	}

	public delegate void ChargeStopAction(bool complete);
	public event ChargeStopAction OnChargeStopAction;
	public void setChargeStop(bool complete)
	{
		if (OnChargeStopAction != null) OnChargeStopAction(complete);
	}

	public enum ChargeState
	{
		CHARGE,
		READY,
		FULL
	}
	public delegate void ChargeUpdateAction(ChargeState state, float statePct, float forceRatio);
	public event ChargeUpdateAction OnChargeUpdateAction;
	public void setChargeUpdate(ChargeState state, float statePct, float forceRatio)
	{
		if (OnChargeUpdateAction != null) OnChargeUpdateAction (state, statePct, forceRatio);
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

    // triggered before the attack pause
    public delegate void DieWarningAction(Transform source, Vector2 attackDirection);
    public event DieAction OnDieWarningAction;
    public void setDieWarning(Transform source, Vector2 attackDirection)
    {
        if (OnDieWarningAction != null) OnDieWarningAction(source, attackDirection);
    }

    public delegate void DieAction(Transform source, Vector2 attackDirection);
    public event DieAction OnDieAction;
    public void setDie(Transform source, Vector2 attackDirection)
    {
		if (OnDieAction != null) OnDieAction(source, attackDirection);
    }

    public delegate void RespawnWarningAction(bool initial);
    public event RespawnWarningAction OnRespawnWarningAction;
    public void setRespawnWarning(bool initial)
    {
        if (OnRespawnWarningAction != null) OnRespawnWarningAction(initial);
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

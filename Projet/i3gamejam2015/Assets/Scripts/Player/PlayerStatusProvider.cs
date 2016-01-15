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

	public delegate void AttackSpecialStartAction(Vector2 direction);
    public event AttackSpecialStartAction OnAttackSpecialStartAction;
	public void setAttackSpecialStart(Vector2 direction)
	{
        if (OnAttackSpecialStartAction != null) OnAttackSpecialStartAction(direction);
	}

    public delegate void AttackSpecialStopAction();
    public event AttackSpecialStopAction OnAttackSpecialStopAction;
    public void setAttackSpecialStop()
    {
        if (OnAttackSpecialStopAction != null) OnAttackSpecialStopAction();
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

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

    public delegate void OnWallAction(bool isOnWall);
    public event OnWallAction OnOnWallStatusChanged;
    public void setOnWallStatus(bool isOnWall)
    {
        if (OnOnWallStatusChanged != null) OnOnWallStatusChanged(isOnWall);
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

	public delegate void AttackSpecialAction();
	public event AttackForwardAction OnAttackSpecialAction;
	public void setAttackSpecial()
	{
		if (OnAttackSpecialAction != null) OnAttackSpecialAction();
	}

	public delegate void KnockBackAction();
    public event KnockBackAction OnKnockBackAction;
    public void setKnockBackUp()
    {
        OnKnockBackAction();
    }
	public void setKnockBackDown()
	{
		if (OnKnockBackAction != null) OnKnockBackAction();
	}
	public void setKnockBackForward()
	{
		if (OnKnockBackAction != null) OnKnockBackAction();
	}

    public delegate void DieAction(Vector2 deathVector);
    public event DieAction OnDieAction;
    public void setDie()
    {
        setDie(Vector2.zero);
    }
    public void setDie(Vector2 deathVector)
    {
        if (OnDieAction != null) OnDieAction(deathVector);
    }

    public delegate void RespawnWarningAction();
    public event RespawnWarningAction OnRespawnWarningAction;
    public void setRespawnWarning()
    {
        if (OnRespawnWarningAction != null) OnRespawnWarningAction();
    }

    public delegate void InvinsibleStateAction();
    public event InvinsibleStateAction OnInvinsibleChangedAction;
    public void setInvincibleStatus(bool isInvincible)
    {
		if (OnInvinsibleChangedAction != null) OnInvinsibleChangedAction();
    }
}

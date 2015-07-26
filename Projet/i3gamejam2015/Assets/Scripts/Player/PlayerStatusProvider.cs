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

    public delegate void DashForwardAction();
    public event DashForwardAction OnDashForwardAction;
    public void setDashForward()
    {
        if (OnDashForwardAction != null) OnDashForwardAction();
    }

    public delegate void DashUpAction();
    public event DashUpAction OnDashUpAction;
    public void setDashUp()
    {
        if (OnDashUpAction != null) OnDashUpAction();
    }

    public delegate void DashDownAction();
    public event DashDownAction OnDashDownAction;
    public void setDashDown()
    {
        if (OnDashDownAction != null) OnDashDownAction();
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

using UnityEngine;
using System.Collections;

public class PlayerStatusProvider : MonoBehaviour {

    public delegate void AxisHChangedAction(float axisHValue);
    public event AxisHChangedAction OnAxisHChanged;
    public void setAxisHValue(float value)
    {
        OnAxisHChanged(value);
    }

    public delegate void VelocityYChangedAction(float velocityYValue);
    public event VelocityYChangedAction OnVelocityYChanged;
    public void setVelocityYValue(float velocityY)
    {
        OnVelocityYChanged(velocityY);
    }

    public delegate void GroundedAction(bool isGrounded);
    public event GroundedAction OnGroundedStatusChanged;
    public void setGroundedStatus(bool isGrounded)
    {
        OnGroundedStatusChanged(isGrounded);
    }

    public delegate void OnWallAction(bool isOnWall);
    public event OnWallAction OnOnWallStatusChanged;
    public void setOnWallStatus(bool isOnWall)
    {
        OnOnWallStatusChanged(isOnWall);
    }

    public delegate void DashForwardAction();
    public event DashForwardAction OnDashForwardAction;
    public void setDashForward()
    {
        OnDashForwardAction();
    }

    public delegate void DashUpAction();
    public event DashUpAction OnDashUpAction;
    public void setDashUp()
    {
        OnDashUpAction();
    }

    public delegate void DashDownAction();
    public event DashDownAction OnDashDownAction;
    public void setDashDown()
    {
        OnDashDownAction();
    }

    public delegate void KnockBackAction();
    public event KnockBackAction OnKnockBackAction;
    public void setKnockBackUp()
    {
        OnKnockBackAction();
    }
	public void setKnockBackDown()
	{
		OnKnockBackAction();
	}
	public void setKnockBackForward()
	{
		OnKnockBackAction();
	}

    public delegate void DieAction();
    public event DieAction OnDieAction;
    public void setDie()
    {
        OnDieAction();
    }

    public delegate void RespawnWarningAction();
    public event RespawnWarningAction OnRespawnWarningAction;
    public void setRespawnWarning()
    {
        OnRespawnWarningAction();
    }
}

﻿using UnityEngine;
using System.Collections;

public class PlayerSoundManager : MonoBehaviour
{
    public PlayerStatusProvider statusProvider;

    bool prevIsGroundedFlag = true;

    void Start()
    {
        statusProvider.OnBoundChangedAction += OnBounded;
        statusProvider.OnGroundedStatusChanged += OnGrounded;
        statusProvider.OnOnWallStatusChanged += OnWallrided;
        statusProvider.OnKnockBackAction += onKnockbacked;
        statusProvider.OnDieAction += onPlayerDied;
        statusProvider.onJumpAction += OnJumped;
        statusProvider.onWallJumpAction += OnWallJumped;
        statusProvider.OnAttackUpAction += OnAttack;
        statusProvider.OnAttackSpecialAction += OnAttack;
        statusProvider.OnAttackForwardAction += OnAttack;
        statusProvider.OnAttackDownAction += OnAttack;
        statusProvider.OnRespawnWarningAction += OnRespawned;
    }

    private void OnRespawned()
    {
        SoundManager.Instance.GAMEPLAY_Rebirth();
    }

    private void OnAttack()
    {
        SoundManager.Instance.GAMEPLAY_Attack();
    }

    private void OnWallJumped()
    { 
        SoundManager.Instance.GAMEPLAY_Walljump();
    }

    private void OnJumped()
    {
        SoundManager.Instance.GAMEPLAY_Jump();
    }

    private void OnBounded(bool isBounded)
    {
        if (isBounded)
        {
            SoundManager.Instance.StartBound();
        }
        else
        {
            SoundManager.Instance.StopBound();
        }
    }

    private void OnGrounded(bool isGrounded)
    {
        if (isGrounded && !prevIsGroundedFlag)
        {
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.GAMEPLAY_Land();
            }
        }
        prevIsGroundedFlag = isGrounded;
    }

    private void OnWallrided(bool isOnWall)
    {
        if (isOnWall)
        {
            //TODO
        }
        else
        {
            //TODO
        }
    }

    private void onKnockbacked()
    {
        SoundManager.Instance.GAMEPLAY_Knockback();
    }

    private void onPlayerDied(Vector2 deathVector)
    {
        SoundManager.Instance.GAMEPLAY_Death();
    }

    //void Update() { }
}

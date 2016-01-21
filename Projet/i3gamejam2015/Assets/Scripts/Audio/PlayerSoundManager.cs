using UnityEngine;
using System.Collections;

public class PlayerSoundManager : MonoBehaviour
{
    public PlayerStatusProvider statusProvider;
       
    void Start()
    {
        statusProvider.OnBoundChangedAction += OnBounded;
        statusProvider.OnCollisionAction += OnCollision;
        statusProvider.OnGrindingStatusChanged += OnWallrided;
        statusProvider.OnHorizontalKnockbackAction += onKnockbacked;
		statusProvider.OnVerticalKnockbackAction += onKnockbacked;
        statusProvider.OnDieAction += onPlayerDied;
        statusProvider.onJumpAction += OnJumped;
        statusProvider.onWallJumpAction += OnWallJumped;
        statusProvider.OnAttackStartAction += OnAttackStart;		
        statusProvider.OnRespawnWarningAction += OnRespawned;
		statusProvider.OnChargeStartAction += OnChargeAction;
		statusProvider.OnChargeStopAction += OnStopChargeAction;
    }

	private void OnChargeAction()
	{
		AudioSingleton<SfxAudioManager>.Instance.PlayChargeSpecialAttack();
	}
	private void OnStopChargeAction(bool complete)
	{
		AudioSingleton<SfxAudioManager>.Instance.StopChargeSpecialAttack();
	}

    private void OnRespawned(bool initial)
    {
		AudioSingleton<SfxAudioManager>.Instance.PlayReBirth(initial);
    }

    private void OnAttackStart(PlayerStatusProvider.AttackType attackType, Vector2 direction)
    {
        switch (attackType)
        {
            case PlayerStatusProvider.AttackType.FORWARD:
                AudioSingleton<SfxAudioManager>.Instance.PlayAttack();
                break;
            case PlayerStatusProvider.AttackType.UP:
                AudioSingleton<SfxAudioManager>.Instance.PlayAttack();
                break;
            case PlayerStatusProvider.AttackType.DOWN:
                AudioSingleton<SfxAudioManager>.Instance.PlayAttack();
                break;
            case PlayerStatusProvider.AttackType.SPECIAL:
                AudioSingleton<SfxAudioManager>.Instance.PlayAttack();
                break;
        }
    }
	
    private void OnWallJumped()
    { 
		AudioSingleton<SfxAudioManager>.Instance.PlayWallJump();
    }

    private void OnJumped()
    {
		AudioSingleton<SfxAudioManager>.Instance.PlayJump();
    }

    private void OnBounded(bool isBounded)
    {
		if (isBounded) 
		{
			AudioSingleton<SfxAudioManager>.Instance.PlayStartBound();
		}
		else 
		{
			AudioSingleton<SfxAudioManager>.Instance.PlayStopBound();
		}
    }

    private void OnCollision(PlayerStatusProvider.CollisionType collisionType, Vector2 velocity)
    {        
        switch (collisionType)
        {
            case PlayerStatusProvider.CollisionType.GROUND:
                AudioSingleton<SfxAudioManager>.Instance.PlayLand();
                break;
            case PlayerStatusProvider.CollisionType.GROUND_ATTACK:
                AudioSingleton<SfxAudioManager>.Instance.PlayLand();
                break;
            case PlayerStatusProvider.CollisionType.WALL:
                // no sound on wall hit
                break;
            case PlayerStatusProvider.CollisionType.WALL_ATTACK:
                // no sound on wall hit
                break;
            case PlayerStatusProvider.CollisionType.SPECIAL_ATTACK:
                AudioSingleton<SfxAudioManager>.Instance.PlayLand();
                break;
        }
    }

    private void OnWallrided(bool isOnWall)
    {
        if (isOnWall)
        {
			AudioSingleton<SfxAudioManager>.Instance.PlayWallSlide();
        }
        else
        {
			AudioSingleton<SfxAudioManager>.Instance.StopWallSlide();
        }
    }

    private void onKnockbacked()
    {
		AudioSingleton<SfxAudioManager>.Instance.PlayKnockBack();
    }

	private void onPlayerDied(Transform source, Vector2 attackDirection)
    {
		AudioSingleton<SfxAudioManager>.Instance.PlayDeath();
    }
}

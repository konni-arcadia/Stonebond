using UnityEngine;
using System.Collections;

public class PlayerSoundManager : MonoBehaviour
{
    public PlayerStatusProvider statusProvider;

    bool prevIsGroundedFlag = true;

    void Start()
    {
        statusProvider.OnBoundChangedAction += OnBounded;
        statusProvider.OnGroundedStatusChanged += OnGrounded;
        statusProvider.OnGrindingStatusChanged += OnWallrided;
        statusProvider.OnHorizontalKnockbackAction += onKnockbacked;
		statusProvider.OnVerticalKnockbackAction += onKnockbacked;
        statusProvider.OnDieAction += onPlayerDied;
        statusProvider.onJumpAction += OnJumped;
        statusProvider.onWallJumpAction += OnWallJumped;
        statusProvider.OnAttackStartAction += OnAttackStart;		
        statusProvider.OnRespawnAction += OnRespawned;
    }

    private void OnRespawned(bool initial)
    {
		AudioSingleton<SfxAudioManager>.Instance.PlayReBirth();
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

    private void OnGrounded(bool isGrounded)
    {
        if (isGrounded && !prevIsGroundedFlag)
        {
			AudioSingleton<SfxAudioManager>.Instance.PlayLand();
        }
        prevIsGroundedFlag = isGrounded;
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

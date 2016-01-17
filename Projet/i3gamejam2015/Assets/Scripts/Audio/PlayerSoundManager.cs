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
        statusProvider.OnAttackUpAction += OnAttack;
		statusProvider.OnAttackSpecialStartAction += OnSpecialAttack;
        statusProvider.OnAttackForwardAction += OnAttack;
        statusProvider.OnAttackDownAction += OnAttack;
        statusProvider.OnRespawnAction += OnRespawned;
    }

    private void OnRespawned(bool initial)
    {
		AudioSingleton<SfxAudioManager>.Instance.PlayReBirth();
    }

    private void OnAttack()
    {
		AudioSingleton<SfxAudioManager>.Instance.PlayAttack();
    }

	private void OnSpecialAttack(Vector2 direction)
	{
		AudioSingleton<SfxAudioManager>.Instance.PlayAttack(); // TODO sound for special action
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
            //TODO
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

    //void Update() { }
}

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
        statusProvider.OnAttackUpAction += OnAttack;
        statusProvider.OnAttackSpecialStartAction += OnAttackSpecialStart;
        statusProvider.OnAttackSpecialStopAction += OnAttackSpecialStop;
        statusProvider.OnAttackForwardAction += OnAttack;
        statusProvider.OnAttackDownAction += OnAttack;
        statusProvider.OnRespawnAction += OnRespawned;
    }

    private void OnRespawned(bool initial)
    {
		if (SoundManager.Instance != null) {
			SoundManager.Instance.GAMEPLAY_Rebirth ();
		}
    }

    private void OnAttack()
    {
		if (SoundManager.Instance != null) {
			SoundManager.Instance.GAMEPLAY_Attack ();
		}
    }

    private void OnAttackSpecialStart(Vector2 direction)
    {
        if (SoundManager.Instance != null) {
            SoundManager.Instance.GAMEPLAY_Attack ();
        }
    }

    private void OnAttackSpecialStop()
    {
        if (SoundManager.Instance != null) {
            SoundManager.Instance.GAMEPLAY_Land ();
        }
    }

    private void OnWallJumped()
    { 
		if (SoundManager.Instance != null) {
			SoundManager.Instance.GAMEPLAY_Walljump ();
		}
    }

    private void OnJumped()
    {
		if (SoundManager.Instance != null) {
			SoundManager.Instance.GAMEPLAY_Jump ();
		}
    }

    private void OnBounded(bool isBounded)
    {
		if (SoundManager.Instance != null) {
			if (isBounded) {
				SoundManager.Instance.StartBound ();
			} else {
				SoundManager.Instance.StopBound ();
			}
		}
    }

    private void OnCollision(PlayerStatusProvider.CollisionType collisionType, Vector2 velocity)
    {
        if (SoundManager.Instance == null)
        {
            return;
        }

        switch (collisionType)
        {
            case PlayerStatusProvider.CollisionType.GROUND:
                SoundManager.Instance.GAMEPLAY_Land();
                break;
            case PlayerStatusProvider.CollisionType.GROUND_ATTACK:
                SoundManager.Instance.GAMEPLAY_Land();
                break;
            case PlayerStatusProvider.CollisionType.WALL:
                // don't play any sound for normal wall collision
                break;
            case PlayerStatusProvider.CollisionType.WALL_ATTACK:
                SoundManager.Instance.GAMEPLAY_Land();
                break;
            case PlayerStatusProvider.CollisionType.SPECIAL_ATTACK:
                SoundManager.Instance.GAMEPLAY_Land();
                break;
        }
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
		if (SoundManager.Instance != null) {
			SoundManager.Instance.GAMEPLAY_Knockback ();
		}
    }

    private void onPlayerDied(Transform source, Vector2 deathVector)
    {
		if (SoundManager.Instance != null) {
			SoundManager.Instance.GAMEPLAY_Death ();
		}
    }

    //void Update() { }
}

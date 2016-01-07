using UnityEngine;
using System.Collections;

public class PlayerSoundManager : MonoBehaviour
{
    public PlayerStatusProvider statusProvider;

    void Start()
    {
        statusProvider.OnBoundChangedAction += OnBounded;
        statusProvider.OnHitGroundAction += OnHitGround;
        statusProvider.OnHitWallAction += OnHitWall;
        statusProvider.OnGrindingStatusChanged += OnWallrided;
        statusProvider.OnHorizontalKnockbackAction += onKnockbacked;
		statusProvider.OnVerticalKnockbackAction += onKnockbacked;
        statusProvider.OnDieAction += onPlayerDied;
        statusProvider.onJumpAction += OnJumped;
        statusProvider.onWallJumpAction += OnWallJumped;
        statusProvider.OnAttackUpAction += OnAttack;
        statusProvider.OnAttackSpecialAction += OnAttack;
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

    private void OnHitGround(PlayerStatusProvider.GroundCollisionType collisionType, Vector2 velocity)
    {
        if (SoundManager.Instance == null)
        {
            return;
        }

        switch (collisionType)
        {
           case PlayerStatusProvider.GroundCollisionType.NORMAL:
                SoundManager.Instance.GAMEPLAY_Land();
                break;
            case PlayerStatusProvider.GroundCollisionType.ATTACK:
                SoundManager.Instance.GAMEPLAY_Land();
                break;
        }
    }

    private void OnHitWall(PlayerStatusProvider.WallCollisionType collisionType, Vector2 velocity)
    {
        if (SoundManager.Instance == null)
        {
            return;
        }
 
        switch (collisionType)
        {
            case PlayerStatusProvider.WallCollisionType.NORMAL:
                // don't play any sound for normal wall collision
                break;
            case PlayerStatusProvider.WallCollisionType.ATTACK:
                SoundManager.Instance.GAMEPLAY_Land();
                break;
            case PlayerStatusProvider.WallCollisionType.SPECIAL_ATTACK:
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

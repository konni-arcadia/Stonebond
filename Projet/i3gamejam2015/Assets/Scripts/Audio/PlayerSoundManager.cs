using UnityEngine;
using System.Collections;

public class PlayerSoundManager : MonoBehaviour
{
    public PlayerStatusProvider statusProvider;

    bool prevIsGroundedFlag = true;
    //bool prevIsGroundedFlag = true;

    void Start()
    {
        //statusProvider.OnDashUpAction??
        statusProvider.OnGroundedStatusChanged += OnGrounded;
        statusProvider.OnOnWallStatusChanged += OnWallrided;
        statusProvider.OnKnockBackAction += onKnockbacked;
        statusProvider.OnDieAction += onPlayerDied;
    }

    private void OnGrounded(bool isGrounded)
    {
        if (isGrounded && !prevIsGroundedFlag)
        {
            SoundManager.Instance.GAMEPLAY_Land();
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

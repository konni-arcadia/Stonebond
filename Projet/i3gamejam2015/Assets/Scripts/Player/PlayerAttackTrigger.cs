using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAttackTrigger : MonoBehaviour
{

    private PlayerStateController playerState;
    private Collider2D attackCollider;

    void Start()
    {
        playerState = GetComponentInParent<PlayerStateController>();
        attackCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        playerState.HandleOnCollide(attackCollider, other);
    }
}

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAttackTrigger : MonoBehaviour
{

    private PlayerStateController playerState;
    private Collider2D collider;
    private List<Collider2D> current = new List<Collider2D>();

    void Start()
    {
        playerState = GetComponentInParent<PlayerStateController>();
        collider = GetComponent<Collider2D>();
    }

    void Update()
    {
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        playerState.OnCollide(collider, other);
    }
}

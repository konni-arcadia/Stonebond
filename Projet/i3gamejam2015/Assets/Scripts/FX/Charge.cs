using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Charge : MonoBehaviour
{
    public List<ParticleSystem> circleParticles;

    private PlayerMovementController movementController;
    private ParticleSystem particles;

    void Awake()
    {
        movementController = transform.GetComponentInParent<PlayerMovementController>();
        int playerId = transform.GetComponentInParent<PlayerStateController>().GetPlayerId();
        particles = circleParticles[playerId - 1];

        PlayerStatusProvider statusProvider = transform.GetComponentInParent<PlayerStatusProvider>();
        statusProvider.OnChargeReadyAction += HandleOnChargeReadyAction;
        statusProvider.OnChargeStopAction += HandleOnChargeStopAction;
    }
 
	public void Update()
    {	
		// if the player is facing left, we need to invert the localScale of the charge particles otherwise they will spawn outside of the circle
        transform.localScale = new Vector3 (movementController.isFacingRight() ? 1.0f : -1.0f, 1.0f, 1.0f);
	}
        
    private void HandleOnChargeReadyAction()
    {
        particles.gameObject.SetActive(true);
    }

    public void HandleOnChargeStopAction(bool complete)
    {
        particles.gameObject.SetActive(false);
    }

}

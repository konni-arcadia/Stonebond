using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Charge : MonoBehaviour {

	// we need a reference to check geometry (facing left or right)
	private GameObject playerGameObject;

	public List<ParticleSystem>circleParticles;

	protected int playerId;

	public void SetPlayer(PlayerStateController playerController)
	{
		this.playerId = playerController.playerId;
		this.playerGameObject = playerController.gameObject;
	}

	ParticleSystem GetCircleParticles()
	{
		return circleParticles[playerId-1];
	}

	public void StartCharge()
	{
		GetCircleParticles().gameObject.SetActive(true);
	}

	public void StopCharge()
	{
		GetCircleParticles().gameObject.SetActive(false);
	}

	public void Update() {
		
		// if the player is facing left, we need to invert the localScale of the charge particles otherwise they will spawn outside of the circle
		gameObject.transform.localScale = new Vector3 ( playerGameObject.transform.localScale.x, 1, 1 );

	}

}

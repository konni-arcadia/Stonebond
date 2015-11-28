using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Shield : MonoBehaviour 
{
	public Animator animator;
	public List<ParticleSystem>circleParticles;
	public List<ParticleSystem> magicalParticles;

	protected int playerId;

	public void SetPlayer(int playerId)
	{
		this.playerId = playerId;
		//circleParticles.
	}

	ParticleSystem GetCircleParticles()
	{
		return circleParticles[playerId];
	}

	ParticleSystem GetMagicalParticles()
	{
		return circleParticles[playerId];
	}

	public void Create()
	{
		GetCircleParticles().gameObject.SetActive(true);
		GetMagicalParticles().gameObject.SetActive(true);
		animator.SetTrigger("Create");
	}

	public void Break()
	{
		GetCircleParticles().gameObject.SetActive(false);
		GetMagicalParticles().gameObject.SetActive(false);
		animator.SetTrigger("Break");
	}
}

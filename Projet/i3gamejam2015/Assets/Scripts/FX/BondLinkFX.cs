using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BondLinkFX : MonoBehaviour 
{
	public List<ParticleSystem> magicalParticles;
	public List<ParticleSystem> lineParticles;
	public Animator contactAnimator;
	
	protected int playerId;
	
	public void SetPlayer(int playerId)
	{
		this.playerId = playerId;
	}

	void Start() {
		foreach (ParticleSystem particles in lineParticles) {
			particles.gameObject.GetComponent<UnpausableParticleSystem > ().acceleration = 10.0f;
		}
	}
	
	public ParticleSystem GetLineParticles()
	{
		return lineParticles[playerId - 1];
	}
	
	public ParticleSystem GetMagicalParticles()
	{
		return magicalParticles[playerId - 1];
	}
	
	public void Create()
	{
		GetLineParticles().gameObject.SetActive(true);
		GetMagicalParticles().gameObject.SetActive(true);
		contactAnimator.SetTrigger("Create");
	}
	
	public void Break()
	{
		GetLineParticles().gameObject.SetActive(false);
		GetMagicalParticles().gameObject.SetActive(false);
		contactAnimator.SetTrigger("Break");
	}
}

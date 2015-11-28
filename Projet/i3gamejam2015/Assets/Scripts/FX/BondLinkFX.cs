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

	
	ParticleSystem GetLineParticles()
	{
		return lineParticles[playerId];
	}
	
	ParticleSystem GetMagicalParticles()
	{
		return magicalParticles[playerId];
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

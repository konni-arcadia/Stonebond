using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Shield : MonoBehaviour 
{
	public Animator animator;
	public List<ParticleSystem> magicalParticles;

	protected int playerId;

	public void SetPlayer(int playerId)
	{
		this.playerId = playerId;
	}

	ParticleSystem GetMagicalParticles()
	{
		return magicalParticles[playerId-1];
	}

	public void Create()
	{
		GetMagicalParticles().gameObject.SetActive(true);
		animator.updateMode = AnimatorUpdateMode.UnscaledTime;
		animator.SetTrigger("Create");
	}

	public void Break()
	{
		GetMagicalParticles().gameObject.SetActive(false);
		animator.updateMode = AnimatorUpdateMode.UnscaledTime;
		animator.SetTrigger("Break");
	}

}

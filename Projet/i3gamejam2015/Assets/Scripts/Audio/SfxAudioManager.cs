﻿using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

[PrefabAttribute("Prefabs/Audio/SFX/SfxAudioManager")]
public class SfxAudioManager : BaseAudioManager {

	//All properties below are set in the prefab..
	public AudioMixerSnapshot SfxBoundSnapshot;
	public AudioMixerSnapshot SfxDefaultSnapshot;

	public AudioMixerSnapshot MainBoundSnapshot;
	public AudioMixerSnapshot MainDefaultSnapshot;
	public AudioMixerSnapshot MainNoSfxSnapshot;

	public AudioClip Cursor;
	public AudioClip Validate;
	public AudioClip Cancel;
	public AudioClip BoundStart;
	public AudioClip BoundStop;
	public AudioClip[] Jumps;
	public AudioClip WallJump;
	public AudioClip AttackA;
	public AudioClip AttackB;
	public AudioClip Knockback;
	public AudioClip[] Lands;
	public AudioClip[] Deaths;
	public AudioClip Rebirth;
	public AudioClip WallSlide;
	public AudioClip ChargeSpecialAttack;
	public AudioSource ChargeSpecialAttackAudioSource;

	public AudioSource WallSlideAudioSource;

	public void SetSfxDefaultSnapshot()
	{
		SfxDefaultSnapshot.TransitionTo(transitionIn);
	}

	public void SetSfxBoundSnapshot()
	{
		SfxBoundSnapshot.TransitionTo(transitionIn);
	}

	public void SetMainDefaultSnapshot()
	{
		MainDefaultSnapshot.TransitionTo(transitionIn);
	}

	public void SetMainBoundSnapshot()
	{
		MainBoundSnapshot.TransitionTo(transitionIn);
	}

	public void PlayStartBound()
	{
		GetAudioSource().PlayOneShot(BoundStart);
		SetSfxBoundSnapshot();
		SetMainBoundSnapshot ();
	}

	public void PlayStopBound()
	{
		GetAudioSource().PlayOneShot(BoundStop);
		SetSfxDefaultSnapshot();
		SetMainDefaultSnapshot();
	}

	public void PlayCursor()
	{
		GetAudioSource().PlayOneShot(Cursor);
	}

	public void PlayValidate()
	{
		GetAudioSource().PlayOneShot(Validate);
	}

	public void PlayCancel()
	{
		GetAudioSource().PlayOneShot(Cancel);
	}

	public void PlayAttack()
	{

		float r = Random.Range(0, 2);
		if (r > 0.5f)
		{
			GetAudioSource().PlayOneShot(AttackA);
		}
		else
		{
			GetAudioSource().PlayOneShot(AttackB);
		}
	}

	public void PlayChargeSpecialAttack()
	{
		ChargeSpecialAttackAudioSource.Play();
	}

	public void StopChargeSpecialAttack()
	{
		ChargeSpecialAttackAudioSource.Stop();
	}

	public void PlayDeath()
	{
		try
		{
			//We can not do Deaths.Count to give a max value so I added a try catch if prefab is null or property size less than 2
			GetAudioSource().PlayOneShot(Deaths[Random.Range(0,2)]);
		}
		catch(System.Exception)
		{
			Debug.Log("Please check property Jumps in SfxAudioManager prefab. We probably my miss values ?");
		}
	}

	public void PlayReBirth()
	{
		GetAudioSource().PlayOneShot(Rebirth);
	}

	public void PlayKnockBack()
	{
		GetAudioSource().PlayOneShot(Rebirth);
	}

	public void PlayJump()
	{
		try
		{
			//We can not do Jumps.Count to give a max value so I added a try catch if prefab is null or property size less than 4
			GetAudioSource().PlayOneShot(Jumps[Random.Range(0,4)]);
		}
		catch(System.Exception)
		{
			Debug.Log("Please check property Jumps in SfxAudioManager prefab. We probably my miss values ?");
		}
	}

	public void PlayLand()
	{
		try
		{
			//We can not do Jumps.Count to give a max value so I added a try catch if prefab is null or property size less than 3
			GetAudioSource().PlayOneShot(Lands[Random.Range(0,3)]);
		}
		catch(System.Exception)
		{
			Debug.Log("Please check property Lands in SfxAudioManager prefab. We probably my miss values ?");
		}
	}

	public void PlayWallJump()
	{
		GetAudioSource().PlayOneShot(WallJump);
	}

	public void PlayWallSlide()
	{
		WallSlideAudioSource.Play();
	}

	public void StopWallSlide()
	{
		WallSlideAudioSource.Stop();
	}

	//This is used to avoid having sfx played during Vitory screeen
	public void SetNoSfxOnMainMixer()
	{
		MainNoSfxSnapshot.TransitionTo(transitionIn);
	}

	public void SetNoSfxOnMainMixerAfterVictory(float time)
	{
		Invoke("SetNoSfxOnMainMixer", time);
	}
}

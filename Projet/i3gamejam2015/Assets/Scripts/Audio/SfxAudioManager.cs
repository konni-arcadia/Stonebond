using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

[PrefabAttribute("Prefabs/Audio/SFX/SfxAudioManager")]
public class SfxAudioManager : BaseAudioManager {

	public AudioMixerSnapshot BoundSnapshot;
	public AudioMixerSnapshot DefaultSnapshot;
	public AudioClip VictoryJingle;
	public AudioClip Cursor;
	public AudioClip Validate;
	public AudioClip Cancel;
	public AudioClip BoundStart;
	public AudioClip BoundStop;
	public AudioClip Jump;
	public AudioClip WallJump;
	public AudioClip AttackA;
	public AudioClip AttackB;
	public AudioClip Knockback;
	public AudioClip Land;
	public AudioClip Death;
	public AudioClip Rebirth;

	private AudioSource _source;

	public void SetDefaultSnapshot()
	{
		DefaultSnapshot.TransitionTo(0f);
	}

	public void SetBoundSnapshot()
	{
		BoundSnapshot.TransitionTo(0f);
	}
		
	public void PlayVictoryJingle()
	{
		GetAudioSource().PlayOneShot(VictoryJingle);
	}

	public void PlayStartBound()
	{
		GetAudioSource().PlayOneShot(BoundStart);
		SetBoundSnapshot();
	}

	public void PlayStopBound()
	{
		GetAudioSource().PlayOneShot(BoundStop);
		SetDefaultSnapshot();
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

	public void PlayDeath()
	{
		GetAudioSource().PlayOneShot(Death);
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
		GetAudioSource().PlayOneShot(Jump);
	}

	public void PlayLand()
	{
		GetAudioSource().PlayOneShot(Land);
	}

	public void PlayWallJump()
	{
		GetAudioSource().PlayOneShot(Land);
	}
}

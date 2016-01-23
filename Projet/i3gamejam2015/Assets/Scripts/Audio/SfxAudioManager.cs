using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System;

[PrefabAttribute("Prefabs/Audio/SFX/SfxAudioManager")]
public class SfxAudioManager : BaseAudioManager {

	//All properties below are set in the prefab. Be carefull as the prefab as multiple levels..

	#region AudioClip
	public AudioMixerSnapshot SfxBoundSnapshot;
	public AudioMixerSnapshot SfxDefaultSnapshot;
	public AudioMixerSnapshot MainBoundSnapshot;
	public AudioMixerSnapshot MainDefaultSnapshot;
	public AudioMixerSnapshot MainNoSfxSnapshot;
	#endregion

	#region AudioClip
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
	#endregion

	#region AudioSource
	public AudioSource ChargeAudioSource;
	public AudioSource ChargeReadyAudioSource;
	public AudioSource WallSlideAudioSource;
	#endregion

	private int InitialRebirthCount;

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

		float r = UnityEngine.Random.Range(0, 2);
		if (r > 0.5f)
		{
			GetAudioSource().PlayOneShot(AttackA);
		}
		else
		{
			GetAudioSource().PlayOneShot(AttackB);
		}
	}

	public void PlayCharge()
	{
		ChargeAudioSource.Play();
	}

	public void StopCharge()
	{
		ChargeAudioSource.Stop();
		StopChargeThatIsReady();
	}

	public void PlayChargeIsReady()
	{
		ChargeReadyAudioSource.Play();
	}

	public void StopChargeThatIsReady()
	{
		ChargeReadyAudioSource.Stop();
	}


	public void PlayDeath()
	{
		try
		{
			//We can not do Deaths.Count to give a max value so I added a try catch if prefab is null or property size less than 2
			GetAudioSource().PlayOneShot(Deaths[UnityEngine.Random.Range(0,2)]);
		}
		catch(System.Exception)
		{
			Debug.Log("Please check property Jumps in SfxAudioManager prefab. We probably my miss values ?");
		}
	}

	public void PlayReBirth(bool initial)
	{
		if (!initial)
		{	
			InitialRebirthCount = 0;
			GetAudioSource().PlayOneShot(Rebirth);
		}
		else
		{
			//There is maybe a better way to be sure this is called in a level :-)
			if (Enum.IsDefined(typeof(Constants.StageEnum), (string)SceneManager.GetActiveScene().name))
			{	
				if (InitialRebirthCount == 0)
				{   
					GetAudioSource().PlayOneShot(Rebirth);
					InitialRebirthCount++;
				}
				else
				{
					InitialRebirthCount++;
					if (InitialRebirthCount == 4)
					{
						InitialRebirthCount = 0;
					}
				}
			}
			else
			{
				InitialRebirthCount = 0;
				GetAudioSource().PlayOneShot(Rebirth);
			}
		}
	}

	public void PlayKnockBack()
	{
		GetAudioSource().PlayOneShot(Knockback);
	}

	public void PlayJump()
	{
		try
		{
			//We can not do Jumps.Count to give a max value so I added a try catch if prefab is null or property size less than 4
			GetAudioSource().PlayOneShot(Jumps[UnityEngine.Random.Range(0,4)]);
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
			GetAudioSource().PlayOneShot(Lands[UnityEngine.Random.Range(0,3)]);
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

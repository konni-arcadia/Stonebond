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
		SetDefaultSnapshot();
	}

	public void PlayValidate()
	{
		GetAudioSource().PlayOneShot(Validate);
	}

	public void PlayCancel()
	{
		GetAudioSource().PlayOneShot(Cancel);
	}
}

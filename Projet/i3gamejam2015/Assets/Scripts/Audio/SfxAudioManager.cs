using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

[PrefabAttribute("Prefabs/Audio/SFX/SfxAudioManager")]
public class SfxAudioManager : BaseAudioManager {

	public AudioMixerSnapshot BoundSnapshot;
	public AudioMixerSnapshot DefaultSnapshot;

	public AudioClip VictoryJingle;


	public void SetDefaultSnapshot()
	{
		DefaultSnapshot.TransitionTo(0f);
	}

	public void SetBoundSnapshot()
	{
		BoundSnapshot.TransitionTo(0f);
	}


	public void VictoryJinglePlay()
	{
		var source = GetAudioSource(this.gameObject.name);
		source.PlayOneShot(VictoryJingle);
	}
}

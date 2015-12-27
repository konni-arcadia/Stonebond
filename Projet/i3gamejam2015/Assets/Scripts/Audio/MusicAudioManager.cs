using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

[PrefabAttribute("Prefabs/Audio/Music/MusicAudioManager")]
public class MusicAudioManager : BaseAudioManager {

	public AudioMixerSnapshot SpireHighSnapshot;
	public AudioMixerSnapshot PipesOfAwakeningSnapshot;
	public AudioMixerSnapshot CloisterOfTheSilenceSnapshot;
	public AudioMixerSnapshot RosetteOfTheWingeSnapshot;
	public AudioMixerSnapshot DefaultSnapshot;

	void Awake() {
		DontDestroyOnLoad(this.gameObject);
	}

	void Start () 
	{
		DefaultSnapshot.TransitionTo(0f);
	}

	public void SetDefaultSnapshot()
	{
		DefaultSnapshot.TransitionTo(0f);
	}

	public void SetPipesOfAwakeningSnapshot()
	{
		PipesOfAwakeningSnapshot.TransitionTo(0f);
	}

	public void SetSpireHighSnapshot()
	{
		SpireHighSnapshot.TransitionTo(0f);
	}

	public void SetCloisterOfTheSilence()
	{
		CloisterOfTheSilenceSnapshot.TransitionTo(0f);
	}

	public void SetRosetteOfTheWingeSnapshot()
	{
		RosetteOfTheWingeSnapshot.TransitionTo(0f);
	}
}

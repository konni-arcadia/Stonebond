using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class MenuAudioManager : BaseAudioManager {

	public AudioMixerSnapshot MainMenuSnapshot;
	public AudioMixerSnapshot SelectSceneSnapshot;
	public AudioMixerSnapshot SelectCharacterSnapshot;
	public AudioMixerSnapshot DefaultSnapshot;

	// Use this for initialization
	void Start () 
	{
		DefaultSnapshot.TransitionTo(5f);
	}
}

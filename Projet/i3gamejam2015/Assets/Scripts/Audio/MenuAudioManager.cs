using UnityEngine;
using System.Collections;
using UnityEngine.Audio;


[PrefabAttribute("/Prefabs/Audio/MenuAudioManager")]
public class MenuAudioManager : BaseAudioManager {

	public AudioMixerSnapshot MainMenuSnapshot;
	public AudioMixerSnapshot SelectSceneSnapshot;
	public AudioMixerSnapshot SelectCharacterSnapshot;
	public AudioMixerSnapshot DefaultSnapshot;

	// Use this for initialization
	void Start () 
	{
		DefaultSnapshot.TransitionTo(transitionIn);
	}


	public void SetMainMenuSnapshot()
	{
		MainMenuSnapshot.TransitionTo(5f);
	}

	public void SetSelectCharacterSnapshot()
	{
		SelectCharacterSnapshot.TransitionTo(5f);
	}

	public void SetSelectSceneSnapshot()
	{
		SelectSceneSnapshot.TransitionTo(5f);
	}

}

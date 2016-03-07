using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

[PrefabAttribute("Prefabs/Audio/Menu/MenuAudioManager")]
public class MenuAudioManager : BaseAudioManager {

	public AudioMixerSnapshot MainMenuSnapshot;
	public AudioMixerSnapshot SelectSceneSnapshot;
	public AudioMixerSnapshot SelectCharacterSnapshot;
	public AudioMixerSnapshot DefaultSnapshot;

	void Start () 
	{
        //Make sure when object is instanciated that the music is default meaning without music
        SetDefaultSnapshot();
	}

	public void SetDefaultSnapshot()
	{
		DefaultSnapshot.TransitionTo(transitionOut);
	}

	public void SetMainMenuSnapshot()
	{
		MainMenuSnapshot.TransitionTo(transitionIn);
	}

	public void SetSelectCharacterSnapshot()
	{
		SelectCharacterSnapshot.TransitionTo(transitionIn);
	}

	public void SetSelectStageSnapshot()
	{
		SelectSceneSnapshot.TransitionTo(transitionIn);
	}
}

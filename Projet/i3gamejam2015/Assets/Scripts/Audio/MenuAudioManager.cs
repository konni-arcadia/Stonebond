using UnityEngine;
using System.Collections;
using UnityEngine.Audio;


[PrefabAttribute("Prefabs/Audio/Menu/MenuAudioManager")]
public class MenuAudioManager : BaseAudioManager {

	public AudioMixerSnapshot MainMenuSnapshot;
	public AudioMixerSnapshot SelectSceneSnapshot;
	public AudioMixerSnapshot SelectCharacterSnapshot;
	public AudioMixerSnapshot DefaultSnapshot;

	void Awake() {
		DontDestroyOnLoad(this.gameObject);
	}

	// Use this for initialization
	void Start () 
	{
		MainMenuSnapshot.TransitionTo(transitionIn);
	}

	public void SetDefaultSnapshot()
	{
		DefaultSnapshot.TransitionTo(0f);
	}

	public void SetMainMenuSnapshot()
	{
		MainMenuSnapshot.TransitionTo(0f);
	}

	public void SetSelectCharacterSnapshot()
	{
		SelectCharacterSnapshot.TransitionTo(0f);
	}

	public void SetSelectSceneSnapshot()
	{
		SelectSceneSnapshot.TransitionTo(0f);
	}

}

using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

[PrefabAttribute("Prefabs/Audio/Music/MusicAudioManager")]
public class MusicAudioManager : BaseAudioManager {

	public AudioMixerSnapshot SpireHighSnapshot;
	public AudioMixerSnapshot PipesOfAwakeningSnapshot;
	public AudioMixerSnapshot CloisterOfTheSilenceSnapshot;
	public AudioMixerSnapshot RosetteOfTheWingeSnapshot;
	public AudioMixerSnapshot MusicDefaultSnapshot;
	public AudioMixerSnapshot PauseSnapshot;
	public AudioMixerSnapshot MainDefaultSnapshot;
	public AudioSource VictoryJingleAudioSource;

	public void Stage_Play(Constants.StageEnum _stage)
	{
		switch (_stage)
		{
		case Constants.StageEnum.LevelOrgan:
			AudioSingleton<MusicAudioManager>.Instance.SetPipesOfAwakeningSnapshot();
			break;
		case Constants.StageEnum.LevelRoof:
			AudioSingleton<MusicAudioManager>.Instance.SetSpireHighSnapshot();
			break;
		case Constants.StageEnum.LevelForest:
			AudioSingleton<MusicAudioManager>.Instance.SetCloisterOfTheSilence();
			break;
		case Constants.StageEnum.LevelCathedrale:
			AudioSingleton<MusicAudioManager>.Instance.SetRosetteOfTheWingeSnapshot();
			break;
		}
	}

    void Start()
    {
        SetMusicDefaultSnapshot();
    }

	public void SetMusicDefaultSnapshot()
	{
		MusicDefaultSnapshot.TransitionTo(transitionIn);
	}

	public void SetMainDefaultSnapshot()
	{
		MainDefaultSnapshot.TransitionTo(transitionIn);
	}

	public void SetPauseSnapshot()
	{
		PauseSnapshot.TransitionTo(transitionIn);
	}

	public void SetPipesOfAwakeningSnapshot()
	{
		PipesOfAwakeningSnapshot.TransitionTo(transitionIn);
	}

	public void SetSpireHighSnapshot()
	{
		SpireHighSnapshot.TransitionTo(transitionIn);
	}

	public void SetCloisterOfTheSilence()
	{
		CloisterOfTheSilenceSnapshot.TransitionTo(transitionIn);
	}

	public void SetRosetteOfTheWingeSnapshot()
	{
		RosetteOfTheWingeSnapshot.TransitionTo(transitionIn);
	}

	public void PlayVictoryJingle()
	{
		VictoryJingleAudioSource.Play();
	}
}

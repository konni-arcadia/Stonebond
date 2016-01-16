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


	public void Stage_Play(Constants.StageEnum _stage)
	{
		switch (_stage)
		{
		case Constants.StageEnum.PipesOfAwakening:
			AudioSingleton<MusicAudioManager>.Instance.SetPipesOfAwakeningSnapshot();
			break;
		case Constants.StageEnum.SpireHigh:
			AudioSingleton<MusicAudioManager>.Instance.SetSpireHighSnapshot();
			break;
		case Constants.StageEnum.CloisterOfTheSilence:
			AudioSingleton<MusicAudioManager>.Instance.SetCloisterOfTheSilence();
			break;
		case Constants.StageEnum.RosetteOfTheWingedOnes:
			AudioSingleton<MusicAudioManager>.Instance.SetRosetteOfTheWingeSnapshot();
			break;
		}
	}


	public void SetMusicDefaultSnapshot()
	{
		MusicDefaultSnapshot.TransitionTo(0f);
	}

	public void SetMainDefaultSnapshot()
	{
		MainDefaultSnapshot.TransitionTo(0f);
	}

	public void SetPauseSnapshot()
	{
		PauseSnapshot.TransitionTo(0f);
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

using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

[PrefabAttribute("Prefabs/Audio/Voice/VoiceAudioManager")]
public class VoiceAudioManager : BaseAudioManager {

	public AudioClip VOICETitle;
	public AudioClip VOICECharacterSelect;
	public AudioClip VOICEGetReady;
	public AudioClip VOICEFight;
	public AudioClip VOICEGameover;
	public AudioClip VOICESuddenDeath;

	public void PlayTitle()
	{
		GetAudioSource().PlayOneShot(VOICETitle);
	}

	public void SelectCharacterPlay()
	{
		GetAudioSource().PlayOneShot(VOICECharacterSelect);
	}

	public void PlayGameReady()
	{
		GetAudioSource().PlayOneShot(VOICEGetReady);
	}

	public void PlayGameOver()
	{
		GetAudioSource().PlayOneShot(VOICEGameover);
	}

	public void PlayFight()
	{
		GetAudioSource().PlayOneShot(VOICEFight);
	}

	public void PlaySuddenDeathVoice()
	{
		GetAudioSource().PlayOneShot(VOICESuddenDeath);
	}

	public void DelayPlayGameOver(float second)
	{
		Invoke("PlayGameOver",second);
	}
}

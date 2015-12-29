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


	public void TitlePlay()
	{
		var source = GetAudioSource(this.gameObject.name);
		source.PlayOneShot(VOICETitle);
	}

	public void SelectCharacterPlay()
	{
		var source = GetAudioSource(this.gameObject.name);
		source.PlayOneShot(VOICECharacterSelect);
	}
}

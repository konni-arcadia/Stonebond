using UnityEngine;

public class AudioPreLoader : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//This will pre load all the Audio Manager to avoid the expensive call later during the game
		AudioSingleton<MusicAudioManager>.Instance.GetType();
		AudioSingleton<MenuAudioManager>.Instance.GetType();
		AudioSingleton<SfxAudioManager>.Instance.GetType();
        AudioSingleton<VoiceAudioManager>.Instance.GetType();
    }
}

using UnityEngine;
using System.Collections;

public class AudioPreLoader : MonoBehaviour {

	// Use this for initialization
	void Start () {
		AudioSingleton<MusicAudioManager>.Instance.GetType();
		AudioSingleton<MenuAudioManager>.Instance.GetType();
		AudioSingleton<SfxAudioManager>.Instance.GetType();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

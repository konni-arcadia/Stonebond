using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class BaseAudioManager : MonoBehaviour {

	public AudioMixerGroup mixerGroup;
	protected float BPM = 110;
	protected float quarterNote;
	protected float transitionIn;
	protected float transitionOut;

	void start()
	{
		quarterNote = 60f / BPM;
		transitionIn = quarterNote / 8f;
		transitionOut = quarterNote * 8f;
	}

	void Awake() {
		DontDestroyOnLoad(this.gameObject);
	}
		
	protected AudioSource GetAudioSource()
	{
		var gameObject = GameObject.Find(this.gameObject.name);
		AudioSource source = null;
		if (gameObject != null)
		{
			source = gameObject.GetComponent<AudioSource>();
		}
		else
		{
			Debug.Log ("Missing an audio source to object: " + this.gameObject.name);
		}	
		return source;
	}
}

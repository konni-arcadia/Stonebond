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

	protected AudioSource GetAudioSource(string _objectID)
	{
		var gameObject = GameObject.Find(_objectID);
		AudioSource source = null;
		if (gameObject != null)
		{
			source = gameObject.GetComponent<AudioSource>();
		}
		return source;
	}
}

using UnityEngine;
using System.Collections;

public class CountDownManager : MonoBehaviour {

	public LevelManager levelMgr;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnGetReadyTextShown()
	{
        // TODO add sound
        //Debug.Log("OnGetReadyTextShown");
		AudioSingleton<VoiceAudioManager>.Instance.PlayGameReady();
	}
	
	public void OnFightTextShown()
	{
		Flash.Show ();
		// TODO add sound
		//Debug.Log("OnFightTextShown");
		AudioSingleton<VoiceAudioManager>.Instance.PlayFight();
    }
	
	public void OnCountDownOver()
	{
        // TODO notify level manager
        //Debug.Log("OnCountDownOver");
		//AudioSingleton<VoiceAudioManager>.Instance.PlayGameOver();
    }
}

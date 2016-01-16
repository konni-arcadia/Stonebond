using UnityEngine;
using System.Collections;

public class CountDownManager : MonoBehaviour {

	public LevelManager levelMgr;

	// Use this for initialization
	void Start () {
		AudioSingleton<MenuAudioManager>.Instance.SetMainMenuSnapshot();
	
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
        //Debug.Log("OnCountDownOver");
		AudioSingleton<MenuAudioManager>.Instance.SetDefaultSnapshot();
		AudioSingleton<MusicAudioManager>.Instance.Stage_Play(levelMgr.GetCurrentStageEnum());
    }
}

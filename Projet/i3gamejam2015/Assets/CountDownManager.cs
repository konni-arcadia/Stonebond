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
		AudioSingleton<VoiceAudioManager>.Instance.PlayGameReady();
		AudioSingleton<SfxAudioManager>.Instance.SetMainDefaultSnapshot();
	}
	
	public void OnFightTextShown()
	{
		Flash.Show ();
		//Debug.Log("OnFightTextShown");
		AudioSingleton<VoiceAudioManager>.Instance.PlayFight();
    }
	
	public void OnCountDownOver()
	{
        //Debug.Log("OnCountDownOver");
		AudioSingleton<MenuAudioManager>.Instance.SetDefaultSnapshot();
		AudioSingleton<MusicAudioManager>.Instance.StagePlay(levelMgr.GetCurrentStageEnum());
    }
}

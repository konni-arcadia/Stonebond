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
		if (SoundManager.Instance != null) {
			SoundManager.Instance.GAMEPLAY_Ready ();
		}
	}
	
	public void OnFightTextShown()
	{
		Flash.flash ();
		// TODO add sound
		//Debug.Log("OnFightTextShown");
		if (SoundManager.Instance != null) {
			SoundManager.Instance.GAMEPLAY_Fight ();
		}
    }
	
	public void OnCountDownOver()
	{
        // TODO notify level manager
        //Debug.Log("OnCountDownOver");
		if (SoundManager.Instance != null) {
			SoundManager.Instance.GAMEPLAY_Gameover ();
		}
    }
}

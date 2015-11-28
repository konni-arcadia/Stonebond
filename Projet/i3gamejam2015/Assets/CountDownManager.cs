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
        SoundManager.Instance.GAMEPLAY_Ready();
	}
	
	public void OnFightTextShown()
	{
		Flash.flash ();
		// TODO add sound
		//Debug.Log("OnFightTextShown");
        SoundManager.Instance.GAMEPLAY_Fight();
    }
	
	public void OnCountDownOver()
	{
        // TODO notify level manager
        //Debug.Log("OnCountDownOver");
        SoundManager.Instance.GAMEPLAY_Gameover();
    }
}

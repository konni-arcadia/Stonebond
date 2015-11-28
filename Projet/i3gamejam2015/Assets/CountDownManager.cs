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
		Debug.Log("OnGetReadyTextShown");
	}
	
	public void OnFightTextShown()
	{
		Flash.flash ();
		// TODO add sound
		Debug.Log("OnFightTextShown");
	}
	
	public void OnCountDownOver()
	{
		// TODO notify level manager
		Debug.Log("OnCountDownOver");
	}
}

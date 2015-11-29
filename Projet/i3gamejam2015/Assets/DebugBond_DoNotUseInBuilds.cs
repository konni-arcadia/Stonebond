using UnityEngine;
using System.Collections;

public class DebugBond_DoNotUseInBuilds : MonoBehaviour {

	public GameObject bondObject;
	protected BondLink bondScript;
	public GameObject playerX;
	public GameObject playerY;

	protected float debugCompletion = 0.0f;

	// Use this for initialization
	void Start () {
		bondScript = bondObject.GetComponent<BondLink> ();
		bondScript.LinkPlayers (playerX, playerY);
	}
	
	// Update is called once per frame
	void Update () {
		debugCompletion = Mathf.Min(1.0f, 0.001f + debugCompletion);
		bondScript.completion = debugCompletion;
	}
}

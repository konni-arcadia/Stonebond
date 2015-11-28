using UnityEngine;
using System.Collections.Generic;

public class Familiz : MonoBehaviour {

	public GameObject char1;
	public GameObject char2;
	public GameObject char3;
	public GameObject char4;

	public Dictionary<float,string[]> script;

	protected int step;

	// Use this for initialization
	void Start () {
//		script.Add( 1.0f, { "1", "action", "param" } );
//		Dictionary<float,string[]>.Enumerator enu = script.GetEnumerator ();
//		enu.MoveNext ();
//		print (enu.Current);
		step = 0;
	}
	
	// Update is called once per frame
	void Update () {

		float t = Time.time;
		float off = -48;

		if ( step < 1 && t > off+58 ) { char4.transform.Translate(0,0,-1); step++; }
		if ( step < 2 && t > off+58.5 ) { char4.transform.Translate(0,0,1); step++; }
		if ( step < 3 && t > off+60 ) { char4.transform.Translate(0,0,-1); step++; }
		if ( step < 4 && t > off+60.3 ) { char4.transform.Translate(0,0,1); step++; }
		if ( step < 5 && t > off+60+19.75 ) { char4.transform.Translate(0,0,-1.5f); step++; }
		if ( step < 6 && t > off+60+20 ) { char4.transform.Translate(0,0,1.5f); step++; }
		if ( step < 7 && t > off+60+21 ) { char2.transform.Translate(0,0,-1); step++; }
		if ( step < 8 && t > off+60+22 ) { char2.transform.Translate(0,0,-0.5f); step++; }
		if ( step < 9 && t > off+60+30 ) { char1.transform.Translate(0,0,-1.6f); step++; }
		if ( step < 10 && t > off+60+30.25 ) { char1.transform.Translate(0.5f,0,0); step++; }
		if ( step < 11 && t > off+60+30.5 ) { char1.transform.Translate(0.5f,0,0); step++; }
		if ( step < 12 && t > off+60+30.75 ) { char1.transform.Translate(0.5f,0,0); step++; }
		if ( step < 13 && t > off+60+31 ) { char1.transform.Translate(0.5f,0,0); step++; }
		if ( step < 14 && t > off+60+31.25 ) { char1.transform.Translate(0.5f,0,0); step++; }
		if ( step < 15 && t > off+60+31.5 ) { char1.transform.Translate(0.5f,0,0); step++; }
		if ( step < 16 && t > off+60+32 ) { char1.transform.Translate(0,0,-0.5f); step++; }
		if ( step < 17 && t > off+60+40 ) { char4.transform.Translate(0,0,-1); step++; }
		if ( step < 18 && t > off+60+41 ) { char1.transform.Translate(0,0,-1f); step++; }
		if ( step < 19 && t > off+60+41.5 ) { char2.transform.Translate(-0.4f,0,-1f); step++; }
		if ( step < 20 && t > off+60+41.5 ) { char4.transform.Translate(0,0,-1f); step++; }
//		if ( step < 20 && t > off+60+41.5 ) { char4.transform.Translate(0,0,-0.5f); step++; }
//		if ( step < 21 && t > off+60+41.5 ) { char4.transform.Translate(0,0,-0.5f); step++; }
		//		if ( step < 17 && t > off+60+40 ) { char4.transform.Translate(0,0,-0.5f); step++; }
//		if ( step < 17 && t > off+60+40 ) { char4.transform.Translate(0,0,-0.5f); step++; }
//		if ( step < 17 && t > off+60+40 ) { char4.transform.Translate(0,0,-0.5f); step++; }


	}
}

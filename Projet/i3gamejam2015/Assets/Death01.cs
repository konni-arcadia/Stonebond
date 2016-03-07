using UnityEngine;
using System.Collections;
using System;

public class Death01 : MonoBehaviour {

	public Vector2 xForceMinMax;
	public Vector2 yForceMinMax;
	protected float fxRange;
	protected float fyRange;

	protected System.Random rand;

	Rigidbody2D[] rigidBodies;

	float timerTest;
	bool explose = false;
	bool desactive = false;
	bool bouge = false;

	// Use this for initialization
	void Start () {
		rigidBodies = transform.gameObject.GetComponentsInChildren<Rigidbody2D> () as Rigidbody2D[];
//		rigidBodies [0].gravityScale = 0.5f;

		rand = new System.Random ();

//		float fxRange = xForceMinMax.y - xForceMinMax.x;
//		float fyRange = yForceMinMax.y - yForceMinMax.x;
//		print (fxRange);

		for (int i = 0; i < rigidBodies.Length; i++) {
			rigidBodies [i].isKinematic = true;
		}

		timerTest = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		timerTest += Time.deltaTime;
		
		if (!explose && timerTest>2.0f) {
			print ("BOUM");
			explose = true;
			for (int i = 0; i < rigidBodies.Length; i++) {
				rigidBodies [i].isKinematic = false;
				float fx = xForceMinMax.x + (((float)rand.Next( 1, 100)) * fxRange / 100.0f);
				float fy = yForceMinMax.x + (((float)rand.Next( 1, 100)) * fyRange / 100.0f);
				Vector2 force = new Vector2( fx, fy );
				print ( force );
				rigidBodies [i].AddForce( force, ForceMode2D.Impulse );
			}
		}
		
		if (explose && !desactive && timerTest>4.0f) {
			desactive = true;
			for (int i = 0; i < rigidBodies.Length; i++) {
				rigidBodies [i].isKinematic = true;
			}
		}

		if (explose && desactive && !bouge && timerTest>6.0f) {
			bouge = true;
			for (int i = 0; i < rigidBodies.Length; i++) {
				rigidBodies [i].transform.localPosition = Vector3.zero;
				rigidBodies [i].transform.localRotation = Quaternion.identity;
			}
		}

	}
}

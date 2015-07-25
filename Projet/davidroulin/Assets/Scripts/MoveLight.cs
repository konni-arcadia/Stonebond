using UnityEngine;
using System.Collections;

public class MoveLight : MonoBehaviour {

	float minX = -4.0f;
	float maxX = 5.0f;
	Vector3 iterate = new Vector3( -10.0f, 0, 0 );

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate ( iterate * Time.deltaTime );
		if (transform.position.x > maxX || transform.position.x < minX) {
			iterate = - iterate;
		}
	}
}

using UnityEngine;
using System.Collections;

public class RotateLight : MonoBehaviour {

	public float radius;
	public float angularSpeed;

	// Use this for initialization
	void Start () {
		transform.position = new Vector3( transform.position.x, transform.position.y, - radius);
	}
	
	// Update is called once per frame
	void Update () {

		transform.RotateAround( Vector3.zero, Vector3.up, angularSpeed * Time.deltaTime );

	}
}

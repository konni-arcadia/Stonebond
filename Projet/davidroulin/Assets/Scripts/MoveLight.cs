using UnityEngine;
using System.Collections;

public class MoveLight : MonoBehaviour {

	public float minX = -12.0f; // -2.0f; 
	public float maxX = 5.0f; // 2.0f;
	public Vector3 iterationVectorX = new Vector3( 3.0f, 0, 0 );
	Vector3 iterateX;

	float minZ = -10.0f;
	float maxZ = -1.0f;
	Vector3 iterationVectorZ = new Vector3( 0, 0, 10.0f );
	Vector3 iterateZ;

	// Use this for initialization
	void Start () {
		iterateX = iterationVectorX;
		iterateZ = iterationVectorZ;
	}
	
	// Update is called once per frame
	void Update () {

		transform.Translate ( iterateX * Time.deltaTime );
//		transform.Translate ( ( iterateX + iterateZ ) * Time.deltaTime );

		if (transform.position.x > maxX) {
			iterateX = -1.0f * iterationVectorX;
		} else if (transform.position.x < minX) {
			iterateX = iterationVectorX;
		}

//		if (transform.position.z > maxZ) {
//			iterateZ = -1.0f * iterationVectorZ;
//		} else if (transform.position.z < minZ) {
//			iterateZ = iterationVectorZ;
//		}
//
//		print (transform.position.z);

	}
}

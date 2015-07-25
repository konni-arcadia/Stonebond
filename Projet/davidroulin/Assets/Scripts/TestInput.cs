using UnityEngine;
using System.Collections;

public class TestInput : MonoBehaviour {

	public float DebugDirectTranslateSpeed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKey(KeyCode.LeftArrow)) {
			transform.Translate(Vector3.left * Time.deltaTime * DebugDirectTranslateSpeed);
		}
		if (Input.GetKey(KeyCode.RightArrow)) {
			transform.Translate(Vector3.right * Time.deltaTime * DebugDirectTranslateSpeed);
		}
		if (Input.GetKey(KeyCode.UpArrow)) {
			transform.Translate(Vector3.up * Time.deltaTime * DebugDirectTranslateSpeed);
		}
		if (Input.GetKey(KeyCode.DownArrow)) {
			transform.Translate(Vector3.down * Time.deltaTime * DebugDirectTranslateSpeed);
		}

	}
}

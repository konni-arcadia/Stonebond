using UnityEngine;
using System.Collections;

public class FogMoveAndTiled : MonoBehaviour {


    public float fogMoveSpeed = 0.01f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<Transform>().position += new Vector3(fogMoveSpeed, 0.0f, 0.0f);

        if (GetComponent<Transform>().position.x > 40.93333333333f)
        {
            GetComponent<Transform>().position = new Vector3(-40.93333333333f, -3.84f, 0.0f);
        }
	}
}

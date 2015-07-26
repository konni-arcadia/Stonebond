using UnityEngine;
using System.Collections;

public class FogMoveAndTiled : MonoBehaviour {


    public float fogMoveSpeed = 0.01f;

    public float rightOffset = 40.93333333333f;

    public float yPos = -3.84f;

    public float xPos = -40.93333333333f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<Transform>().position += new Vector3(fogMoveSpeed, 0.0f, 0.0f);

        if (GetComponent<Transform>().position.x > rightOffset)
        {
            GetComponent<Transform>().position = new Vector3(-rightOffset, yPos, 0.0f);
        }
	}
}

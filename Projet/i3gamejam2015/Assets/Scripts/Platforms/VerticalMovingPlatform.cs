using UnityEngine;
using System.Collections;

public class VerticalMovingPlatform : MonoBehaviour {

    public Transform leftBound;
    public Transform rightBound;
    public Transform platform;

    Transform[] bounds = new Transform[2];

    int currentBoundIndex = 0;

    public float movementSpeed = 1f;

    SurfaceEffector2D surfaceEffector;
    float surfaceEffectorSpeed;

    public bool startToLeft = true;

	// Use this for initialization
	void Start () {
        bounds[0] = leftBound;
        bounds[1] = rightBound;
        surfaceEffector = GetComponentInChildren<SurfaceEffector2D>();
        currentBoundIndex = startToLeft ? 0 : 1;
        surfaceEffectorSpeed = movementSpeed * 5.5f;
        surfaceEffector.speed = (startToLeft ? -1 : 1) * surfaceEffectorSpeed;
        if (surfaceEffector == null)
        { 
            Debug.LogError("Surface effector can't be found in moving platform " + name);
        }
	}
	
	// Update is called once per frame
	void Update () {
	    if(platform.position.y < bounds[0].position.y)
        {
            currentBoundIndex = 1;
            surfaceEffector.speed = surfaceEffectorSpeed;
        }
        if(platform.position.y > bounds[1].position.y)
        {
            currentBoundIndex = 0;
            surfaceEffector.speed = -surfaceEffectorSpeed;
        }

        platform.position += new Vector3(0f, (currentBoundIndex == 0 ? -1 : 1) * movementSpeed * Time.deltaTime, 0f);
	}
}

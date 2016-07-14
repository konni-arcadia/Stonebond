using UnityEngine;
using System.Collections;

public class HorizontalMovingPlatform : MonoBehaviour {

    public Transform leftBound;
    public Transform rightBound;
    public Transform platform;

    private Transform[] bounds = new Transform[2];

    private int currentBoundIndex = 0;

    public float movementSpeed = 1f;

    private Rigidbody2D platformRigidbody;

   // private SurfaceEffector2D surfaceEffector;
   // private float surfaceEffectorSpeed;

    public bool startToLeft = true;

	// Use this for initialization
	void Start () {
        bounds[0] = leftBound;
        bounds[1] = rightBound;
        //surfaceEffector = GetComponentInChildren<SurfaceEffector2D>();
        currentBoundIndex = startToLeft ? 0 : 1;
        //surfaceEffectorSpeed = movementSpeed * 5.5f;        
        //if (surfaceEffector == null)
        //{ 
        //    Debug.LogError("Surface effector can't be found in moving platform " + name);
        //}
        //surfaceEffector.speed = (startToLeft ? -1 : 1) * surfaceEffectorSpeed;

        platformRigidbody = platform.GetComponent<Rigidbody2D>();
        if(platformRigidbody == null)
        {
            Debug.LogError("Rigidbody2D can't be found in moving platform " + name);
        }
    }
	
	// Update is called once per frame
	void Update () {
        Vector2 position = platformRigidbody.position;

        if(position.x < bounds[0].position.x)
        {
            currentBoundIndex = 1;
           // surfaceEffector.speed = surfaceEffectorSpeed;
        }
        if(position.x > bounds[1].position.x)
        {
            currentBoundIndex = 0;
            //surfaceEffector.speed = -surfaceEffectorSpeed;
        }

        position.x += (currentBoundIndex == 0 ? -1 : 1) * movementSpeed * Time.deltaTime;
        platformRigidbody.MovePosition(position);
	}
}

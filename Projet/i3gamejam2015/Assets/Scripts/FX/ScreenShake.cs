using UnityEngine;
using System.Collections;

public class ScreenShake : MonoBehaviour {

	private static ScreenShake instance = null;

	public float scaler = 1.0f;

	//
	// SCREEN SHAKE
	//

	private float strenghtX = 0.0f;
	private float strenghtY = 0.0f;
	private float strenghtZ = 0.0f;

	private float decayX = 0.0f;
	private float decayY = 0.0f;
	private float decayZ = 0.0f;

	private float originX;
	private float originY;
	private float originZ;

	private bool init = true;

	void Awake () {
		instance = this;
	}

	void Start () {
	}

	void Update () {
		if (init) {
			originX = transform.localPosition.x;
			originY = transform.localPosition.y;
			originZ = transform.localPosition.z;
			
			print ("x=" + originX + " y=" + originY + " z=" + originZ);
			init = false;
		}

		Vector3 position = transform.localPosition;

		if (decayX > 0.0f && strenghtX > 0.0f) {
			strenghtX -= decayX * Time.deltaTime;
			if(strenghtX < 0.0f) {
				strenghtX = 0.0f;
			}
		}

		if (decayY > 0.0f && strenghtY > 0.0f) {
			strenghtY -= decayY * Time.deltaTime;
			if(strenghtY < 0.0f) {
				strenghtY = 0.0f;
			}
		}

		if (decayZ > 0.0f && strenghtZ > 0.0f) {
			strenghtZ -= decayZ * Time.deltaTime;
			if(strenghtZ < 0.0f) {
				strenghtZ = 0.0f;
			}
		}


		if (strenghtX == 0.0f && strenghtY == 0.0f && strenghtZ == 0.0f) {
			position.x = originX;
			position.y = originY;
			position.z = originZ;
		} else {
			if (strenghtX > 0.0f) {
				position.x = originX + Random.Range (-strenghtX, strenghtX);
			}

			if (strenghtY > 0.0f) {
				position.y = originY + Random.Range (-strenghtY, strenghtY);
			}

			if (strenghtZ > 0.0f) {
				position.z = originZ + Random.Range (-strenghtZ, strenghtZ);
			}
		}

		transform.localPosition = position;
	}

	public static void shake(float strenghtX, float strenghtY, float decayX, float decayY) {
		if (instance == null) {
			return;
		}

		instance.strenghtX = strenghtX * instance.scaler;
		instance.strenghtY = strenghtY * instance.scaler;
		instance.decayX = decayX * instance.scaler;
		instance.decayY = decayY * instance.scaler;
	}

	public static void shakeZ(float strenghtZ, float decayZ) {
		if (instance == null) {
			return;
		}
		
		instance.strenghtZ = strenghtZ * instance.scaler;
		instance.decayZ = decayZ * instance.scaler;
	}
}

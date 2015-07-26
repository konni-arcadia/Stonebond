using UnityEngine;
using System.Collections;

public class MyLittlePoney : MonoBehaviour {

	private static MyLittlePoney instance = null;

	private float strenghtX = 0.0f;
	private float strenghtY = 0.0f;

	private float decayX = 0.0f;
	private float decayY = 0.0f;

	void Awake () {
		instance = this;
	}

	void Update () {
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

		if (strenghtX == 0.0f && strenghtY == 0.0f) {
			position.x = 0.0f;
			position.y = 0.0f;
		} else {
			if (strenghtX > 0.0f) {
				position.x = Random.Range (-strenghtX, strenghtX);
			}

			if (strenghtY > 0.0f) {
				position.y = Random.Range (-strenghtY, strenghtY);
			}
		}

		transform.localPosition = position;
	}

	public static void shake(float strenghtX, float strenghtY, float decayX, float decayY) {
		if (instance == null) {
			return;
		}

		instance.strenghtX = strenghtX;
		instance.strenghtY = strenghtY;
		instance.decayX = decayX;
		instance.decayY = decayY;
	}
}

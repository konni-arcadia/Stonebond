using UnityEngine;
using System.Collections;

public class ScreenShake : MonoBehaviour {

	public static ScreenShake instance;

	private float strenghtX = 0.0f;
	private float strenghtY = 0.0f;

	private float decayX = 0.0f;
	private float decayY = 0.0f;

	// Use this for initialization
	void Start () {
		instance = this;
	}
	
	// Update is called once per frame
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

	public void shake(float strenghtX, float strenghtY, float decayX, float decayY) {
		this.strenghtX = strenghtX;
		this.strenghtY = strenghtY;
		this.decayX = decayX;
		this.decayY = decayY;
	}
}

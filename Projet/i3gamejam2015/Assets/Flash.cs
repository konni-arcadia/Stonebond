using UnityEngine;
using System.Collections;

public class Flash : MonoBehaviour {

	private static Flash instance;

	private SpriteRenderer renderer;

	public float flashTime = 0.5f;
	public AnimationCurve curve;

	private float counter = 0.0f;

	// Use this for initialization
	void Awake () {
		renderer = GetComponent<SpriteRenderer> ();
		renderer.enabled = false;

		instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		if (counter > 0.0f) {
			counter -= Time.deltaTime;
			if (counter <= 0.0f) {
				renderer.enabled = false;
				counter = 0.0f;
				return;
			}
		}

		float pct = 1.0f - counter / flashTime;
		float alpha = curve.Evaluate (pct);
		renderer.color = new Color (1.0f, 1.0f, 1.0f, alpha);
	}

	public static void flash() {
		if (instance == null) {
			return;
		}

		instance.counter = instance.flashTime;
		instance.renderer.enabled = true;
		instance.renderer.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
	}
}

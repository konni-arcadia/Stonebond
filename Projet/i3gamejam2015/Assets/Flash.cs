using UnityEngine;
using System.Collections;

public class Flash : MonoBehaviour {

	private static Flash instance;

	private SpriteRenderer spriteRenderer;

	public float flashTime = 0.5f;
	public AnimationCurve curve;

	private float counter = 0.0f;

	private float r, g, b;

	// Use this for initialization
	void Awake () {
		spriteRenderer = GetComponent<SpriteRenderer> ();
        spriteRenderer.enabled = false;

		instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		if (counter > 0.0f) {
			counter -= Time.deltaTime;
			if (counter <= 0.0f) {
                spriteRenderer.enabled = false;
				counter = 0.0f;
				return;
			}
		}

		float pct = 1.0f - counter / flashTime;
		float alpha = curve.Evaluate (pct);
        spriteRenderer.color = new Color (instance.r, instance.g, instance.b, alpha);
	}

	public static void flash() {
		flash (1.0f, 1.0f, 1.0f);
	}

	public static void flash(float r, float g, float b) {
		if (instance == null) {
			return;
		}
		
		instance.counter = instance.flashTime;
		instance.r = r;
		instance.g = g;
		instance.b = b;
        instance.spriteRenderer.enabled = true;
        instance.spriteRenderer.color = new Color (instance.r, instance.g, instance.b, 1.0f);
	}
}

using UnityEngine;
using System.Collections;

public class BackLightAlpha : MonoBehaviour {

	public GameObject backLight;
	public float BackLightRadius;

	public float FullAt;

	public float characterXOffset;
	public float characterYOffset;

	public bool Invert;

	SpriteRenderer renderer;

	// Use this for initialization
	void Start () {
		//backLight = GameObject.Find("BackLight");
		renderer = GetComponent<SpriteRenderer> ();

		//renderer.

	}
	
	// Update is called once per frame
	void Update () {
		Vector3 distance = transform.position + new Vector3( characterXOffset, characterYOffset, 0 ) - backLight.transform.position;
		distance.z = 0;
		float alpha = Mathf.Max ( ( BackLightRadius - distance.magnitude ) / BackLightRadius, 0.0f );
		if (alpha >= FullAt) {
			alpha = 1.0f;
		}
		if (Invert) {
			alpha = 1.0f - alpha;
		}
		SpriteRenderer renderer = GetComponent<SpriteRenderer> ();
		Color originalColor = renderer.color;
		renderer.color = new Color ( originalColor.r, originalColor.g, originalColor.b, alpha );

		//renderer.

		print (originalColor.a);
	}
}

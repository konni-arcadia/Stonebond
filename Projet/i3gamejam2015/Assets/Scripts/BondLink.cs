using UnityEngine;
using System.Collections;

public class BondLink : MonoBehaviour {

	public GameObject emitterA;
	public GameObject emitterB;

	public GameObject emitterAsource;
	public GameObject emitterBsource;
	
	public GameObject emitterAlinkMiddle;
	public GameObject emitterBlinkMiddle;

	public GameObject magicalParticlesAcontainer;
	public GameObject magicalParticlesBcontainer;
	
	//protected GameObject linkMiddle;
	protected Renderer rend;
	protected float originalZ;

    float originalWidth;

	// Use this for initialization
	void Start () {

		//linkMiddle = transform.Find ("testAnimLink01").gameObject;
		//rend = linkMiddle.GetComponent<Renderer> ();
		//originalZ = linkMiddle.transform.position.z;

//        originalWidth = rend.bounds.size.x;

//        if (originalWidth == 0)
//            Debug.LogError("Link sprite has a width of 0 !!!");
		
	}

	public void OnBond() {
//		ParticleSystem particles
	}
	
	Vector3 castLine() {
		Vector3 a = emitterA.transform.position;
		Vector3 b = emitterB.transform.position;
		a.z = originalZ;
		b.z = originalZ;
		return b - a;
	}
	
	void redrawLink() {

		// first we cast the line between the two gargoyles (the simple math is deferred to a sub-function, for readability)
		Vector3 linkLine = castLine ();
		Vector3 linkLineMiddlePoint = emitterA.transform.position + linkLine * 0.5f;
		Vector3 linkLineQuarterLengthPoint = emitterA.transform.position + linkLine * 0.25f;

//		float angle = Mathf.Atan2(linkLine.y, linkLine.x) * Mathf.Rad2Deg;
//		linkMiddle.transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
//
		emitterAsource.transform.position = emitterA.transform.position;
		emitterAlinkMiddle.transform.position = linkLineMiddlePoint;


		float angle = Mathf.Atan2(linkLine.y, linkLine.x) * Mathf.Rad2Deg;
		angle -= 90;
		emitterAsource.transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
		emitterAlinkMiddle.transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);


		magicalParticlesAcontainer .transform.position = linkLineQuarterLengthPoint;

//		emitterBlinkMiddle.transform.position = linkLineMiddlePoint;


		//emitterAsource.transform.LookAt (emitterAlinkMiddle.transform, - Vector3.forward);
//		emitterB.tran	sform.LookAt (emitterBlinkMiddle);
		
//		emitterAlinkMiddle.transform.LookAt (emitterA.transform);
//		emitterBlinkMiddle.transform.LookAt (emitterB);

//
//		float linkWidth = linkLine.magnitude;
//		float currentWidth = rend.bounds.size.x;
//		Vector3 scale = linkMiddle.transform.localScale;
//        scale.x = linkWidth / originalWidth;
//
//		linkMiddle.transform.localScale = scale;

	}
	
	// Update is called once per frame
	void Update () {
		redrawLink ();
	}
}

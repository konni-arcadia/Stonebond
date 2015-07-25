using UnityEngine;
using System.Collections;

using UnityEditor;

public class BondLink : MonoBehaviour {

	public GameObject emitterA;
	public GameObject emitterB;

//	protected GameObject linkA;
//	protected GameObject linkB;
	protected GameObject linkMiddle;
	protected Renderer rend;
	protected float originalZ;

	// Use this for initialization
	void Start () {

//		linkA = GameObject.Find ("link/particleChestBurn");
//		linkB = GameObject.Instantiate (linkA);

//		linkMiddle = GameObject.Find ("link/particleLinkMiddle");
		linkMiddle = GameObject.Find ("link/testAnimLink01");
		rend = linkMiddle.GetComponent<Renderer> ();
		originalZ = linkMiddle.transform.position.z;

////		linkA.transform.parent = origin.transform;
//		linkA.transform.parent = emitterA.transform;
//		linkA.transform.localPosition = Vector3.zero + new Vector3(0,1,0);
////		linkB.transform.parent = target.transform;
//		linkB.transform.parent = emitterB.transform;
//		linkB.transform.localPosition = Vector3.zero + new Vector3(0,1,0);
		
	}
	
	Vector3 castLine() {
		Vector3 a = emitterA.transform.position;
		Vector3 b = emitterB.transform.position;
		a.z = originalZ;
		b.z = originalZ;
		return b - a;
	}
	
	void redrawLink() {

		Vector3 linkLine = castLine ();

		linkMiddle.transform.position = emitterA.transform.position + linkLine * 0.5f;

//		link.transform.rotation.SetLookRotation (linkLine);
//		link.transform.LookAt (origin.transform.position);
		float angle = Mathf.Atan2(linkLine.y, linkLine.x) * Mathf.Rad2Deg;
		linkMiddle.transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);

//		emitterA.transform.rotation = Quaternion.AngleAxis ( angle - 90, Vector3.forward);
//		emitterB.transform.rotation = Quaternion.AngleAxis ( angle + 90, Vector3.forward);

		float linkWidth = linkLine.magnitude;
//		print (linkWidth);
		float currentWidth = rend.bounds.size.x;
		Vector3 scale = linkMiddle.transform.localScale;
		scale.x = linkWidth * scale.x / currentWidth;

		linkMiddle.transform.localScale = scale;

	}
	
	// Update is called once per frame
	void Update () {
		redrawLink ();
	}
}

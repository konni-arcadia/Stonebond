using UnityEngine;
using System.Collections;

using UnityEditor;

public class BondLink : MonoBehaviour {

//	protected GameObject link;
	public GameObject emitterA;
	public GameObject emitterB;

	protected GameObject linkA;
	protected GameObject linkB;
	protected GameObject linkMiddle;

	protected SerializedObject so_linkA;
	protected SerializedObject so_linkB;

//	protected GameObject origin;
//	protected GameObject target;
	
	// Use this for initialization
	void Start () {
		
//		link = GameObject.Find ("link");

//		emitterA = GameObject.Find ("player/sprite/linkEmit");
//		emitterB = GameObject.Find ("statue/linkEmit");

		linkA = GameObject.Find ("link/particleChestBurn");
		linkB = GameObject.Instantiate (linkA);
		linkMiddle = GameObject.Find ("link/particleLinkMiddle");

//		origin = GameObject.Find ("player/sprite");
//		target = GameObject.Find ("statue");

		so_linkA = new SerializedObject(linkA.GetComponent<ParticleSystem>());
		so_linkB = new SerializedObject(linkB.GetComponent<ParticleSystem>());

//		linkA.transform.parent = origin.transform;
		linkA.transform.parent = emitterA.transform;
		linkA.transform.localPosition = Vector3.zero + new Vector3(0,1,0);
//		linkB.transform.parent = target.transform;
		linkB.transform.parent = emitterB.transform;
		linkB.transform.localPosition = Vector3.zero + new Vector3(0,1,0);
		
		SerializedObject so = new SerializedObject(linkA.GetComponent<ParticleSystem>());
		//so.FindProperty("ShapeModule.radius").floatValue = 0.2f;
		//so.FindProperty("InitialModule.startSize.scalar").floatValue = 0.6f;
//		so.ApplyModifiedProperties();
		
		SerializedProperty it = so.GetIterator();
		while (it.Next(true)) {
			Debug.Log (it.propertyPath);
			//			Debug.Log (it.stringValue);
		}
	}
	
	Vector3 castLine() {
//		return target.transform.position - origin.transform.position;
		return emitterB.transform.position - emitterA.transform.position;
	}
	
	void redrawLink() {
		Vector3 linkLine = castLine ();
		//print (linkLine.magnitude);
//		link.
//			transform.position = origin.transform.position + linkLine * 0.5f;
			transform.position = emitterA.transform.position + linkLine * 0.5f;
//		link.transform.rotation.SetLookRotation (linkLine);
//		link.transform.LookAt (origin.transform.position);
		float angle = Mathf.Atan2(linkLine.y, linkLine.x) * Mathf.Rad2Deg;
//		link.
			transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);

		emitterA.transform.rotation = Quaternion.AngleAxis ( angle - 90, Vector3.forward);

		emitterB.transform.rotation = Quaternion.AngleAxis ( angle + 90, Vector3.forward);

//		print ( link.transform.rotation );
		//		so_linkA = new SerializedObject(linkA.GetComponent<ParticleSystem>());
//		so_linkA.FindProperty("ShapeModule.boxX").floatValue = 30.0f;
//		so_linkA.ApplyModifiedProperties();
	}
	
	// Update is called once per frame
	void Update () {
		
		//print (transform.parent.position.x);
		
		redrawLink ();
		
	}
}

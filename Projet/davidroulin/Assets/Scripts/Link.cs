using UnityEngine;
using System.Collections;

using UnityEditor;

public class Link : MonoBehaviour {

	public GameObject link;
	public GameObject linkAemit;
	public GameObject linkBemit;

	public GameObject linkA;
	public GameObject linkB;
	public GameObject linkMiddle;

	SerializedObject so_linkA;
	SerializedObject so_linkB;

	public GameObject origin;
	public GameObject target;
	
	// Use this for initialization
	void Start () {
		
		link = GameObject.Find ("link");
		linkAemit = GameObject.Find ("player/sprite/linkEmit");
		linkBemit = GameObject.Find ("statue/linkEmit");

		linkA = GameObject.Find ("link/particleChestBurn");
		linkB = GameObject.Instantiate (linkA);
		linkMiddle = GameObject.Find ("link/particleLinkMiddle");

		origin = GameObject.Find ("player/sprite");
		target = GameObject.Find ("statue");

		so_linkA = new SerializedObject(linkA.GetComponent<ParticleSystem>());
		so_linkB = new SerializedObject(linkB.GetComponent<ParticleSystem>());

//		linkA.transform.parent = origin.transform;
		linkA.transform.parent = linkAemit.transform;
		linkA.transform.localPosition = Vector3.zero + new Vector3(0,1,0);
//		linkB.transform.parent = target.transform;
		linkB.transform.parent = linkBemit.transform;
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
		return target.transform.position - origin.transform.position;
	}
	
	void redrawLink() {
		Vector3 linkLine = castLine ();
		//print (linkLine.magnitude);
		link.transform.position = origin.transform.position + linkLine * 0.5f;
//		link.transform.rotation.SetLookRotation (linkLine);
//		link.transform.LookAt (origin.transform.position);
		float angle = Mathf.Atan2(linkLine.y, linkLine.x) * Mathf.Rad2Deg;
		link.transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);

		linkAemit.transform.rotation = Quaternion.AngleAxis ( angle - 90, Vector3.forward);

		linkBemit.transform.rotation = Quaternion.AngleAxis ( angle + 90, Vector3.forward);

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

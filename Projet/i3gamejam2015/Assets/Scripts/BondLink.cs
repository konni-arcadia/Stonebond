using UnityEngine;
using System.Collections;

public class BondLink : MonoBehaviour {

	public GameObject emitterA;
	public GameObject emitterB;

	public GameObject emitterAsource;
	public GameObject emitterBsource;
	
	public GameObject emitterAnoReturnPoint;
	public GameObject emitterBnoReturnPoint;
	
	protected GameObject linkMiddle;
	protected Renderer rend;
	protected float originalZ;

    float originalWidth;

	// Use this for initialization
	void Start () {

		linkMiddle = transform.Find ("testAnimLink01").gameObject;
		rend = linkMiddle.GetComponent<Renderer> ();
		originalZ = linkMiddle.transform.position.z;
        originalWidth = rend.bounds.size.x;
        if (originalWidth == 0)
            Debug.LogError("Link sprite has a width of 0 !!!");
		
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

		float angle = Mathf.Atan2(linkLine.y, linkLine.x) * Mathf.Rad2Deg;
		linkMiddle.transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);

		float linkWidth = linkLine.magnitude;
		float currentWidth = rend.bounds.size.x;
		Vector3 scale = linkMiddle.transform.localScale;
        scale.x = linkWidth / originalWidth;

		linkMiddle.transform.localScale = scale;

	}
	
	// Update is called once per frame
	void Update () {
		redrawLink ();
	}
}

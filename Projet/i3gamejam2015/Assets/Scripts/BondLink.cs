using UnityEngine;
using System.Collections;

public class BondLink : MonoBehaviour {

	public GameObject playerA;
	public GameObject playerB;

	public PlayerStateController playerAStateController;
	public PlayerStateController playerBStateController;
	
	protected BondLinkFX fxA;
	protected BondLinkFX fxB;

	public GameObject emitterAsource;
	public GameObject emitterBsource;
	
	public GameObject emitterAlinkMiddle;
	public GameObject emitterBlinkMiddle;

	protected GameObject magicalParticlesAcontainer;
	protected GameObject magicalParticlesBcontainer;
	
	protected ParticleSystem magicalParticlesAsystem;
	protected ParticleSystem magicalParticlesBsystem;

	public GameObject forwardLineColliderA;
	public GameObject forwardLineColliderB;

//	public GameObject backwardParticlesAcontainer;
//	public GameObject backwardParticlesBcontainer;
	
	public GameObject backwardParticlesAcollider;
	public GameObject backwardParticlesBcollider;
	
	//protected GameObject linkMiddle;
	protected Renderer rend;
	protected float originalZ;

    float originalWidth;

	// Use this for initialization
	void Start () {

		fxA = emitterAsource.GetComponent<BondLinkFX> ();
		fxB = emitterBsource.GetComponent<BondLinkFX> ();

		playerAStateController = playerA.GetComponent<PlayerStateController> ();
		playerBStateController = playerB.GetComponent<PlayerStateController> ();

		//linkMiddle = transform.Find ("testAnimLink01").gameObject;
		//rend = linkMiddle.GetComponent<Renderer> ();
		//originalZ = linkMiddle.transform.position.z;

//        originalWidth = rend.bounds.size.x;

//        if (originalWidth == 0)
//            Debug.LogError("Link sprite has a width of 0 !!!");
		
	}

	public void OnBond() {
//		ParticleSystem particles

//		fxA.GetLineParticles ();
//		fxB.GetLineParticles ();

		//magicalParticlesAcontainer
		//magicalParticlesBcontainer
//		magicalParticlesAsystem
//		magicalParticlesBsystem
	}
	
	Vector3 castLine() {
		Vector3 a = playerA.transform.position;
		Vector3 b = playerB.transform.position;
		a.z = originalZ;
		b.z = originalZ;
		return b - a;
	}
	
	void redrawLink() {

		// first we cast the line between the two gargoyles (the simple math is deferred to a sub-function, for readability)
		Vector3 linkLine = castLine ();
		Vector3 linkLineMiddlePoint = playerA.transform.position + linkLine * 0.5f;
		Vector3 linkLineQuarterLengthPoint = playerA.transform.position + linkLine * 0.25f;

//		float angle = Mathf.Atan2(linkLine.y, linkLine.x) * Mathf.Rad2Deg;
//		linkMiddle.transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
//

		//
		// Met à jour la géométrie du lien
		//

		// Bouge les deux points source et linkmiddle
		emitterAsource.transform.position = playerA.transform.position;
		emitterAlinkMiddle.transform.position = linkLineMiddlePoint;

		// Tourne les deux points source et linkmiddle pour qu'ils se regardent
		float angle = Mathf.Atan2(linkLine.y, linkLine.x) * Mathf.Rad2Deg;
		angle -= 90;
		emitterAsource.transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
		emitterAlinkMiddle.transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);

		//
		// Met à jour les particules
		//

		// forward particles
		forwardLineColliderA.transform.position = linkLineMiddlePoint;

		// magic particles
		magicalParticlesAcontainer.transform.position = linkLineQuarterLengthPoint;
		// magicalParticlesAsystem = ...
		// Faire en sorte que la shape box Y soit égale à la longueur de linkLineMiddlePoint

		// backward particles
//backwardParticlesAcontainer.transform.localPosition = Vector3.zero;
		backwardParticlesAcollider.transform.localPosition = Vector3.zero;



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

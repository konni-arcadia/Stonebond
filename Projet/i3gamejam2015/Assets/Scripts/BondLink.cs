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
	
//	protected ParticleSystem forwardParticlesAsystem;
//	protected ParticleSystem forwardParticlesBsystem;
	
	protected ParticleSystem magicalParticlesAsystem;
	protected ParticleSystem magicalParticlesBsystem;
	
	public GameObject forwardLineColliderA;
	public GameObject forwardLineColliderB;

//	public GameObject backwardParticlesAcontainer;
//	public GameObject backwardParticlesBcontainer;
	
	public GameObject backwardParticlesAcollider;
	public GameObject backwardParticlesBcollider;

	protected Renderer rend;
	protected float originalZ;

    float originalWidth;

	// Use this for initialization
	void Start () {

		fxA = emitterAsource.GetComponent<BondLinkFX> ();
		fxB = emitterBsource.GetComponent<BondLinkFX> ();

		playerAStateController = playerA.GetComponent<PlayerStateController> ();
		playerBStateController = playerB.GetComponent<PlayerStateController> ();

		// TODO: call from LevelManager
		OnBond ();

	}

	public void OnBond() {
//		ParticleSystem particles

		fxA.SetPlayer (playerAStateController.playerId);
		fxB.SetPlayer (playerBStateController.playerId);

		fxA.GetLineParticles ().gameObject.SetActive(true);
		fxB.GetLineParticles ().gameObject.SetActive(true);

//		forwardParticlesAsystem = 
//		forwardParticlesAsystem
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
		// --------------------------------

		//
		// Bouge les deux points source et linkmiddle

		// -- A

		// -- -- source
		emitterAsource.transform.position = playerA.transform.position;

		// -- -- middle point
		emitterAlinkMiddle.transform.position = linkLineMiddlePoint;

		// -- B

		// -- -- source
		emitterBsource.transform.position = playerB.transform.position;

		// -- -- middle point
		emitterBlinkMiddle.transform.position = linkLineMiddlePoint;


		//
		// Tourne les deux points source et linkmiddle pour qu'ils se regardent

		// -- calcul d'angle
		float angle = Mathf.Atan2(linkLine.y, linkLine.x) * Mathf.Rad2Deg;

		// -- A
		float angleA = angle - 90;

		// -- -- source
		emitterAsource.transform.rotation = Quaternion.AngleAxis (angleA, Vector3.forward);
		
		// -- -- middle point
		emitterAlinkMiddle.transform.rotation = Quaternion.AngleAxis (angleA, Vector3.forward);

		// -- B
		float angleB = angle + 90;

		// -- -- source
		emitterBsource.transform.rotation = Quaternion.AngleAxis (angleB, Vector3.forward);
		
		// -- -- middle point
		emitterBlinkMiddle.transform.rotation = Quaternion.AngleAxis (angleB, Vector3.forward);
		

		//
		// Met à jour les particules
		// --------------------------

		//
		// forward particles

		// -- A
		forwardLineColliderA.transform.position = linkLineMiddlePoint;
		
		// -- B
		forwardLineColliderB.transform.position = linkLineMiddlePoint;
		
		// magic particles
///////		magicalParticlesAcontainer.transform.position = linkLineQuarterLengthPoint;
		// magicalParticlesAsystem = ...
		// Faire en sorte que la shape box Y soit égale à la longueur de linkLineMiddlePoint

		// backward particles
//backwardParticlesAcontainer.transform.localPosition = Vector3.zero;

		//
		// complétion du bond

		// -- calcul du pourcentage de complétion (visuel)
		//    pour dire que le lien est à 100%, mettre la local position Y du backwardCollider à 0
		//    pour dire que le lien est à 0%, mettre la local position Y du backwardCollider à <LONGUEUR DU DEMI-LIEN>
		//    formule = position locale du backwardCollider . Y = ( 1 - <PERCENT_COMPLETE (0.00-1.00)> ) * <LONGUEUR DU DEMI-LIEN>
		float fuckingY = (1 - 0.75f) * linkLine.x * 0.5f;
		Vector3 fuckingLocalPos = new Vector3(0, fuckingY, 0);

		// -- A
		backwardParticlesAcollider.transform.localPosition = fuckingLocalPos;
		
		// -- B
		//backwardParticlesBcollider.transform.localPosition = fuckingLocalPos;
		


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

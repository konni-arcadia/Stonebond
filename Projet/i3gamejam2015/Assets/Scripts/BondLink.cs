using UnityEngine;
using System.Collections;


public class BondLink : MonoBehaviour {

	public float completion; // should be between 0.0f and 1.0f

	public GameObject middlePoint;
	public GameObject middleAnimation;
	public Animator middleAnimator;
	public GameObject middleCollider;
	protected BoxCollider2D destroyCollider;

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

	public GameObject backwardParticlesA;
	public GameObject backwardParticlesB;
	
	public GameObject backwardParticlesAcollider;
	public GameObject backwardParticlesBcollider;

	// ------------------------------------------------------------
	// ------------------------------------------------------------
	// Settings for the bond creation animation (during the pause)

	// This variable controls how long the game is frozen when a bond is created.
	// NB: bond link animations are updated but not gargoyles sprites. And players can't move.
	public static float bondCreateGameplayPauseDuration = .9f; // formerly bondCreatePauseTime in LevelManager

	// This variable controls how long the game is frozen when a bond is frozen.
	// NB: bond link animations are updated but not gargoyles sprites. And players can't move.
	public static float bondBreakGameplayPauseDuration = 0.25f; // formerly bondBreakPauseTime in LevelManager

	// This variable controls how long it takes to the first particles to join in the middle of the bond.
	public static float bondCreateForwardParticlesRayDuration = .25f; // should be fast (in any case, should be inferior to the bondCreatePauseTime

	// This variables controls how long to wait after the bond middle blast and before starting to increment the bond's completion.
	public static float bondCreateDelayAfterMiddleBlastDuration = 1f; // formerly increaseStartCooldownSecs, in LevelManager

	protected float bondAnimCreationElapsedTime;

	protected enum BondAnimState {
		ForwardParticlesApproaching,
		SnowBallsAppearing, // = middle blast (energy ball) + gargoyle shields (but "snowballs" is a nickname in hommage to the first glitchy implementation)
		BackwardParticlesTranslating
	}

	protected BondAnimState currentBondAnimState;

	protected bool forwardBlastIsOver = false;

	// ------------------------------------------------------------
	// ------------------------------------------------------------

	protected Renderer rend;
	protected float originalZ;

    float originalWidth;
	
	void Start () {
		middleAnimator = middleAnimation.GetComponent<Animator> ();
	}

	public void LinkPlayers(GameObject player_A, GameObject player_B) {

		// internal references
		completion = 0;
		fxA = emitterAsource.GetComponent<BondLinkFX> ();
		fxB = emitterBsource.GetComponent<BondLinkFX> ();
		destroyCollider = middleCollider.GetComponent<BoxCollider2D> ();

		// link
		playerA = player_A;
		playerB = player_B;
		OnBond ();

	}

	protected void OnBond() {

		// NB forward and backward particles are off
		currentBondAnimState = BondAnimState.ForwardParticlesApproaching;
		forwardBlastIsOver = false;
		bondAnimCreationElapsedTime = 0;

        redrawLink();

		playerAStateController = playerA.GetComponent<PlayerStateController>();
		playerBStateController = playerB.GetComponent<PlayerStateController>();

		fxA.SetPlayer(playerAStateController.playerId);
		fxB.SetPlayer (playerBStateController.playerId);

		fxA.GetLineParticles ().gameObject.SetActive(true);
		fxB.GetLineParticles ().gameObject.SetActive(true);

		// disable all magical particles
		for(int i=0; i<fxA.magicalParticles.Count;++i)
			fxA.magicalParticles[i].IsAlive(false);
		for(int i=0; i<fxB.magicalParticles.Count;++i)
			fxB.magicalParticles[i].IsAlive(false);

		// activate player specific magical particles
		if(fxA.magicalParticles.Count >= playerAStateController.playerId)
			fxA.magicalParticles[playerAStateController.playerId-1].gameObject.SetActive(true);
		if(fxB.magicalParticles.Count >= playerAStateController.playerId)
			fxB.magicalParticles[playerBStateController.playerId-1].gameObject.SetActive(true);
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

		// -- break collider
		middleCollider.transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);


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
		// middle animation (energy sphere)

		middlePoint.transform.position = linkLineMiddlePoint;
		middleAnimation.transform.localPosition = Vector3.zero;
		// TODO? rotate this animation ? Ask Matthieu Bonvin.

		//middleAnimation.


		//
		// forward particles

		// -- anim of forward particles completion
		float forwardBlastAnimCompletion = forwardBlastIsOver ? 1.0f : bondAnimCreationElapsedTime / bondCreateForwardParticlesRayDuration;

		//Debug.Log ( "bondAnimCreationElapsedTime = " + bondAnimCreationElapsedTime + ", forwardBlastAnimCompletion = " + forwardBlastAnimCompletion );

		// -- A
		Vector3 forwardParticlesMovingColliderPositionA = playerA.transform.position + linkLine * 0.5f * forwardBlastAnimCompletion;
		forwardLineColliderA.transform.position = forwardParticlesMovingColliderPositionA;

		// -- B
		Vector3 forwardParticlesMovingColliderPositionB = playerB.transform.position - linkLine * 0.5f * forwardBlastAnimCompletion;
		forwardLineColliderB.transform.position = forwardParticlesMovingColliderPositionB;


		//
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
		float fuckingY = (1 - completion) * linkLine.magnitude * 0.5f;
		Vector3 fuckingLocalPos = new Vector3(0, fuckingY, 0);

		// -- A
		backwardParticlesAcollider.transform.localPosition = fuckingLocalPos;
		
		// -- B
		backwardParticlesBcollider.transform.localPosition = fuckingLocalPos;

		// -- collider
//		destroyCollider.transform.position = middleAnimation.transform.position;
		float colliderSizeX = completion * linkLine.magnitude;
//		Vector2 colliderSize = new Vector2( colliderSizeX, 1);
        Vector2 colliderSize = new Vector2(1, 1);
        destroyCollider.size = colliderSize;

	}
	
	// Note on implementation: the bond link creation animation will work well only if the Bond prefab
	//    is instantiated AFTER the Pause has been added to the TimeManager. It assumes the .isPaused
	//    will be true on the first frame.
	void Update () {

		bondAnimCreationElapsedTime += TimeManager.realDeltaTime;

		//Debug.Log ("bondAnimCreationElapsedTime : " + bondAnimCreationElapsedTime);

		if ( bondAnimCreationElapsedTime > bondCreateForwardParticlesRayDuration ) {
			forwardBlastIsOver = true; // which means forward particles should have already touched in the middle
		}

		switch (currentBondAnimState) {

		case BondAnimState.ForwardParticlesApproaching:

			if (forwardBlastIsOver) {
				currentBondAnimState = BondAnimState.SnowBallsAppearing; // jump to next state
				//Debug.Log("Forward blast over after " + bondAnimCreationElapsedTime);
				middleAnimation.SetActive (true);
				middleAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
				middleAnimator.Play ("Create");
				playerAStateController.ActivateShield ();
				playerBStateController.ActivateShield ();
			} else {
				
			}

			break;

		case BondAnimState.SnowBallsAppearing:

			if (TimeManager.isPaused) {
//				middleAnimator.playbackTime = bondAnimCreationElapsedTime;
			} else {
				//Debug.Log("Exited pause and starting backward particles after " + bondAnimCreationElapsedTime);
//				middleAnimator.StopPlayback ();
//				middleAnimator.Play ("Cycle");
				currentBondAnimState = BondAnimState.BackwardParticlesTranslating; // jump to next state
				backwardParticlesA.SetActive(true);
				backwardParticlesB.SetActive(true);
				backwardParticlesAcollider.SetActive(true);
				backwardParticlesBcollider.SetActive(true);
			}

			break;

		case BondAnimState.BackwardParticlesTranslating:
			break;

		default:
			break;

		}

//		middleAnimator.playbackTime = bondAnimCreationElapsedTime;

		// We ignore the pause in this class
		redrawLink ();

	}
}

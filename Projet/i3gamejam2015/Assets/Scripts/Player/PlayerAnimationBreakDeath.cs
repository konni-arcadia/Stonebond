using UnityEngine;
using System.Collections;
using System;

using DG.Tweening;

public class PlayerAnimationBreakDeath : MonoBehaviour {

	// NB : bug, lorsque je fais bodyParts.SetActive(false) il disable les Sprite Renderer des objets enfants
	// J'ai pas encore compris pourquoi
	// Il serait possible de les re-enabler en chaîne mais j'aimerais comprendre
	// Bon je l'ai fait

	// TODO uniformiser : System.Random ou UnityEngine.Random mais pas les deux

	// Calibration vars
	public float minStrength;
	public float maxStrength;
	public float explosionDuration = 1.2f;
	public float lyingDownDuration = 0.9f;
	public float reassemblingDuration = 0.35f;
	public float fadingBackToInactiveDuration = 0.3f;
	// TODO: add gravityScale (maybe)
	// TODO: add the shaking before reassembling parameters (intervals if many shakes - re-code - and vibrato, strength, etc)
	// TODO: add reassembling Easing function (maybe)

	// Internal refs
	private System.Random rand;
	private GameObject bodyPartsContainer;
	private Rigidbody2D[] rigidBodies;

	// Internal state vars
	private float internalClock;
	private enum State {
		Inactive, StartingDeath, Exploding, LyingDownBrokenInPieces, Reassembling, FadingBackToInactive
	}
	private State currentState = State.Inactive;

	// DEBUG VARS -- begin
	public Vector2 hitDirection;
//	public bool mustDie;
	// DEBUG VARS -- end

	void Start () {
		// set to position of parent
		transform.localPosition = Vector3.zero;
		bodyPartsContainer = transform.Find ("BodyParts").gameObject;
		rand = new System.Random ();
		rigidBodies = bodyPartsContainer.GetComponentsInChildren<Rigidbody2D> () as Rigidbody2D[];
		for (int i = 0; i < rigidBodies.Length; i++) {
			rigidBodies [i].isKinematic = true;
			rigidBodies [i].gravityScale = 2.5f;
		}
		SetState (State.Inactive);
	}

	public void Die() {
		if (currentState == State.Inactive) {
			SetState (State.StartingDeath);
		}
	}

	// stops ongoing stuff if needed
	private void SetState(State newState) {

		// set the state
		currentState = newState;

		// performs internal changes according to the new state
		switch (currentState) {
		case State.Inactive:
			bodyPartsContainer.SetActive (false);
			break;
		default:
			bodyPartsContainer.SetActive (true);
			SpriteRenderer[] sprites = bodyPartsContainer.GetComponentsInChildren<SpriteRenderer> ();
			foreach (var s in sprites) {
				s.enabled = true;
			}
			break;
		}

	}

	// NB: this is not an Animation in the traditional meaning. It is based on the physics engine.
	private void StartExplosionAnim(Vector2 directionOfHit) {

		// foolproof
		if (currentState == State.Exploding) {
			// an animation is already ongoing
			return;
		}

		//Debug.Log ("BOUM");

		internalClock = 0.0f;

		SetState (State.Exploding);

		directionOfHit.Normalize ();

		for (int i = 0; i < rigidBodies.Length; i++) {
			rigidBodies [i].isKinematic = false;
			Vector2 force;
			if (directionOfHit.magnitude > 0.0f) {
				// the parts spread in a given direction
				force = directionOfHit;
			} else {
				// the parts explode in a circle
				float degrees = (float) rand.Next( 0, 360);
				force = Quaternion.Euler (0, 0, degrees) * Vector2.right;
				force.Normalize ();
				Debug.Log (force);
			}
			float strength = minStrength + (((float)rand.Next( 1, 100)) * (maxStrength - minStrength) / 100.0f);
			rigidBodies [i].isKinematic = false;
			rigidBodies [i].AddForce( force * strength, ForceMode2D.Impulse );
		}

	}

	// NB: this is not an Animation in the traditional meaning. It is based on the physics engine.
	private void StartLyingDownAnims() {

		// foolproof
		if (currentState == State.LyingDownBrokenInPieces) {
			// an animation is already ongoing
			return;
		}

		internalClock = 0.0f;
		
		SetState (State.LyingDownBrokenInPieces);

//		Sequence seq = DOTween.Sequence ();
//		for (int i = 0; i < rigidBodies.Length; i++) {
//			seq.Insert (0, rigidBodies [i].transform.DOShakePosition (lyingDownDuration * 1.2f, new Vector3 (0.7f, 0.1f), 10, 180, false));
//		}
//		seq.Play ();
//		seq.OnComplete (() => {
//			//currentState = State.Inactive;
//		});

		for (int i = 0; i < rigidBodies.Length; i++) {
			rigidBodies [i].AddForce (Vector2.up * UnityEngine.Random.Range(10, 30), ForceMode2D.Impulse);
		}

	}

	// NB: this is not an Animation in the traditional meaning. It is based on the physics engine.
	private void StartReassemblingAnim() {

		// foolproof
		if (currentState == State.Reassembling) {
			// an animation is already ongoing
			return;
		}

		internalClock = 0.0f;

		SetState (State.Reassembling);

//		for (int i = 0; i < rigidBodies.Length; i++) {
//			rigidBodies [i].isKinematic = true;
//			rigidBodies [i].transform.localPosition = Vector3.zero;
//			rigidBodies [i].transform.localRotation = Quaternion.identity;
//		}

		Sequence seq = DOTween.Sequence ();
		for (int i = 0; i < rigidBodies.Length; i++) {
			rigidBodies [i].isKinematic = true;
			seq.Insert (0, rigidBodies [i].transform.DOLocalMove (Vector3.zero, reassemblingDuration).SetEase(Ease.InCirc));
			seq.Insert (0, rigidBodies [i].transform.DOLocalRotateQuaternion (Quaternion.identity, reassemblingDuration).SetEase(Ease.InCirc));
		}
		seq.Play ();
		seq.OnComplete (StartFadingBackToInactiveAnim);

	}

	private void StartFadingBackToInactiveAnim() {

		// foolproof
		if (currentState == State.FadingBackToInactive) {
			// an animation is already ongoing
			return;
		}

		internalClock = 0.0f;

		SetState(State.FadingBackToInactive);

	}

	void Update () {

// DEBUG CODE
//		if (mustDie) {
//			mustDie = false;
//			Die ();
//		}
// DEBUG CODE

		internalClock += Time.deltaTime;
		
		if (currentState == State.StartingDeath) {
			StartExplosionAnim (hitDirection);
		}
		
		if (currentState == State.Exploding && internalClock > explosionDuration + lyingDownDuration/2) {
			StartLyingDownAnims ();
		}

		if (currentState == State.LyingDownBrokenInPieces && internalClock > lyingDownDuration) {
			StartReassemblingAnim ();
		}

		if (currentState == State.FadingBackToInactive && internalClock > fadingBackToInactiveDuration) {
			SetState (State.Inactive);
		}

	}
}

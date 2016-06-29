using UnityEngine;
using System.Collections;
using System;

public class PlayerAnimationBreakDeath : MonoBehaviour {

	// Calibration vars
	public float minStrength;
	public float maxStrength;
	public float explosionDuration = 2.0f;
	public float lyingDownDuration = 2.0f;

	// Internal refs
	private System.Random rand;
	private Rigidbody2D[] rigidBodies;

	// Internal state vars
	private float internalClock;
	private enum State {
		Inactive, StartingDeath, Exploding, LyingDownBrokenInPieces, Reassembling
	}
	private State currentState;


	public Vector2 hitDirection;
	public bool mustDie;

	void Start () {
		currentState = State.Inactive;
		rand = new System.Random ();
		rigidBodies = transform.gameObject.GetComponentsInChildren<Rigidbody2D> () as Rigidbody2D[];
		for (int i = 0; i < rigidBodies.Length; i++) {
			rigidBodies [i].isKinematic = true;
			rigidBodies [i].gravityScale = 2.5f;
		}
	}

	public void Die() {
		if (currentState == State.Inactive) {
			currentState = State.StartingDeath;
		}
	}

	// stops ongoing stuff if needed
	private void SetState(State newState) {
		currentState = newState;
	}

	// NB: this is not an Animation in the traditional meaning. It is based on the physics engine.
	private void StartExplosionAnim(Vector2 directionOfHit) {

		// foolproof
		if (currentState == State.Exploding) {
			// an animation is already ongoing
			return;
		}

		internalClock = 0.0f;
		
		Debug.Log ("BOUM");
		
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

	void Update () {

// DEBUG CODE
		if (mustDie) {
			mustDie = false;
			Die ();
		}
// DEBUG CODE

		internalClock += Time.deltaTime;
		
		if (currentState == State.StartingDeath) {
			StartExplosionAnim (hitDirection);
		}
		
		if (currentState == State.Exploding && internalClock > explosionDuration) {
			currentState = State.LyingDownBrokenInPieces;
			internalClock = 0.0f;
//			for (int i = 0; i < rigidBodies.Length; i++) {
//				rigidBodies [i].isKinematic = true;
//			}
		}

		if (currentState == State.LyingDownBrokenInPieces && internalClock > lyingDownDuration) {
			currentState = State.Reassembling;
			for (int i = 0; i < rigidBodies.Length; i++) {
				rigidBodies [i].isKinematic = true;
				rigidBodies [i].transform.localPosition = Vector3.zero;
				rigidBodies [i].transform.localRotation = Quaternion.identity;
			}

currentState = State.Inactive;
				
		}

	}
}

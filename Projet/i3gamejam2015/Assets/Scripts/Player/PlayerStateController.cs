using UnityEngine;
using System.Collections;

public class PlayerStateController : MonoBehaviour {
	private bool facingRight;
	public int playerId;

	// Use this for initialization
	void Start () {}
	
	// Update is called once per frame
	void Update() { }

	// Returns whether the character is facing right
	public bool IsFacingRight() {
		return facingRight;
	}

	public bool IsInvincible() {
		return false;
	}

	public bool IsLinked() {
		return false;
	}

	public bool IsStun() {
		return false;
	}

	public int PlayerId() {
		return playerId;
	}

	// Meant to be used by the player movement controller only
	public void SetFacingRight(bool rightDirection) {
		facingRight = rightDirection;
	}
}

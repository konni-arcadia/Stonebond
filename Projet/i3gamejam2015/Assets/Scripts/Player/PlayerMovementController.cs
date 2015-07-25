using UnityEngine;
using System.Collections;

public class PlayerMovementController : MonoBehaviour {

	public float maxSpeed;				// The fastest the player can travel in the x axis.
	public float maxFallSpeed;
	public float breakForce;
	public float initialJumpForce;
	public float extensionJumpForce;
	public Transform[] groundChecks;			// A position marking where to check if the player is grounded.

	public AudioClip jumpClip;
	public AudioClip landClip;

	private bool grounded = false;			// Whether or not the player is grounded.

	// Executed at next iteration
	private bool wantJump = false, wantJumpExtension = false;
	private int playerId;
	private float allowJumpTime = 0;

	private Rigidbody2D body;
	private InputManager inputManager;

	void Awake() {
		// Setting up references.
		body = GetComponent<Rigidbody2D>();
		inputManager = FindObjectOfType<InputManager>();
		playerId = GetComponent<PlayerStateController>().GetPlayerId();
	}


	void Update() {
		// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
		bool wasGrounded = grounded;
		grounded = false;
		for (int i = 0; i < groundChecks.Length; i++)
			grounded = grounded || Physics2D.Linecast(transform.position, groundChecks[i].position, 1 << LayerMask.NameToLayer("Ground"));
		// Allow jumping even after we left the ground for a short period
		if (grounded)
			allowJumpTime = 0.1f;
		else
			allowJumpTime = Mathf.Max(0, allowJumpTime - Time.deltaTime);

		// If the jump button is pressed and the player is grounded then the player should jump.
		if (inputManager.WasPressed(playerId, InputManager.A) && allowJumpTime > 0)
			wantJump = true;
		if (inputManager.IsHeld(playerId, InputManager.A) && wantJumpExtension)
			wantJumpExtension &= body.velocity.y > 0;		// not able to extend jump anymore when grounded
		else
			wantJumpExtension = false;
	}

	void FixedUpdate() {
		// Cache the horizontal input.
		float h = inputManager.AxisValue(playerId, InputManager.Horizontal);

		// If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
//		body.AddForce(Vector2.right * h * moveForce);

		Vector2 velocity = body.velocity;
		float targetSpeed = h * maxSpeed;
		velocity.x += (targetSpeed - velocity.x) / 2.0f;

		body.velocity = velocity;

		// If the player should jump...
		if (wantJump) {
			AudioSource.PlayClipAtPoint(jumpClip, body.position);

			// Ensure that the current vy doesn't take in account
			Vector2 vel = body.velocity;
			vel.y = 0;
			body.velocity = vel;
			// Add a vertical force to the player.
			body.AddForce(new Vector2(0.0f, initialJumpForce));

			// Make sure the player can't jump again until the jump conditions from Update are satisfied.
			wantJump = false;
			wantJumpExtension = true;
		}
		else if (wantJumpExtension) {
			body.AddForce(new Vector2(0, extensionJumpForce * Time.fixedDeltaTime));
		}

	}

	public void setMovementEnabled(bool enabled) {
		// TODO
	}

	public void applyForce(Vector2 force) {
		body.AddForce (force);
	}
}

using UnityEngine;
using System.Collections;

public class PlayerMovementController : MonoBehaviour {

	public float maxSpeed = 5.0f / 5;				// The fastest the player can travel in the x axis.
	public float maxFallSpeed = 100.0f / 5;
	public float breakForce = 200.0f / 5;
	public float jumpForce = 1000.0f / 5;			// Amount of force added when the player jumps
	public Transform[] groundChecks;			// A position marking where to check if the player is grounded.

	public AudioClip jumpClip;
	public AudioClip landClip;

	private bool jump = false;				// Condition for whether the player should jump.
	private bool grounded = false;			// Whether or not the player is grounded.

	private Transform groundCheck;			// A position marking where to check if the player is grounded.
	private Rigidbody2D body;
	private InputManager inputManager;
	private int playerId;

	void Awake() {
		// Setting up references.
		groundCheck = transform.Find("groundCheck");
		body = GetComponent<Rigidbody2D>();
		inputManager = FindObjectOfType<InputManager>();
		playerId = GetComponent<PlayerStateController>().PlayerId();
	}


	void Update() {
		Debug.Log("V4: " + inputManager.AxisValue(4, InputManager.Horizontal));
		// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
		bool wasGrounded = grounded;
		grounded = false;
		for (int i = 0; i < groundChecks.Length; i++)
			grounded = grounded || Physics2D.Linecast(transform.position, groundChecks[i].position, 1 << LayerMask.NameToLayer("Ground"));

		// If the jump button is pressed and the player is grounded then the player should jump.
		if (inputManager.IsHeld(playerId, InputManager.A) && grounded) {
			// jump
			jump = true;
		}
	}

	void FixedUpdate() {
		// Cache the horizontal input.
		float h = inputManager.AxisValue(playerId, InputManager.Horizontal);

		// If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
//		body.AddForce(Vector2.right * h * moveForce);

		Vector2 velocity = body.velocity;
		float targetSpeed = h * maxSpeed;
//		velocity.x += (targetSpeed - velocity.x) / 5.0f;
		velocity.x += (targetSpeed - velocity.x) / 3.0f;

/*		if (h == 0.0f) {
			if (velocity.x > 0.0f) {
				velocity.x -= breakForce * Time.fixedDeltaTime;
				if (velocity.x < 0.0f) velocity.x = 0.0f;
			}
			else if (velocity.x < 0.0f) {
				velocity.x += breakForce * Time.fixedDeltaTime;
				if (velocity.x > 0.0f) velocity.x = 0.0f;
			}
		}

		// If the player's horizontal velocity is greater than the maxSpeed...
		if (Mathf.Abs(velocity.x) > maxSpeed) {
			// ... set the player's velocity to the maxSpeed in the x axis.
			velocity.x = Mathf.Sign(velocity.x) * maxSpeed;
		}*/

		if (velocity.y < -maxFallSpeed) {
			velocity.y = -maxFallSpeed;
		}

		body.velocity = velocity;

		// If the player should jump...
		if (jump) {
			AudioSource.PlayClipAtPoint(jumpClip, body.position);

			// Add a vertical force to the player.
			body.AddForce(new Vector2(0.0f, jumpForce));

			// Make sure the player can't jump again until the jump conditions from Update are satisfied.
			jump = false;
		}
	}
}

using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
	[HideInInspector]
	public bool jump = false;				// Condition for whether the player should jump.


	public float moveForce = 365.0f;			// Amount of force added to move the player left and right.
	public float maxSpeed = 5.0f;				// The fastest the player can travel in the x axis.
	public float maxFallSpeed = 100.0f;
	public float breakForce = 200.0f;
	public float jumpForce = 1000.0f;			// Amount of force added when the player jumps

	public AudioClip jumpClip;
	public AudioClip landClip;
	
	private bool grounded = false;			// Whether or not the player is grounded.

	private Transform groundCheck;			// A position marking where to check if the player is grounded.
	private Rigidbody2D body;

	void Awake()
	{
		// Setting up references.
		groundCheck = transform.Find("groundCheck");
		body = GetComponent<Rigidbody2D> ();
	}


	void Update()
	{
		// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
		bool wasGrounded = grounded;
		grounded = Physics2D.Linecast(body.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));  

		// If the jump button is pressed and the player is grounded then the player should jump.
		if (Input.GetButtonDown ("Jump") && grounded) {
			// jump
			Debug.Log ("jump", gameObject);
			jump = true;
		}
	}

	void FixedUpdate ()
	{
		// Cache the horizontal input.
		float h = Input.GetAxis("Horizontal");
		
		// If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
		if (h * body.velocity.x < maxSpeed) {
			// ... add a force to the player.
			body.AddForce (Vector2.right * h * moveForce);
		}

		Vector2 velocity = body.velocity;
		if (h == 0.0f) {
			if(velocity.x > 0.0f) {
				velocity.x -= breakForce * Time.fixedDeltaTime;
				if(velocity.x < 0.0f) velocity.x = 0.0f;
			}
			else if(velocity.x < 0.0f) {
				velocity.x += breakForce * Time.fixedDeltaTime;
				if(velocity.x > 0.0f) velocity.x = 0.0f;
			}
		}

		// If the player's horizontal velocity is greater than the maxSpeed...
		if (Mathf.Abs (velocity.x) > maxSpeed) {
			// ... set the player's velocity to the maxSpeed in the x axis.
			velocity.x = Mathf.Sign (velocity.x) * maxSpeed;
		}

		if (body.velocity.y < -maxFallSpeed) {
			velocity.y = -maxFallSpeed;
		}

		body.velocity = velocity;

		// If the player should jump...
		if(jump)
		{
			AudioSource.PlayClipAtPoint(jumpClip, body.position);

			// Add a vertical force to the player.
			body.AddForce(new Vector2(0.0f, jumpForce));

			// Make sure the player can't jump again until the jump conditions from Update are satisfied.
			jump = false;
		}
	}
}

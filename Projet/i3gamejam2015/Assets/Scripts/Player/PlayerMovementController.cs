using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovementController : MonoBehaviour
{

    public float maxSpeed;				// The fastest the player can travel in the x axis.
    public float maxFallSpeed;
    public float breakForce;
    public float initialJumpForce;
    public float extensionJumpForce;
    public float wallJumpLateralForce;
    public float maxVelocityWhenGrinding;
    public Transform[] groundChecks;			// A position marking where to check if the player is grounded.
    public Transform raycastBase, wallJumpCheck;
    public bool startsFacingRight = true;
	public float frictionFactorGround = 0.5f;
	public float frictionFactorAir = 0.1f;

    public AudioClip jumpClip;
    public AudioClip landClip;

    private bool grounded = false;			// Whether or not the player is grounded.
    private bool isGrinding = false;

	private float originalGravityScale;

    // Executed at next iteration
    private bool wantJump = false, wantWallJump = false, wantJumpExtension = false;
    private int playerId;
    private float allowJumpTime = 0, disallowDirectionTime = 0;
    private List<Vector2> pendingForcesToApply = new List<Vector2>();
    private bool inWallJump = false;
	private bool isJumpEnabled = true;
	private float fixedFrictionFactor = 0.0f;
	private float movementFactor = 1.0f;

    private Rigidbody2D body;
    private InputManager inputManager;
    private Collider2D bodyCollider;

    private PlayerStatusProvider myStatusProvider;

    void Awake()
    {
        // Setting up references.
        body = GetComponent<Rigidbody2D>();
        inputManager = FindObjectOfType<InputManager>();        
        bodyCollider = transform.Find("bodyCollider").GetComponent<Collider2D>();        
        myStatusProvider = GetComponent<PlayerStatusProvider>();
    }

	void Start()
	{
		playerId = GetComponent<PlayerStateController>().GetPlayerId();
		originalGravityScale = body.gravityScale;
		setFacingRight(startsFacingRight);
	}

    void Update()
    {
        // The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
        grounded = false;
        if (body.velocity.y <= 0)
        {
            for (int i = 0; i < groundChecks.Length; i++)
                grounded = grounded || Physics2D.Linecast(raycastBase.position, groundChecks[i].position, 1 << LayerMask.NameToLayer("Ground"));
        }

        //Update grounded status in player status component
        myStatusProvider.setGroundedStatus(grounded);

        // Allow jumping even after we left the ground for a short period
        if (grounded)
            allowJumpTime = 0.1f;
        else
            allowJumpTime = Mathf.Max(0, allowJumpTime - Time.deltaTime);

        disallowDirectionTime = Mathf.Max(0, disallowDirectionTime - Time.deltaTime);
        // Wall jump state stops when grounded
        inWallJump = inWallJump && !grounded;

        isGrinding = isJumpEnabled && allowJumpTime < Mathf.Epsilon &&
            body.velocity.y < 0 && Physics2D.Linecast(raycastBase.position, wallJumpCheck.position, 1 << LayerMask.NameToLayer("Ground"));

        //Update grounded status in player status component
        myStatusProvider.setOnWallStatus(isGrinding);

        // If the jump button is pressed and the player is grounded then the player should jump.
        if (isJumpEnabled && inputManager.WasPressed(playerId, InputManager.A))
        {
            // Standard way of jumping (allowed to jump)
            if (allowJumpTime > 0)
                wantJump = true;
            // Or by going down while on a wall
            else if (isGrinding)
                wantWallJump = true;
        }
        if (inputManager.IsHeld(playerId, InputManager.A) && wantJumpExtension)
            wantJumpExtension &= body.velocity.y > 0;		// not able to extend jump anymore when grounded
        else
            wantJumpExtension = false;
    }

    void FixedUpdate()
    {
        foreach (Vector2 vector in pendingForcesToApply)
            body.AddForce(vector);
        pendingForcesToApply.Clear();

        // Cache the horizontal input.
        float h = disallowDirectionTime == 0 ? inputManager.AxisValue(playerId, InputManager.Horizontal) : 0;
		h *= movementFactor;

        // Facing right?
        if (Mathf.Abs(h) >= Mathf.Epsilon)
            setFacingRight(h > 0);

        //Update the horizontal input in the player status component
        myStatusProvider.setAxisHValue(Mathf.Abs(h));

        // If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
        //		body.AddForce(Vector2.right * h * moveForce);

        Vector2 velocity = body.velocity;
		float targetSpeed = h * maxSpeed;
		float frictionFactor;
		if (fixedFrictionFactor == 0.0f)
		{
			frictionFactor = grounded && !inWallJump ? frictionFactorGround : frictionFactorAir;
		}
		else
		{
			frictionFactor = fixedFrictionFactor;
		}

        // In the air, there is less friction
        velocity.x += (targetSpeed - velocity.x) * frictionFactor;
        // When grinding, limit the velocity
        if (isGrinding && velocity.y < maxVelocityWhenGrinding)
            velocity.y = maxVelocityWhenGrinding;
        body.velocity = velocity;

        // If the player should jump...
        if (wantJump || wantWallJump)
        {
			if(wantWallJump) {
				myStatusProvider.setWallJump();
			}
			else {
				myStatusProvider.setJump ();
			}

            // Ensure that the current vy doesn't take in account
            Vector2 vel = body.velocity;
            vel.y = 0;
            body.velocity = vel;
            // Add a vertical force to the player.
            body.AddForce(new Vector2(0.0f, initialJumpForce));

            // Also move the player away from the wall
            if (wantWallJump)
            {
                body.AddForce(new Vector2(isFacingRight() ? -wallJumpLateralForce : wallJumpLateralForce, 0));
                setFacingRight(!isFacingRight());
                disallowDirectionTime = 0.3f;
                inWallJump = true;
            }

            // Make sure the player can't jump again until the jump conditions from Update are satisfied.
            wantJump = wantWallJump = false;
            wantJumpExtension = true;
        }
        else if (wantJumpExtension)
        {
            body.AddForce(new Vector2(0, extensionJumpForce * Time.fixedDeltaTime));
        }

        //Update Y velocity in player status component
        myStatusProvider.setVelocityYValue(body.velocity.y);
    }

	public void resetForces()
	{
		body.velocity = new Vector2(0, 0);
		wantJumpExtension = false;
		pendingForcesToApply.Clear();
	}

    public void setMovementFactor(float factor)
    {
		movementFactor = factor;
    }

	public void setJumpEnabled(bool enabled)
	{
		isJumpEnabled = enabled;
	}

	public void setGravityFactor(float factor)
	{
		body.gravityScale = factor * originalGravityScale;
	}

	// must be set between 0.0f and 1.0f
	// set to 0.0f to disable fixed friction and have friction depend on the fact the player is on air or ground
	public void setFixedFrictionFactor(float factor)
	{
		fixedFrictionFactor = factor;
	}

    public void applyForce(Vector2 force)
    {
        pendingForcesToApply.Add(force);
    }

    public bool isFacingRight()
    {
        return transform.localScale.x > 0;
    }

    public bool isGrounded()
    {
        return grounded;
    }

    // Freezes the player until an applyForce is called
  //  public void resetForces()
    //{
      //  body.velocity = new Vector2(0, 0);
        //wantJumpExtension = false;
    //}

    public void setFacingRight(bool facingRight)
    {
        Vector2 val = transform.localScale;
        val.x = facingRight ? 1 : -1;
        transform.localScale = val;
    }
}

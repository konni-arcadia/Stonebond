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
    public float oneWayPlatformHitAngle = 20.0f;

    public bool gameOver = false;

    private bool grounded = true;			// Whether or not the player is grounded.
    private bool isGrinding = false;
    private bool onWall = false;

	private float originalGravityScale;

    // Executed at next iteration
    private bool wantJump = false, wantWallJump = false, wantJumpExtension = false, wantDropThru = false;
    private int playerId;
    private float allowJumpTime = 0, disallowDirectionTime = 0;
    private bool inWallJump = false;
	private bool isJumpEnabled = true;
	private bool isMovementEnabled = true;
    private bool isFrictionEnabled = true;

    private Rigidbody2D body;
    private InputManager inputManager;
    private Collider2D coll;

    private PlayerStatusProvider myStatusProvider;

    void Awake()
    {
        // Setting up references.
        body = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        inputManager = FindObjectOfType<InputManager>();              
        myStatusProvider = GetComponent<PlayerStatusProvider>();
    }

	void Start()
	{
		playerId = GetComponent<PlayerStateController>().GetPlayerId();
		originalGravityScale = body.gravityScale;
		setFacingRight(startsFacingRight);
        myStatusProvider.setGroundedStatus(grounded);
        myStatusProvider.setGrindingStatus(isGrinding);
        myStatusProvider.setOnWall(onWall);
	}

    void Update()
    {
        if (gameOver)
            return;

        bool wasGrounded = grounded;
        grounded = body.velocity.y <= 0 && IsHittingSolid(raycastBase, groundChecks, Vector2.down);

        if (wasGrounded != grounded)
        {
            myStatusProvider.setGroundedStatus(grounded);
        }

        // Allow jumping even after we left the ground for a short period
        if (grounded)
            allowJumpTime = 0.1f;
        else
            allowJumpTime = Mathf.Max(0, allowJumpTime - Time.deltaTime);

        disallowDirectionTime = Mathf.Max(0, disallowDirectionTime - Time.deltaTime);
        // Wall jump state stops when grounded
        inWallJump = inWallJump && !grounded;

        bool wasOnWall = onWall;
        onWall = IsHittingSolid(raycastBase.position, wallJumpCheck.position, isFacingRight() ? Vector2.right : Vector2.left);

        if (wasOnWall != onWall)
        {
            myStatusProvider.setOnWall(onWall);
        }

        bool wasGrinding = isGrinding;
        isGrinding = isJumpEnabled && allowJumpTime < Mathf.Epsilon && body.velocity.y < 0 && onWall;

        if (wasGrinding != isGrinding)
        {
            //Update grounded status in player status component
            myStatusProvider.setGrindingStatus(isGrinding);
        }

        //Cache the vertical input.
        float v = isMovementEnabled && disallowDirectionTime == 0 ? inputManager.AxisValue(playerId, InputManager.Vertical) : 0;

        // If the jump button is pressed and the player is grounded then the player should jump.
		if (isJumpEnabled && inputManager.WasPressed(playerId, InputManager.A)) {
			bool isHoldingDown = v < 0.5 ? false : true;
			// Standard way of jumping (allowed to jump)
			if (allowJumpTime > 0 && !isHoldingDown)
				wantJump = true;
			// Or by going down while on a wall
			else if (isGrinding && !isHoldingDown)
				wantWallJump = true;
			// Drop trhough OWP if down is held
			else if (isHoldingDown)
				wantDropThru = true;

			//If he holds down, he doesnt jump
		}

        if (isJumpEnabled && inputManager.IsHeld(playerId, InputManager.A) && wantJumpExtension)
            wantJumpExtension &= body.velocity.y > 0;		// not able to extend jump anymore when grounded
        else
            wantJumpExtension = false;

    }

    void FixedUpdate()
    {
        if (gameOver)
            return;

        // Cache the horizontal input.
        float h = isMovementEnabled && disallowDirectionTime == 0 ? inputManager.AxisValue(playerId, InputManager.Horizontal) : 0;


        // Facing right?
        if (Mathf.Abs(h) >= Mathf.Epsilon)
            setFacingRight(h > 0);

        //Update the horizontal input in the player status component
        myStatusProvider.setAxisHValue(Mathf.Abs(h));

        // If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
        //		body.AddForce(Vector2.right * h * moveForce);

        Vector2 velocity = body.velocity;
		
        float frictionFactor = grounded && !inWallJump ? frictionFactorGround : frictionFactorAir;
        if (isMovementEnabled)
        {
            float targetSpeed = h * maxSpeed;
            velocity.x += (targetSpeed - velocity.x) * (isFrictionEnabled ? frictionFactor : 1.0f);
        }
        else if (isFrictionEnabled)
        {
            velocity.x -= velocity.x * frictionFactor;
        }
 
        // When grinding, limit the velocity
        if (isGrinding && velocity.y < maxVelocityWhenGrinding)
            velocity.y = maxVelocityWhenGrinding;
		setVelocity(velocity);

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
			setVelocity(vel);
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

        if (wantDropThru)
        {
			wantDropThru = false;
			TryDropThru();
        }

        //Update Y velocity in player status component
        myStatusProvider.setVelocityYValue(body.velocity.y);
    }

    public void resetVelocity(bool resetX = true, bool resetY = true)
	{
		body.velocity = new Vector2(resetX ? 0.0f : body.velocity.x, resetY ? 0.0f : body.velocity.y);
        if (resetY)
        {
            wantJumpExtension = false;
        }
	}

    public void setVelocity(Vector2 velocity)
    {
        body.velocity = velocity;
    }

    public void setMovementEnabled(bool enabled)
    {
        isMovementEnabled = enabled;
    }

	public void setJumpEnabled(bool enabled)
	{
		isJumpEnabled = enabled;
        if(!enabled)
        {
            wantJump = false;
            wantWallJump = false;
            wantJumpExtension = false;
            wantDropThru = false;
        }
	}

    public void setFrictionEnabled(bool enabled)
    {
        isFrictionEnabled = enabled;
    }

	public void setGravityFactor(float factor)
	{
		body.gravityScale = factor * originalGravityScale;
	}

    // must be called within FixedUpdate()
    public void applyForce(Vector2 force)
    {
        body.AddForce(force);
    }

    public bool isFacingRight()
    {
        return transform.localScale.x > 0;
    }

    public bool isGrounded()
    {
        return grounded;
    }

    public void setFacingRight(bool facingRight)
    {
        Vector2 val = transform.localScale;
        val.x = facingRight ? 1 : -1;
        transform.localScale = val;
    }

    private RaycastHit2D[] raycastHits = new RaycastHit2D[32];
    public bool IsHittingSolid(Vector2 a, Vector2 b, Vector2 movementDirection)
    {
        int hitCount = Physics2D.LinecastNonAlloc(a, b, raycastHits, 1 << LayerMask.NameToLayer("Ground"));
        for(int i  = 0; i < hitCount; ++i)
        {
            RaycastHit2D hit = raycastHits[i];
            
            if (hit.collider == null || hit.collider.isTrigger)
            {
                continue;
            }
            
            if (hit.collider.transform.GetComponent<OneWayPlatform>() == null)
            {
                // not a one way platform, this is an hit
                return true;
            }
            
            // only collide with one way platform if the player is going down
            if(Vector2.Angle(movementDirection, Vector2.down) < oneWayPlatformHitAngle)
            {
                return true;
            }
        }
        
        return false;
    }

    public bool IsHittingSolid(Transform start, Transform[] ends, Vector2 movementDirection)
    {
        for (int i = 0; i < ends.Length; i++)
        {
            if(IsHittingSolid(start.position, ends [i].position, movementDirection))
            {
                return true;
            }
        }
        return false;
    }

    public void TryDropThru()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundChecks[1].transform.position, groundChecks[0].transform.position);
		// Only available on one-way platforms (OWP)
		if (hit.collider.transform.GetComponent<OneWayPlatform>() != null)
            Physics2D.IgnoreCollision(hit.collider, coll, true);
    }

    
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerStateController : MonoBehaviour
{
	//
	// REFERENCES
	//

	private InputManager inputManager;
	private PlayerMovementController movementController;
	private PlayerStatusProvider statusProvider;
	private List<PlayerStateController> enemies = new List<PlayerStateController> ();
	
	// 
	// GLOBAL STATE
	//

	public int playerId;

	private enum State
	{
		SPAWN,
		IDLE,
		SLASH_ATTACK,
		KNOCKBACK,
		SLASHED,
		INVINCIBLE
	}

	private State state = State.SPAWN;
	private float stateTime = 0.0f;
	private bool isBond = false;

	//
	// COLLIDERS
	//

	private Collider2D bodyCollider;
	private Collider2D slashAttackColliderForward;
	private Collider2D slashAttackColliderUp;
	private Collider2D slashAttackColliderDown;

	//
	// AIM DIRECTION
	//
	
	private enum AimDirection
	{
		UP,
		DOWN,
		FORWARD
	}

	private AimDirection aimDirection = AimDirection.FORWARD;
    public float VerticalAimThresholdDegree = 90f;

	//
	// SPAWN
	//

	public float spawnTime = 2.0f;

	//
	// SLASH ATTACK
	// 

	public float slashAttackTime = 1.0f;
	public float slashAttackCooldownTime = 4.0f;
	public float slashAttackDashVelocityUp = 1000.0f;
	public float slashAttackDashVelocityDown = 1000.0f;
	public float slashAttackDashVelocityForwardGround = 2000.0f;
	public float slashAttackDashVelocityForwardAir = 2000.0f;
	public float slashAttackHorizDashGravity = 20.0f;
	public AnimationCurve slashAttackDashCurve;
	private bool airDash = false;

	//
	// KNOCKBACK
	//

	public float knockbackTime = 2.0f;
	public float knockbackVelocityUp = 2000.0f;
	public float knockbackVelocityDown = 2000.0f;
	public float knockbackVelocityForward = 3000.0f;
	public AnimationCurve knockbackCurve;

	private float slashAttackCooldown;
	private AimDirection knockbackDirection;

	//
	// SLASHED
	//

	public float slashedTime = 2.0f;
	public float invincibleAfterSpawnTime = 5.0f;
	private GameObject crystal = null;

	//
	// INIT
	//

	private bool initialized = false;

	void Awake ()
	{
		print ("Awake player " + playerId);

		bodyCollider = transform.Find ("bodyCollider").GetComponent<Collider2D> ();

		slashAttackColliderForward = transform.Find ("slashAttackColliderForward").GetComponent<Collider2D> ();
		slashAttackColliderUp = transform.Find ("slashAttackColliderUp").GetComponent<Collider2D> ();
		slashAttackColliderDown = transform.Find ("slashAttackColliderDown").GetComponent<Collider2D> ();
		slashAttackColliderUp.enabled = false;
		slashAttackColliderDown.enabled = false;
		slashAttackColliderForward.enabled = false;

		inputManager = FindObjectOfType<InputManager>();
		movementController = GetComponent<PlayerMovementController>();
		statusProvider = GetComponent<PlayerStatusProvider> ();

		initialized = true;
	}

	void Start ()
	{
		PlayerStateController[] allPlayers = FindObjectsOfType<PlayerStateController> ();
		for (int i = 0; i < allPlayers.Length; ++i) {
			PlayerStateController player = allPlayers [i];
			if (player.playerId != playerId) {
				enemies.Add (player);
			}
		}

		// start in spawn state
		movementController.setMovementEnabled (false);
		spawn ();
	}

	//
	// PUBLIC
	//

	public bool IsSlashed() {
		return state == State.SLASHED;
	}

	public void setBondLink(BondLink bondLink) {
		isBond = bondLink != null;
	}

	public void onCollide(Collider2D source, Collider2D other) {
		debug ("onCollide");
		
		if (state != State.SLASH_ATTACK) {
			//debug ("not in SLASH_ATTACK state");
			return;
		}
		
		switch (aimDirection) {
		case AimDirection.UP:
			if(source != slashAttackColliderUp) {
				//debug ("source is up");
				return;
			}
			break;
		case AimDirection.DOWN:
			if(source != slashAttackColliderDown) {
				//debug ("source is down");
				return;
			}
			break;
		case AimDirection.FORWARD:
			if(source != slashAttackColliderForward) {
				//debug ("source is up");
				return;
			}
			break;
		}
		
		//debug ("collide");
		PlayerStateController enemy = other.GetComponentInParent<PlayerStateController> ();
		if (enemy != null) {
			//debug ("collide with enemy " + enemy.playerId);
			if (enemy.isPerformingSlashAttack () && isAimingOppositeDirection (enemy)) {
				enemy.knockback (aimDirection);
				knockback (enemy.aimDirection);
				//knockedBack = true;
				//knockedBackDirection = enemy.aimDirection;
			} else if (enemy.isSlashable ()) {
				enemy.hitWithSlash ();
			}
			return;
		}
		
		BondLink bondLink = other.GetComponentInParent<BondLink> ();
		if (bondLink != null) {
			LevelManager levelManager = FindObjectOfType<LevelManager>();
			levelManager.bondHasBeenSlashedBy(this);
		}
		
		debug ("WARNING unexpected collision");
	}

	//
	// STATE MACHINE
	//

	void Update ()
	{
		switch (state) {
		case State.SPAWN:
			updateSpawn ();
			break;
		case State.IDLE:
			updateIdle ();
			break;
		case State.SLASH_ATTACK:
			updateSlashAttack ();
			break;
		case State.KNOCKBACK:
			updateKnockback ();
			break;
		case State.SLASHED:
			updateSlashed ();
			break;
		case State.INVINCIBLE:
			updateInvincible ();
			break;
		}
	}

	private void updateSpawn ()
	{
		stateTime -= Time.deltaTime;
		if (stateTime <= 0.0f) {
			print ("p" + playerId + ": enter INVINCIBLE state");
			state = State.INVINCIBLE;
			stateTime = invincibleAfterSpawnTime;
		}
	}

	private void updateSlashed ()
	{
		stateTime -= Time.deltaTime;
		if (stateTime <= 0.0f) {
			spawn ();
		}
	}

	private void updateIdle ()
	{
		movementController.setMovementEnabled (true);

        //Aim direction vector based:
        float x = inputManager.AxisValue(playerId, InputManager.Horizontal);
        float y = inputManager.AxisValue(playerId, InputManager.Vertical);
        Vector2 v = new Vector2(x, y);                      //set a Vector2 based on controller tilt.
        float angleRadians = Mathf.Atan2(v.y, v.x);         //Get the angle of the vector (radian)
        float angleDegrees = angleRadians * Mathf.Rad2Deg;  //Convert to degree
        //angleDegrees will be in the range [-180,180], convert to [0,360] 'cause I'm human... Ho and 0 Degree = straight right.
        if (angleDegrees < 0)
            angleDegrees += 360;

        if (angleDegrees > 270 - (VerticalAimThresholdDegree / 2f) && angleDegrees < 270 + (VerticalAimThresholdDegree / 2f))
        { aimDirection = AimDirection.UP; }
        else if (angleDegrees > 90 - (VerticalAimThresholdDegree / 2f) && angleDegrees < 90 + (VerticalAimThresholdDegree / 2f))
        { aimDirection = AimDirection.DOWN; }
        else
        { aimDirection = AimDirection.FORWARD; }
        //if (angleDegrees != 0) //don't take "empty" pads into account.
        //    print("Joystick angle: " + angleDegrees + " Direction is: " + aimDirection); //DEBUG THIS SHIT NIGGA

		// slash attack
		if (inputManager.WasPressed (playerId, InputManager.BUTTON_ATTACK)) {
			if (slashAttackCooldown == 0.0f) {
				slash ();
			} else {
				print ("p" + playerId + ": slash attack on CD");
			}
		}

		// slash attack cooldown
		if (slashAttackCooldown > 0.0f) {
			slashAttackCooldown -= Time.deltaTime;
			if (slashAttackCooldown < 0.0f) {
				slashAttackCooldown = 0.0f;
			}
		}
	}
	
	private void updateSlashAttack ()
	{
		stateTime -= Time.deltaTime;
		if (stateTime <= 0.0f) {
			slashAttackCooldown = slashAttackCooldownTime;
			setIdleState ();
			return;
		}

		// dash

		float slashPct = 1.0f - stateTime / slashAttackTime;
		float dashVelocityPct = slashAttackDashCurve.Evaluate (slashPct);

		switch (aimDirection) {
		case AimDirection.UP:
			movementController.setVelocity (new Vector2(0.0f, dashVelocityPct * Time.deltaTime * slashAttackDashVelocityUp));
			break;
		case AimDirection.DOWN:
			movementController.setVelocity (new Vector2(0.0f, dashVelocityPct * Time.deltaTime * -slashAttackDashVelocityDown));
			break;
		case AimDirection.FORWARD:
			// TODO check if ground
			float velocity;
			if(airDash) {
				velocity = slashAttackDashVelocityForwardAir;
				if(movementController.isGrounded ()) {
					// cancel the air dash
					slashAttackCooldown = slashAttackCooldownTime;
					setIdleState ();
					return;
				}
			}
			else {
				velocity = slashAttackDashVelocityForwardGround;
			}

			if(!movementController.isFacingRight()) {
				velocity = -velocity;
			}
			movementController.setVelocity (new Vector2(dashVelocityPct * Time.deltaTime * velocity, -slashAttackHorizDashGravity));
			break;
		}
	}
	
	private void updateKnockback ()
	{
		stateTime -= Time.deltaTime;
		if (stateTime <= 0.0f) {
			print ("p" + playerId + ": enter IDLE state");
			state = State.IDLE;
			return;
		}

		float knockbackPct = 1.0f - stateTime / knockbackTime;
		float knockbackVelocityPct = knockbackCurve.Evaluate (knockbackPct);

		switch (knockbackDirection) {
		case AimDirection.UP:
			movementController.applyForce (new Vector2(0.0f, knockbackVelocityPct * Time.deltaTime * knockbackVelocityUp));
			break;
		case AimDirection.DOWN:
			movementController.applyForce (new Vector2(0.0f, knockbackVelocityPct * Time.deltaTime * -knockbackVelocityDown));
			break;
		case AimDirection.FORWARD:
			float velocity = movementController.isFacingRight() ? -knockbackVelocityForward : knockbackVelocityForward;
			movementController.applyForce (new Vector2(knockbackVelocityPct * Time.deltaTime * velocity, 0.0f));
			break;
		}
	}

	private void updateInvincible()
	{
		updateIdle ();

		stateTime -= Time.deltaTime;
		if (stateTime <= 0.0f) {
			print ("p" + playerId + ": enter IDLE state");
			state = State.IDLE;

			// TODO notification
		}
	}

	private void setIdleState() {
		print ("p" + playerId + ": enter IDLE state");
		state = State.IDLE;
		
		slashAttackColliderUp.enabled = false;
		slashAttackColliderDown.enabled = false;
		slashAttackColliderForward.enabled = false;
	}

	private void spawn()
	{
		setVisible(true);
		statusProvider.setRespawnWarning ();

		print ("p" + playerId + ": enter SPAWN state");
		state = State.SPAWN;
		stateTime = spawnTime;
	}

	private void slash()
	{
		print ("p" + playerId + ": enter SLASH_ATTACK state");
		state = State.SLASH_ATTACK;
		stateTime = slashAttackTime;
		
		movementController.setMovementEnabled(false);
		movementController.resetForces ();

		// notification
		switch(aimDirection) {
		case AimDirection.UP:
			slashAttackColliderUp.enabled = true;
			slashAttackColliderDown.enabled = false;
			slashAttackColliderForward.enabled = false;
			break;
		case AimDirection.DOWN:
			slashAttackColliderUp.enabled = false;
			slashAttackColliderDown.enabled = true;
			slashAttackColliderForward.enabled = false;
			break;
		case AimDirection.FORWARD:
			slashAttackColliderUp.enabled = false;
			slashAttackColliderDown.enabled = false;
			slashAttackColliderForward.enabled = true;
			break;
		}

		// notification
		switch(aimDirection) {
		case AimDirection.UP:
			statusProvider.setDashUp ();
			break;
		case AimDirection.DOWN:
			statusProvider.setDashDown ();
			break;
		case AimDirection.FORWARD:
			statusProvider.setDashForward ();
			break;
		}
	}
	
	private void knockback (AimDirection knockbackDirection)
	{
		print ("p" + playerId + ": enter KNOCKBACK state (dir=" + knockbackDirection + ")");
		state = State.KNOCKBACK;
		stateTime = knockbackTime;
		this.knockbackDirection = knockbackDirection;

		movementController.setMovementEnabled (false);
		movementController.resetForces ();

		slashAttackColliderUp.enabled = false;
		slashAttackColliderDown.enabled = false;
		slashAttackColliderForward.enabled = false;

		// notification
		switch(aimDirection) {
		case AimDirection.UP:
			statusProvider.setKnockBackUp ();
			break;
		case AimDirection.DOWN:
			statusProvider.setKnockBackDown ();
			break;
		case AimDirection.FORWARD:
			statusProvider.setKnockBackForward ();
			break;
		}
	}
	
	private void hitWithSlash ()
	{
		print ("p" + playerId + ": enter SLASHED state");
		state = State.SLASHED;
		stateTime = slashedTime;

		movementController.setMovementEnabled (false);
		movementController.resetForces ();

		setVisible (false);

		// notification
		statusProvider.setDie ();
	}

	//
	// GETTERS
	//

	public int GetPlayerId ()
	{
		return playerId;
	}

	//
	// HELPERS
	//

	private bool isPerformingSlashAttack ()
	{
		return state == State.SLASH_ATTACK;
	}

	private bool isSlashable() {
		// TODO blink for INVINCIBLE_AFTER_SLASHED
		return !isBond && state != State.KNOCKBACK && state != State.SLASHED; //&& state != State.INVINCIBLE_AFTER_SLASHED;
	}

	private bool isAimingOppositeDirection (PlayerStateController enemy)
	{
		if (aimDirection == AimDirection.UP && enemy.aimDirection == AimDirection.DOWN)
			return true;

		if (aimDirection == AimDirection.DOWN && enemy.aimDirection == AimDirection.UP)
			return true;

		if (aimDirection == AimDirection.FORWARD && enemy.aimDirection == AimDirection.FORWARD)
			return movementController.isFacingRight () != enemy.movementController.isFacingRight ();

		return false;
	}

	private void setVisible(bool visible) {
		foreach(SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>()) {
			sprite.enabled = visible;
		}
	}

	//
	// DEBUG DRAW
	//

	void debug(string text) {
		print ("p" + playerId + ": " + text);
	}

	public void OnDrawGizmos ()
	{
		if (!initialized) {
			return;
		}

		switch (state) {
		case State.IDLE:
			//drawColliderRect (bodyCollider, Color.white);
			break;
		case State.SLASH_ATTACK:
			drawColliderRect (bodyCollider, Color.red);
			switch(aimDirection) {
			case AimDirection.UP:
				drawRect (bodyCollider.transform.position.x, bodyCollider.transform.position.y + 1.0f, 0.6f, 0.6f, Color.red);
				break;
			case AimDirection.DOWN:
				drawRect (bodyCollider.transform.position.x, bodyCollider.transform.position.y - 1.0f, 0.6f, 0.6f, Color.red);
				break;
			case AimDirection.FORWARD:
				if(movementController.isFacingRight()) {
					drawRect (bodyCollider.transform.position.x + 1.0f, bodyCollider.transform.position.y, 0.6f, 0.6f, Color.red);
				}
				else {
					drawRect (bodyCollider.transform.position.x - 1.0f, bodyCollider.transform.position.y, 0.6f, 0.6f, Color.red);
				}
				break;
			}
			break;
		case State.KNOCKBACK:
			//drawColliderRect (bodyCollider, Color.green);
			break;
		case State.SLASHED:
			drawColliderRect (bodyCollider, Color.blue);
			break;
		case State.INVINCIBLE:
			drawColliderRect (bodyCollider, Color.yellow);
			break;
		}
	}

	private static void drawColliderRect (Collider2D collider, Color color)
	{
		float x = collider.transform.position.x + collider.offset.x;
		float y = collider.transform.position.y + collider.offset.y;
		Gizmos.color = color;
		Gizmos.DrawCube (new Vector3 (x, y, 0.0f), new Vector3 (((BoxCollider2D)collider).size.x, ((BoxCollider2D)collider).size.y, 0.0f));
	}

	private static void drawRect (float x, float y, float width, float height, Color color)
	{
		Gizmos.color = color;
		Gizmos.DrawCube (new Vector3 (x, y, 0.0f), new Vector3 (width, height, 0.0f));
	}
}

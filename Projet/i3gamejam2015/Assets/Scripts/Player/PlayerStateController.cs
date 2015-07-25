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
		IDLE,
		SLASH_ATTACK,
		KNOCKBACK,
		SLASHED
	}

	private State state = State.IDLE;
	private float stateTime = 0.0f;

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
	public float verticalAimThreshold = 0.85f;

	//
	// SLASH ATTACK
	// 

	public float slashAttackTime = 1.0f;
	public float slashAttackCooldownTime = 4.0f;
	public float slashAttackDashVelocityUp = 1000.0f;
	public float slashAttackDashVelocityDown = 1000.0f;
	public float slashAttackDashVelocityForward = 2000.0f;
	public AnimationCurve slashAttackDashCurve;

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

	public float slashedTime = 10.0f;

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
	}

	//
	// PUBLIC
	//

	public bool isSlashed() {
		return state == State.SLASHED;
	}

	public void setBondLink(BondLink link) {
	}

	//
	// STATE MACHINE
	//

	void Update ()
	{
		switch (state) {
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
		}
	}

	private void updateIdle ()
	{
		movementController.setMovementEnabled (true);

		// aim direction
		float v = inputManager.AxisValue (playerId, InputManager.Vertical);
		if (v <= -verticalAimThreshold) {
			aimDirection = AimDirection.UP;
		} else if (v >= verticalAimThreshold) {
			aimDirection = AimDirection.DOWN;
		} else {
			aimDirection = AimDirection.FORWARD;
		}

		//print (" v=" + v + " aim=" + aimDirection);

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
			print ("p" + playerId + ": enter IDLE state");
			state = State.IDLE;
			slashAttackCooldown = slashAttackCooldownTime;
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
			float force = movementController.isFacingRight() ? slashAttackDashVelocityForward : -slashAttackDashVelocityForward;
			movementController.setVelocity (new Vector2(dashVelocityPct * Time.deltaTime * force, 0.0f));
			break;
		}

		// collision

		bool knockedBack = false;
		AimDirection knockedBackDirection = AimDirection.FORWARD;

		for (int i = 0; i < enemies.Count; ++i) {
			PlayerStateController enemy = enemies [i];

			Collider2D collider = null;
			switch (aimDirection) {
			case AimDirection.UP:
				collider = slashAttackColliderUp;
				break;
			case AimDirection.DOWN:
				collider = slashAttackColliderDown;
				break;
			case AimDirection.FORWARD:
				collider = slashAttackColliderForward;
				break;
			}

			//print ("p" + playerId + ": testing " + collider + " against " + enemy.bodyCollider);
			if (collider.IsTouching (enemy.bodyCollider)) {

				print ("enemy.isAttack=" + enemy.isPerformingSlashAttack() + " isAimingOpposite=" + isAimingOppositeDirection (enemy));
				print ("dir=" + aimDirection + " enemy.dir=" + enemy.aimDirection);

				if (enemy.isPerformingSlashAttack () && isAimingOppositeDirection (enemy)) {
					enemy.knockback (aimDirection);
					knockedBack = true;
					knockedBackDirection = enemy.aimDirection;
				} else if (enemy.isSlashable ()) {
					enemy.hitWithSlash ();
				}
			}
		}
		
		if (knockedBack) {
			knockback (knockedBackDirection);
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

	private void updateSlashed ()
	{
		stateTime -= Time.deltaTime;
		if (stateTime <= 0.0f) {
			print ("p" + playerId + ": enter IDLE state");
			state = State.IDLE;

			// FIXME to be called a bit before...
			statusProvider.setRespawnWarning();
		}
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

		statusProvider.setDie ();
	}

	//
	// GETTERS
	//

	public int GetPlayerId ()
	{
		return playerId;
	}

	public bool IsSlashed() {
		return false;
	}

	//
	// HELPERS
	//

	private bool isPerformingSlashAttack ()
	{
		return state == State.SLASH_ATTACK;
	}

	private bool isSlashable() {
		return state != State.KNOCKBACK && state != State.SLASHED;
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
			drawColliderRect (bodyCollider, Color.white);
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
			drawColliderRect (bodyCollider, Color.green);
			break;
		case State.SLASHED:
			drawColliderRect (bodyCollider, Color.blue);
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

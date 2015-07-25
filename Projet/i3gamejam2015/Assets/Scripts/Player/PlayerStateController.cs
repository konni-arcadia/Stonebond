using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerStateController : MonoBehaviour
{
	public int playerId;

	private bool facingRight;
	private Collider2D bodyCollider;
	private InputManager inputManager;
	private PlayerMovementController movementController;
	private List<PlayerStateController> enemies = new List<PlayerStateController> ();
	
	// 
	// GLOBAL STATE
	//

	public enum State
	{
		IDLE,
		SLASH_ATTACK_WARNING,
		SLASH_ATTACK,
		KNOCKBACK,
		SLASHED
	}

	private State state = State.IDLE;
	private float stateTime = 0.0f;

	//
	// AIM DIRECTION
	//
	
	public enum AimDirection
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

	public float slashAttackWarningTime = 1.0f;
	public float slashAttackTime = 1.0f;
	public float slashAttackCooldownTime = 4.0f;
	public float slashAttackKnockbackTime = 2.0f;
	public float slashAttackSlashedTime = 10.0f;
	public float slashAttackDashForceUp = 1000.0f;
	public float slashAttackDashForceDown = 1000.0f;
	public float slashAttackDashForceForward = 2000.0f;
	public float slashAttackKnockbackForceUp = 2000.0f;
	public float slashAttackKnockbackForceDown = 2000.0f;
	public float slashAttackKnockbackForceForward = 3000.0f;

	public AnimationCurve slashAttackDashCurve;
	public AnimationCurve slashAttackKnockbackCurve;

	private float slashAttackCooldown;

	private Collider2D  slashAttackColliderForward;
	private Collider2D  slashAttackColliderUp;
	private Collider2D  slashAttackColliderDown;

	//
	// KNOCKBACK
	//

	private AimDirection knockbackDirection;

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
	// STATE MACHINE
	//

	void Update ()
	{
		switch (state) {
		case State.IDLE:
			updateIdle ();
			break;
		case State.SLASH_ATTACK_WARNING:
			updateSlashAttackWarning ();
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
				if(slashAttackWarningTime == 0.0f) {
					print ("p" + playerId + ": enter SLASH_ATTACK state");
					// don't loose a frame if warning is configured to 0
					state = State.SLASH_ATTACK;
					stateTime = slashAttackTime;
				}
				else {
					print ("p" + playerId + ": enter SLASH_ATTACK_WARNING state");
					state = State.SLASH_ATTACK_WARNING;
					stateTime = slashAttackWarningTime;
				}
				movementController.setMovementEnabled(false);
				movementController.resetForces ();

			} else {
				print ("p" + playerId + ": slash attack on CD");
			}
		}

		if (slashAttackCooldown > 0.0f) {
			slashAttackCooldown -= Time.deltaTime;
			if (slashAttackCooldown < 0.0f) {
				slashAttackCooldown = 0.0f;
			}
		}
	}
	
	private void updateSlashAttackWarning ()
	{
		stateTime -= Time.deltaTime;
		if (stateTime <= 0.0f) {
			print ("p" + playerId + ": enter SLASH_ATTACK state dir=" + aimDirection);
			state = State.SLASH_ATTACK;
			stateTime = slashAttackTime;
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

		float slashPct = 1.0f - stateTime / slashAttackTime;
		float dashForcePct = slashAttackDashCurve.Evaluate (slashPct);

		switch (aimDirection) {
		case AimDirection.UP:
			movementController.applyForce (new Vector2(0.0f, dashForcePct * Time.deltaTime * slashAttackDashForceUp));
			break;
		case AimDirection.DOWN:
			movementController.applyForce (new Vector2(0.0f, dashForcePct * Time.deltaTime * -slashAttackDashForceDown));
			break;
		case AimDirection.FORWARD:
			float force = movementController.isFacingRight() ? slashAttackDashForceForward : -slashAttackDashForceForward;
		//	print ("force=" + force);
			movementController.applyForce (new Vector2(dashForcePct * Time.deltaTime * force, 0.0f));
			break;
		}
		
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
				if (enemy.isPerformingSlashAttack () && isAimingOppositeDirection (enemy)) {
					enemy.knockback (aimDirection);
					knockedBack = true;
					knockedBackDirection = enemy.aimDirection;
				} else {
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

		float knockbackPct = 1.0f - stateTime / slashAttackKnockbackTime;
		float knockbackForcePct = slashAttackKnockbackCurve.Evaluate (knockbackPct);
		
		switch (knockbackDirection) {
		case AimDirection.UP:
			movementController.applyForce (new Vector2(0.0f, knockbackForcePct * Time.deltaTime * slashAttackDashForceUp));
			break;
		case AimDirection.DOWN:
			movementController.applyForce (new Vector2(0.0f, knockbackForcePct * Time.deltaTime * -slashAttackDashForceDown));
			break;
		case AimDirection.FORWARD:
			float force = movementController.isFacingRight() ? slashAttackDashForceForward : -slashAttackDashForceForward;
			movementController.applyForce (new Vector2(knockbackForcePct * Time.deltaTime * force, 0.0f));
			break;
		}
	}

	private void updateSlashed ()
	{
		stateTime -= Time.deltaTime;
		if (stateTime <= 0.0f) {
			print ("p" + playerId + ": enter IDLE state");
			state = State.IDLE;
		}
	}

	private void knockback (AimDirection knockbackDirection)
	{
		print ("p" + playerId + ": enter KNOCKBACK state");
		state = State.KNOCKBACK;
		stateTime = slashAttackKnockbackTime;
		this.knockbackDirection = knockbackDirection;

		movementController.setMovementEnabled (false);
		movementController.resetForces ();
	}

	private void hitWithSlash ()
	{
		print ("p" + playerId + ": enter SLASHED state");
		state = State.SLASHED;
		stateTime = slashAttackSlashedTime;

		movementController.setMovementEnabled (false);
		movementController.resetForces ();
	}

	//
	// GETTERS
	//

	// Returns whether the character is facing right
	public bool IsFacingRight ()
	{
		return facingRight;
	}

	public int GetPlayerId ()
	{
		return playerId;
	}

	// Meant to be used by the player movement controller only
	public void SetFacingRight (bool rightDirection)
	{
		facingRight = rightDirection;
	}

	//
	// HELPERS
	//

	private bool isPerformingSlashAttack ()
	{
		return state == State.SLASH_ATTACK || state == State.SLASH_ATTACK_WARNING;
	}

	private bool isAimingOppositeDirection (PlayerStateController enemy)
	{
		if (aimDirection == AimDirection.UP && enemy.aimDirection == AimDirection.DOWN)
			return true;

		if (aimDirection == AimDirection.DOWN && enemy.aimDirection == AimDirection.UP)
			return true;

		if (aimDirection == AimDirection.FORWARD && enemy.aimDirection == AimDirection.FORWARD)
			return IsFacingRight () != enemy.IsFacingRight ();

		return false;
	}

	//
	// DEBUG DRAW
	//

	public void OnDrawGizmos ()
	{
		if (!initialized) {
			return;
		}

		switch (state) {
		case State.IDLE:
			drawColliderRect (bodyCollider, Color.white);
			break;
		case State.SLASH_ATTACK_WARNING:
			drawColliderRect (bodyCollider, Color.yellow);
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

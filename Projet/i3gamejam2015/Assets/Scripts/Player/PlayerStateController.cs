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
		ATTACK,
		SPECIAL_ATTACK,
		KNOCKBACK,
		CRYSTALED,
		INVINCIBLE
	}

	private State state = State.SPAWN;
	private float stateTime = 0.0f;
	private bool isBond = false;
	private bool visible = true;

	//
	// COLLIDERS
	//

	private Collider2D bodyCollider;
	private Collider2D attackColliderForward;
	private Collider2D attackColliderUp;
	private Collider2D attackColliderDown;

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
    public float verticalAimThresholdDegree = 90.0f;

	//
	// SPAWN
	//

	public float initialSpawnTime = 1.6f;
	public float respawnTime = 1.3f;
	private bool initialSpawn = true;

	//
	// ATTACK
	// 

	public float attackUpTimeMin = 0.25f;
	public float attackUpTimeMax = 0.25f;
	public float attackUpForceMin = 0.0f;
	public float attackUpForceMax = 46000.0f;
	public AnimationCurve attackUpCurve;

	public float attackDownTimeMin = 0.25f;
	public float attackDownTimeMax = 0.25f;
	public float attackDownForceMin = 0.0f;
	public float attackDownForceMax = 46000.0f;
	public AnimationCurve attackDownCurve;

	public float attackForwardTimeMin = 0.2f;
	public float attackForwardTimeMax = 0.2f;
	public float attackForwardForceMin = 0.0f;
	public float attackForwardForceMax = 30000.0f;
	public AnimationCurve attackForwardCurve;

	public float attackCooldownTime = 0.3f;

	private float attackCooldown;

	//
	// SPECIAL ATTACK
	// 

	public float specialAttackVelocity = 2000.0f;

	//
	// KNOCKBACK
	//

	public float knockbackTime = 2.0f;
	public float knockbackForceUp = 20000.0f;
	public float knockbackForceDown = 20000.0f;
	public float knockbackForceForward = 30000.0f;
	public AnimationCurve knockbackCurve;

	private AimDirection knockbackDirection;

	//
	// CRYSTALED
	//

	public float crystaledTime = 2.0f;
	private GameObject crystal = null;

	//
	// INVINSIBLE
	//

	public float invincibleAfterSpawnTime = 5.0f;
	public float invinsibleBlinkInterval = 0.2f;
	private float invisibleBlinkCounter = 0.0f;

	//
	// INIT
	//

	private bool awake = false;

	void Awake ()
	{
		print("p" + playerId + ": awake");

		bodyCollider = transform.Find ("bodyCollider").GetComponent<Collider2D> ();

		attackColliderForward = transform.Find ("attackColliderForward").GetComponent<Collider2D> ();
		attackColliderUp = transform.Find ("attackColliderUp").GetComponent<Collider2D> ();
		attackColliderDown = transform.Find ("attackColliderDown").GetComponent<Collider2D> ();
		attackColliderUp.enabled = false;
		attackColliderDown.enabled = false;
		attackColliderForward.enabled = false;

		inputManager = FindObjectOfType<InputManager>();
		movementController = GetComponent<PlayerMovementController>();
		statusProvider = GetComponent<PlayerStatusProvider> ();

		awake = true;
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
		movementController.setJumpEnabled (false);
		spawn ();
	}

	//
	// PUBLIC
	//

	public bool IsCrystaled() {
		return state == State.CRYSTALED || state == State.SPAWN;
	}

	public int GetPlayerId ()
	{
		return playerId;
	}

	public float GetAttackCooldownPct() {
		return attackCooldown / attackCooldownTime;
	}

	public float GetAttackPct() {
		if (state != State.ATTACK) {
			return 0.0f;
		}

		switch (aimDirection) {
		case AimDirection.UP:
			return stateTime / attackUpTimeMax;
		case AimDirection.DOWN:
			return stateTime / attackDownTimeMax;
		case AimDirection.FORWARD:
			return stateTime / attackForwardTimeMax;
		default:
			return 0.0f;
		}
	}

	public void setBondLink(BondLink bondLink) {
		isBond = bondLink != null;
		statusProvider.setBoundStatus (bondLink != null);
	}

	public void onCollide(Collider2D source, Collider2D other) {
		//debug ("onCollide");
		
		if (state != State.ATTACK) {
			//debug ("not in ATTACK state");
			return;
		}
		
		switch (aimDirection) {
		case AimDirection.UP:
			if(source != attackColliderUp) {
				//debug ("source is up");
				return;
			}
			break;
		case AimDirection.DOWN:
			if(source != attackColliderDown) {
				//debug ("source is down");
				return;
			}
			break;
		case AimDirection.FORWARD:
			if(source != attackColliderForward) {
				//debug ("source is up");
				return;
			}
			break;
		}
		
		//debug ("collide");
		PlayerStateController enemy = other.GetComponentInParent<PlayerStateController> ();
		if (enemy != null) {
			//debug ("collide with enemy " + enemy.playerId);
			if (enemy.isPerformingAttack () && isAimingOppositeDirection (enemy)) {
				enemy.knockback (aimDirection);
				knockback (enemy.aimDirection);
			} else if (enemy.isAttackable ()) {
				enemy.hitWithAttack (aimDirection);
			}
			return;
		}
		
		BondLink bondLink = other.GetComponentInParent<BondLink> ();
		if (bondLink != null) {
			LevelManager levelManager = FindObjectOfType<LevelManager>();
			levelManager.bondHasBeenBrokenBy(this);
			return;
		}
		
		Debug.LogWarning ("p" + playerId + ": unexpected collision", this);
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
		case State.ATTACK:
			updateAttack ();
			break;
		case State.SPECIAL_ATTACK:
			updateSpecialAttack ();
			break;
		case State.KNOCKBACK:
			updateKnockback ();
			break;
		case State.CRYSTALED:
			updateCrystaled ();
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
			movementController.setMovementEnabled (true);
			movementController.setJumpEnabled (true);

			if(initialSpawn) {
				print ("p" + playerId + ": enter IDLE state");
				state = State.IDLE;
				initialSpawn = false;
			}
			else {
				print ("p" + playerId + ": enter INVINCIBLE state");
				state = State.INVINCIBLE;
				invisibleBlinkCounter = invinsibleBlinkInterval;
				stateTime = invincibleAfterSpawnTime;
				statusProvider.setInvincibleStatus(true);
			}
		}
	}

	private void updateCrystaled ()
	{
		stateTime -= Time.deltaTime;
		if (stateTime <= 0.0f) {
			spawn ();
		}
	}

	private void updateIdle ()
	{		        
        float aimX = inputManager.AxisValue(playerId, InputManager.Horizontal);
        float aimY = inputManager.AxisValue(playerId, InputManager.Vertical);        
		float aimAngle = Mathf.Atan2(aimY, aimX) * Mathf.Rad2Deg;          
		if (aimAngle < 0.0f) {
			aimAngle += 360.0f;
		}

		if (aimAngle > 270.0f - (verticalAimThresholdDegree / 2.0f) && aimAngle < 270.0f + (verticalAimThresholdDegree / 2.0f)) {
			aimDirection = AimDirection.UP;
		}
		else if (aimAngle > 90.0f - (verticalAimThresholdDegree / 2.0f) && aimAngle < 90.0f + (verticalAimThresholdDegree / 2.0f)) {
			aimDirection = AimDirection.DOWN;
		}
        else {
			aimDirection = AimDirection.FORWARD;
		}
		
		// attack
		if (inputManager.WasPressed (playerId, InputManager.BUTTON_ATTACK)) {
			if (attackCooldown == 0.0f) {
				attack ();
			} else {
				print ("p" + playerId + ": attack on CD");
				statusProvider.setAttackFailed();
			}
		}

		// attack cooldown
		if (attackCooldown > 0.0f) {
			attackCooldown -= Time.deltaTime;
			if (attackCooldown < 0.0f) {
				attackCooldown = 0.0f;
			}
		}
	}
	
	private void updateAttack ()
	{
		float attackMinTime = 0.0f;
		float attackMaxTime = 0.0f;

		switch (aimDirection) {
		case AimDirection.UP:
			attackMinTime = attackUpTimeMin;
			attackMaxTime = attackUpTimeMax;
			break;
		case AimDirection.DOWN:
			attackMinTime = attackDownTimeMin;
			attackMaxTime = attackDownTimeMax;
			break;
		case AimDirection.FORWARD:
			attackMinTime = attackForwardTimeMin;
			attackMaxTime = attackForwardTimeMax;
			break;
		}

		stateTime += Time.deltaTime;
	
		// if attack time is over
		if(stateTime > attackMaxTime || (stateTime > attackMinTime && !inputManager.IsHeld(playerId, InputManager.BUTTON_ATTACK))) {
			print ("p" + playerId + ": enter IDLE state");
			state = State.IDLE;
						
			switch (aimDirection) {
			case AimDirection.UP:
				attackColliderUp.enabled = false;
				break;
			case AimDirection.DOWN:
				attackColliderDown.enabled = false;
				break;
			case AimDirection.FORWARD:
				movementController.setGravityEnabled (true);
				movementController.setJumpEnabled (true);
				attackColliderForward.enabled = false;
				break;
			}

			attackCooldown = attackCooldownTime;
			return;
		}

		float attackPct = stateTime / attackMaxTime;

		switch (aimDirection) {
		case AimDirection.UP:
			{
				float force = (attackUpForceMin + attackUpCurve.Evaluate (attackPct) * (attackUpForceMax - attackUpForceMin)) * Time.deltaTime;
				movementController.applyForce (new Vector2 (0.0f, force));
				break;
			}
		case AimDirection.DOWN:
			{
				float force = (attackDownForceMin + attackDownCurve.Evaluate (attackPct) * (attackDownForceMax - attackDownForceMin)) * Time.deltaTime;
				movementController.applyForce (new Vector2 (0.0f, -force));
				break;
			}
		case AimDirection.FORWARD:
			{
				float force = (attackForwardForceMin + attackForwardCurve.Evaluate (attackPct) * (attackForwardForceMax - attackForwardForceMin)) * Time.deltaTime;
				movementController.applyForce (new Vector2 (movementController.isFacingRight () ? force : -force, 0.0f));
				break;
			}
		}
	}

	private void updateSpecialAttack () {
		//float velocity = specialAttackVelocity;
		//if(!movementController.isFacingRight()) {
		//	velocity = -velocity;
		//}
	}
	
	private void updateKnockback ()
	{
		stateTime -= Time.deltaTime;
		if (stateTime <= 0.0f) {
			print ("p" + playerId + ": enter IDLE state");
			state = State.IDLE;
			
			movementController.setMovementEnabled (true);
			movementController.setJumpEnabled (true);
			movementController.setGravityEnabled (true);
			return;
		}

		float knockbackPct = 1.0f - stateTime / knockbackTime;
		float knockbackVelocityPct = knockbackCurve.Evaluate (knockbackPct);

		switch (knockbackDirection) {
		case AimDirection.UP:
			movementController.applyForce (new Vector2(0.0f, knockbackVelocityPct * Time.deltaTime * knockbackForceUp));
			break;
		case AimDirection.DOWN:
			movementController.applyForce (new Vector2(0.0f, knockbackVelocityPct * Time.deltaTime * -knockbackForceDown));
			break;
		case AimDirection.FORWARD:
			float velocity = movementController.isFacingRight() ? -knockbackForceForward : knockbackForceForward;
			movementController.applyForce (new Vector2(knockbackVelocityPct * Time.deltaTime * velocity, 0.0f));
			break;
		}
	}

	private void updateInvincible()
	{
		updateIdle ();

		stateTime -= Time.deltaTime;
		if (stateTime <= 0.0f) {
			statusProvider.setInvincibleStatus(false);
			setVisible (true);

			print ("p" + playerId + ": enter IDLE state");
			state = State.IDLE;
			return;
		}

		invisibleBlinkCounter -= Time.deltaTime;
		if (invisibleBlinkCounter < 0.0f) {
			invisibleBlinkCounter += invinsibleBlinkInterval;
			setVisible (!visible);
		}
	}

	private void spawn()
	{
		print ("p" + playerId + ": enter SPAWN state");
		state = State.SPAWN;
		stateTime = initialSpawn ? initialSpawnTime : respawnTime;

		statusProvider.setRespawn (initialSpawn);
	}

	private void attack()
	{
		print ("p" + playerId + ": enter ATTACK state");
		state = State.ATTACK;
		stateTime = 0.0f;

		movementController.resetForces ();
		switch(aimDirection) {
		case AimDirection.UP:
			attackColliderUp.enabled = true;
			attackColliderDown.enabled = false;
			attackColliderForward.enabled = false;
			statusProvider.setAttackUp ();
			break;
		case AimDirection.DOWN:
			attackColliderUp.enabled = false;
			attackColliderDown.enabled = true;
			attackColliderForward.enabled = false;
			statusProvider.setAttackDown ();
			break;
		case AimDirection.FORWARD:
			movementController.setJumpEnabled (false);
			movementController.setGravityEnabled (false);
			attackColliderUp.enabled = false;
			attackColliderDown.enabled = false;
			attackColliderForward.enabled = true;
			statusProvider.setAttackForward ();
			break;
		}
	}

	private void specialAttack()
	{
		print ("p" + playerId + ": enter SPECIAL_ATTACK state");
		state = State.SPECIAL_ATTACK;

		// NOT IMPLEMENTED
	}
	
	private void knockback (AimDirection knockbackDirection)
	{
		print ("p" + playerId + ": enter KNOCKBACK state (dir=" + knockbackDirection + ")");
		state = State.KNOCKBACK;
		stateTime = knockbackTime;
		this.knockbackDirection = knockbackDirection;

		movementController.setMovementEnabled (false);
		movementController.setJumpEnabled (false);
		movementController.setGravityEnabled (false);

		attackColliderUp.enabled = false;
		attackColliderDown.enabled = false;
		attackColliderForward.enabled = false;

		if(aimDirection == AimDirection.FORWARD) {
			statusProvider.setHorizontalKnockback();
		}
		else {
			statusProvider.setVerticalKnockback();
		}	
	}
	
	private void hitWithAttack (AimDirection dir)
	{
		print ("p" + playerId + ": enter CRYSTALED state");
		state = State.CRYSTALED;
		stateTime = crystaledTime;

		movementController.setMovementEnabled (false);
		movementController.setJumpEnabled (false);
		movementController.setGravityEnabled (true);
	
		switch(aimDirection) {
		case AimDirection.UP:
			statusProvider.setDie (new Vector2(0.0f, 1.0f));
			break;
		case AimDirection.DOWN:
			statusProvider.setDie (new Vector2(0.0f, -1.0f));
			break;
		case AimDirection.FORWARD:		
			statusProvider.setDie (new Vector2(movementController.isFacingRight() ? 1.0f : -1.0f, 0.0f));
			break;
		}
	}

	//
	// HELPERS
	//

	private bool isPerformingAttack ()
	{
		return state == State.ATTACK;
	}

	private bool isAttackable() {
		if (isBond) {
			return false;
		}
	
		return state == State.IDLE;
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
		if (this.visible != visible) {
			foreach (SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>()) {
				sprite.enabled = visible;
			}
			this.visible = visible;
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
		if (!awake) {
			return;
		}

		switch (state) {
		case State.IDLE:
			drawColliderRect (bodyCollider, Color.white);
			break;
		case State.ATTACK:
			drawColliderRect (bodyCollider, Color.white);
			switch(aimDirection) {
			case AimDirection.UP:
				drawColliderRect (attackColliderUp, Color.red);
				break;
			case AimDirection.DOWN:
				drawColliderRect (attackColliderDown, Color.red);
				break;
			case AimDirection.FORWARD:
				drawColliderRect (attackColliderForward, Color.red);
				break;
			}
			break;
		case State.KNOCKBACK:
			drawColliderRect (bodyCollider, Color.white);
			break;
		case State.CRYSTALED:
			drawColliderRect (bodyCollider, Color.gray);
			break;
		case State.INVINCIBLE:
			drawColliderRect (bodyCollider, Color.gray);
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

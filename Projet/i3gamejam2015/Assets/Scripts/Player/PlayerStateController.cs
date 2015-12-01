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
    public float verticalAimThresholdDegree = 90f;

	//
	// SPAWN
	//

	public float initialSpawnTime = 1.6f;
	public float respawnTime = 1.3f;
	private bool initialSpawn = true;

	//
	// ATTACK
	// 

	public float attackUpMinTime = 0.35f;
	public float attackUpMaxTime = 0.35f;
	public float attackDownMinTime = 0.35f;
	public float attackDownMaxTime = 0.35f;
	public float attackForwardMinTime = 0.35f;
	public float attackForwardMaxTime = 0.35f;

	public float attackCooldownTime = 4.0f;
	public float attackVelocityUp = 1000.0f;
	public float attackVelocityDown = 1000.0f;
	public float attackVelocityForward = 2000.0f;
	public float attackHorizGravity = 20.0f;
	public AnimationCurve attackCurve;
	public float attackUpHorizControl = 0.5f;
	public float attackDownHorizControl = 0.5f;

	//
	// SPECIAL ATTACK
	// 

	public float specialAttackVelocity = 2000.0f;

	//
	// KNOCKBACK
	//

	public float knockbackTime = 2.0f;
	public float knockbackVelocityUp = 2000.0f;
	public float knockbackVelocityDown = 2000.0f;
	public float knockbackVelocityForward = 3000.0f;
	public AnimationCurve knockbackCurve;

	private float attackCooldown;
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
		movementController.setFrictionEnabled (true);
		spawn ();

		// FIXME move this somewhere else, doesn't belong to player logic...
		SpriteRenderer bodyRenderer = transform.Find ("CharacterSprites").Find ("Body").GetComponent<SpriteRenderer> ();
		bodyRenderer.material.SetColor("_ChromaTexColor", PlayerInfo.Color);
		bodyRenderer.material.SetColor("_Color", Color.Lerp(Color.white, PlayerInfo.Color, 0.3f));
	}

	//
	// PUBLIC
	//

	public bool IsCrystaled() {
		return state == State.CRYSTALED || state == State.SPAWN;
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
		// DIRTY make sure sprite is visible if went out of invinsible
		if (state != State.INVINCIBLE) {
			setVisible (true);
		}

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
			if(initialSpawn) {
				// prout
				print ("p" + playerId + ": enter IDLE state");
				state = State.IDLE;

				initialSpawn = false;
			}
			else {
				print ("p" + playerId + ": enter INVINCIBLE state");
				state = State.INVINCIBLE;
				invisibleBlinkCounter = invinsibleBlinkInterval;
				stateTime = invincibleAfterSpawnTime;
				movementController.setMovementEnabled (true);
				movementController.setFrictionEnabled (true);
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
		movementController.setMovementEnabled (true);
		movementController.setFrictionEnabled (true);
		        
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
			attackMinTime = attackUpMinTime;
			attackMaxTime = attackUpMaxTime;
			break;
		case AimDirection.DOWN:
			attackMinTime = attackDownMinTime;
			attackMaxTime = attackDownMaxTime;
			break;
		case AimDirection.FORWARD:
			attackMinTime = attackForwardMinTime;
			attackMaxTime = attackForwardMaxTime;
			break;
		}

		stateTime += Time.deltaTime;

		if (stateTime > attackMaxTime) {
			attackCooldown = attackCooldownTime;
			setIdleState ();
			return;
		}

		if(stateTime > attackMinTime && !inputManager.IsHeld(playerId, InputManager.BUTTON_ATTACK)) {
			attackCooldown = attackCooldownTime;
			setIdleState ();
			return;
		}

		float attackPct = stateTime / attackMaxTime;
		float attackVelocityPct = attackCurve.Evaluate (attackPct);

		switch (aimDirection) {
		case AimDirection.UP: {
			float vx = inputManager.AxisValue (playerId, InputManager.Horizontal) * attackUpHorizControl;
			float vy = attackVelocityPct * Time.deltaTime * attackVelocityUp;
			movementController.setVelocity (new Vector2(vx, vy));
			break;
		}
		case AimDirection.DOWN: {
			float vx = inputManager.AxisValue (playerId, InputManager.Horizontal) * attackDownHorizControl;
			float vy = attackVelocityPct * Time.deltaTime * -attackVelocityDown;
			movementController.setVelocity (new Vector2(vx, vy));
			break;
		}
		case AimDirection.FORWARD:
			float velocity = attackVelocityForward;
			if(!movementController.isFacingRight()) {
				velocity = -velocity;
			}
			movementController.setVelocity (new Vector2(attackVelocityPct * Time.deltaTime * velocity, -attackHorizGravity));
			break;
		}
	}

	private void updateSpecialAttack () {
		float velocity = specialAttackVelocity;
		if(!movementController.isFacingRight()) {
			velocity = -velocity;
		}
		movementController.setVelocity (new Vector2(Time.deltaTime * velocity, 0.0f));
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
		float knockbackForcePct = knockbackCurve.Evaluate (knockbackPct);

		switch (knockbackDirection) {
		case AimDirection.UP:
			movementController.applyForce (new Vector2(0.0f, knockbackForcePct * Time.deltaTime * knockbackVelocityUp));
			break;
		case AimDirection.DOWN:
			movementController.applyForce (new Vector2(0.0f, knockbackForcePct * Time.deltaTime * -knockbackVelocityDown));
			break;
		case AimDirection.FORWARD:
			float velocity = movementController.isFacingRight() ? -knockbackVelocityForward : knockbackVelocityForward;
			movementController.applyForce (new Vector2(knockbackForcePct * Time.deltaTime * velocity, 0.0f));
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

			statusProvider.setInvincibleStatus(false);
			setVisible (true);
			return;
		}

		invisibleBlinkCounter -= Time.deltaTime;
		if (invisibleBlinkCounter < 0.0f) {
			invisibleBlinkCounter += invinsibleBlinkInterval;
			setVisible (!visible);
		}
	}

	private void setIdleState() {
		print ("p" + playerId + ": enter IDLE state");
		state = State.IDLE;
		
		attackColliderUp.enabled = false;
		attackColliderDown.enabled = false;
		attackColliderForward.enabled = false;
	}

	private void spawn()
	{
		// FIXME have to events: initalSpawn and respawn
		statusProvider.setRespawnWarning ();

		print ("p" + playerId + ": enter SPAWN state");
		state = State.SPAWN;
		stateTime = initialSpawn ? initialSpawnTime : respawnTime;
	}

	private void attack()
	{
		print ("p" + playerId + ": enter ATTACK state");
		state = State.ATTACK;
		stateTime = 0.0f;
		
		movementController.setMovementEnabled (false);
		movementController.setFrictionEnabled (false);
		movementController.resetForces ();
		
		switch(aimDirection) {
		case AimDirection.UP:
			attackColliderUp.enabled = true;
			attackColliderDown.enabled = false;
			attackColliderForward.enabled = false;
			break;
		case AimDirection.DOWN:
			attackColliderUp.enabled = false;
			attackColliderDown.enabled = true;
			attackColliderForward.enabled = false;
			break;
		case AimDirection.FORWARD:
			attackColliderUp.enabled = false;
			attackColliderDown.enabled = false;
			attackColliderForward.enabled = true;
			break;
		}

		// notification
		switch(aimDirection) {
		case AimDirection.UP:
			statusProvider.setAttackUp ();
			break;
		case AimDirection.DOWN:
			statusProvider.setAttackDown ();
			break;
		case AimDirection.FORWARD:
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
		movementController.setFrictionEnabled (false);
		movementController.resetForces ();

		attackColliderUp.enabled = false;
		attackColliderDown.enabled = false;
		attackColliderForward.enabled = false;

		if(aimDirection == AimDirection.FORWARD) {
			statusProvider.setHorizontalKnockback();
			// TODO move to effect manager
			ScreenShake.shake (0.5f, 0.0f, 2.0f, 0.0f);
		}
		else {
			statusProvider.setVerticalKnockback();
			// TODO move to effect manager
			ScreenShake.shake (0.0f, 0.5f, 0.0f, 2.0f);
		}	
	}
	
	private void hitWithAttack (AimDirection dir)
	{
		print ("p" + playerId + ": enter CRYSTALED state");
		state = State.CRYSTALED;
		stateTime = crystaledTime;

		movementController.setMovementEnabled (false);
		movementController.setFrictionEnabled (true);
		movementController.resetForces ();
	
		// notification
		statusProvider.setDie();
		// TODO use player effect script
		Flash.flash ();
		SlowMotion.slowMotion ();
		ScreenShake.shake (0.5f, 0.5f, 2.0f, 2.0f);
	}

	//
	// GETTERS
	//

	public int GetPlayerId ()
	{
		return playerId;
	}

	private GameState.PlayerInfo PlayerInfo {
		get { return GameState.Instance.Player(playerId); }
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
			//drawColliderRect (bodyCollider, Color.white);
			break;
		case State.ATTACK:
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
		case State.CRYSTALED:
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

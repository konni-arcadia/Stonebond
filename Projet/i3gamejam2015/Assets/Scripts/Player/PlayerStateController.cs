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
    public float VerticalAimThresholdDegree = 90f;

	//
	// SPAWN
	//

	public float initialSpawnTime = 1.6f;
	public float respawnTime = 1.3f;
	private bool initialSpawn = true;

	//
	// ATTACK
	// 

	public float attackTime = 1.0f;
	public float attackCooldownTime = 4.0f;
	public float attackVelocityUp = 1000.0f;
	public float attackVelocityDown = 1000.0f;
	public float attackVelocityForward = 2000.0f;
	public float attackHorizGravity = 20.0f;
	public AnimationCurve attackCurve;
	public float attackUpHorizControl = 0.5f;
	public float attackDownHorizControl = 0.5f;

	//private bool airDash = false;

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
		print ("Awake player " + playerId);

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

				if(aimDirection == AimDirection.FORWARD) {
					MyLittlePoney.shake (0.5f, 0.0f, 2.0f, 0.0f);
				}
				else {
					MyLittlePoney.shake (0.0f, 0.5f, 0.0f, 2.0f);
				}
			} else if (enemy.isAttackable ()) {
				enemy.hitWithAttack (aimDirection);

				MyLittlePoney.shake (0.5f, 0.5f, 2.0f, 2.0f);
			}
			return;
		}
		
		BondLink bondLink = other.GetComponentInParent<BondLink> ();
		if (bondLink != null) {
			LevelManager levelManager = FindObjectOfType<LevelManager>();
			levelManager.bondHasBeenBrokenBy(this);
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
			setVisible (true); // dirty make sure sprite is visible if went out of invinsible
			updateSpawn ();
			break;
		case State.IDLE:
			setVisible (true); // dirty make sure sprite is visible if went out of invinsible
			updateIdle ();
			break;
		case State.ATTACK:
			setVisible (true); // dirty make sure sprite is visible if went out of invinsible
			updateAttack ();
			break;
		case State.KNOCKBACK:
			setVisible (true); // dirty make sure sprite is visible if went out of invinsible
			updateKnockback ();
			break;
		case State.CRYSTALED:
			setVisible (true); // dirty make sure sprite is visible if went out of invinsible
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
				//statusProvider.setInvincibleStatus(true);
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

		// attack
		if (inputManager.WasPressed (playerId, InputManager.BUTTON_ATTACK)) {
			if (attackCooldown == 0.0f) {
				attack ();
			} else {
				print ("p" + playerId + ": attack on CD");
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
		stateTime -= Time.deltaTime;
		if (stateTime <= 0.0f) {
			attackCooldown = attackCooldownTime;
			setIdleState ();
			return;
		}

		float attackPct = 1.0f - stateTime / attackTime;
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

			//statusProvider.setInvincibleStatus(false);
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
		stateTime = attackTime;
		
		movementController.setMovementEnabled (false);
		movementController.setFrictionEnabled (false);
		movementController.resetForces ();

		// notification
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
		movementController.setFrictionEnabled (false);
		movementController.resetForces ();

		attackColliderUp.enabled = false;
		attackColliderDown.enabled = false;
		attackColliderForward.enabled = false;

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
	
	private void hitWithAttack (AimDirection dir)
	{
		print ("p" + playerId + ": enter CRYSTALED state");
		state = State.CRYSTALED;
		stateTime = crystaledTime;

		movementController.setMovementEnabled (false);
		movementController.setFrictionEnabled (true);
		movementController.resetForces ();
	
		// notification
		statusProvider.setDie(); // TODO remove
		Vector2 directionVector = new Vector3 ();
		switch (dir) {
		case AimDirection.UP:
			directionVector.Set(0.0f, 1.0f);
			break;
		case AimDirection.DOWN:
			directionVector.Set(0.0f, -1.0f);
			break;
		case AimDirection.FORWARD:
			directionVector.Set(movementController.isFacingRight() ? 1.0f : -1.0f, 0.0f);
			break;
		}
		// TODO statusProvider.setDie (transform.position, directionVector);

		Flash.flash ();
		MyLittlePoney.slowMotion ();
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

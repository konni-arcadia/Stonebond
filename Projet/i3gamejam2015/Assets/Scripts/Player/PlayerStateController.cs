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
    private List<PlayerStateController> enemies = new List<PlayerStateController>();
    
	private Shield shieldAnimation; // TODO use events and remove

    // 
    // GLOBAL STATE
    //

    public int playerId;

    private enum State
    {
        NONE,
        IDLE,
        ATTACK,
        CHARGE,
        SPECIAL_ATTACK,
        KNOCKBACK,
        CRYSTALED,
        INVINCIBLE
    }

    private State state = State.NONE;
    private float stateElapsedTime;
    private float stateElapsedFixedTime;
	private bool hasShield = false;
    private bool visible = true;
    private bool onGround = false;
    private bool onWall = false;

    public Collider2D bodyCollider;

    public enum AimDirection
    {
        UP,
        DOWN,
        FORWARD
    }

    private AimDirection aimDirection = AimDirection.FORWARD;
    public float verticalAimThresholdDegree = 90.0f;
    private float attackCooldown = 0.0f;

    //
    // ATTACK
    // 

    public float attackUpTimeMin = 0.25f;
    public float attackUpTimeMax = 0.25f;
    public float attackUpForceMin = 0.0f;
    public float attackUpForceMax = 46000.0f;
	[Range(0.0f, 1.0f)]
    public float attackUpHitboxStart = 0.0f;
	[Range(0.0f, 1.0f)]
    public float attackUpHitboxEnd = 0.9f;
    public AnimationCurve attackUpCurve;
    public Collider2D attackUpCollider;

    public float attackDownTimeMin = 0.25f;
    public float attackDownTimeMax = 0.25f;
    public float attackDownForceMin = 0.0f;
    public float attackDownForceMax = 46000.0f;
	[Range(0.0f, 1.0f)]
    public float attackDownHitboxStart = 0.0f;
	[Range(0.0f, 1.0f)]
    public float attackDownHitboxEnd = 1.0f;
    public float attackDownCancelTimeMin = 0.12f;
    public AnimationCurve attackDownCurve;
    public Collider2D attackDownCollider;

    public float attackForwardTimeMin = 0.2f;
    public float attackForwardTimeMax = 0.2f;
    public float attackForwardForceMin = 0.0f;
    public float attackForwardForceMax = 30000.0f;
	[Range(0.0f, 1.0f)]
    public float attackForwardHitboxStart = 0.0f;
	[Range(0.0f, 1.0f)]
    public float attackForwardHitboxEnd = 0.6f;
    public AnimationCurve attackForwardCurve;
    public Collider2D attackForwardCollider;
    public float attackCooldownTime = 0.3f;

    // 
    // CHARGE
    //
	    
	public float chargeGravityReductionTime = 0.08f;
	public float chargeReadyTime = 0.5f;
    public float chargeFullTime = 1.6f;
	public float chargeOverflowTime = 2.0f;
    public float chargeInputCancelTimeMin = 0.2f;
    public AnimationCurve chargeReadyForceRatioCurve;

    //
    // SPECIAL ATTACK
    // 

    public bool specialAttackEnabled = true;
    public float specialAttackTime = 0.35f;
    public float specialAttackCancelTimeMin = 0.25f;
    public float specialAttackForceMin = 0.0f;
    public float specialAttackForceMax = 30000.0f;
	[Range(0.0f, 1.0f)]
    public float specialAttackHitboxStart = 0.0f;
	[Range(0.0f, 1.0f)]
    public float specialAttackHitboxEnd = 0.95f;
    public AnimationCurve specialAttackCurve;
    public Collider2D specialAttackCollider;
    public Transform specialAttackBase;
    public Transform[] specialAttackSolidChecks;

    //
    // KNOCKBACK
    //

    public float knockbackTime = 2.0f;
    public float knockbackForceUp = 20000.0f;
    public float knockbackForceDown = 20000.0f;
    public float knockbackForceForward = 30000.0f;
    public AnimationCurve knockbackCurve;

    //
    // CRYSTALED
    //

    public float diePauseTime = 0.2f;
    public float respawnCrystaledTime = 3.2f;
    public float initialCrystaledTime = 3.8f;
    public float respawnWarningTime = 1.2f;

    //
    // INVINSIBLE
    //

    public float invincibleAfterSpawnTime = 5.0f;
    public float invinsibleBlinkInterval = 0.2f;

    //
    // INIT
    //

    private bool awake = false;

    void Awake()
    {
        //print("p" + playerId + ": awake");

        attackUpCollider.enabled = false;
        attackDownCollider.enabled = false;
		attackForwardCollider.enabled = false;
        specialAttackCollider.enabled = false;

		shieldAnimation = GetComponentInChildren<Shield> ();
		if (shieldAnimation != null) {
			shieldAnimation.gameObject.SetActive (false);
		}

		inputManager = FindObjectOfType<InputManager>();
        movementController = GetComponent<PlayerMovementController>();
        statusProvider = GetComponent<PlayerStatusProvider>();

		statusProvider.OnGroundedStatusChanged += HandleOnGroundedStatusChanged;
        statusProvider.OnOnWallStatusChanged += HandleOnOnWallStatusChanged;
        awake = true;
    }
  
    void Start()
    {
        PlayerStateController[] allPlayers = FindObjectsOfType<PlayerStateController>();
        for (int i = 0; i < allPlayers.Length; ++i)
        {
            PlayerStateController player = allPlayers [i];
            if (player.playerId != playerId)
            {
                enemies.Add(player);
            }
        }

		OnPlayerIdHasBeenSet ();

        // start in spawn state
        SetCrystaledState(initialCrystaledTime);
    }

	// This method must be called after the playerId has been set.
	// It is called automatically by Start(), so if the playerId has been set in Unity Editor,
	//    you don't have to call this method.
	// But if you create the PlayerStateController programmatically, (which means you have to set
	//    the playerId programmatically right after), then you have to call this method afterwards.
	public void OnPlayerIdHasBeenSet() {
		
		if (shieldAnimation != null) {
			shieldAnimation.SetPlayer (playerId);
		}
	}

    //
    // PUBLIC
    //

    public bool IsCrystaled()
    {
        return state == State.CRYSTALED;
    }

    public void Crystalize()
    {
        SetCrystaledState(respawnCrystaledTime);
        statusProvider.setDie(null, Vector2.zero);
    }

    public int GetPlayerId()
    {
        return playerId;
    }

    public float GetAttackCooldownPct()
    {
        return attackCooldown / attackCooldownTime;
    }

    public float GetAttackPct()
    {
        if (state != State.ATTACK)
        {
            return 0.0f;
        }

        switch (aimDirection)
        {
            case AimDirection.UP:
                return stateElapsedTime / attackUpTimeMax;
            case AimDirection.DOWN:
                return stateElapsedTime / attackDownTimeMax;
            case AimDirection.FORWARD:
                return stateElapsedTime / attackForwardTimeMax;
            default:
                return 0.0f;
        }
    }
        
    public float GetSpecialAttackPct()
    {
        if (state != State.SPECIAL_ATTACK)
        {
            return 0.0f;
        }
        
        return stateElapsedTime / specialAttackTime;
    }

    public AimDirection GetAimDirection()
    {
        return aimDirection;
    }

    public void SetBondLink(BondLink bondLink)
    {
        statusProvider.setBoundStatus(bondLink != null);
    }

    //
    // STATE MACHINE
    //

    void Update()
    {
        stateElapsedTime += Time.deltaTime;

        // FIXME trying a dirty fix for the invisible bug
        SetVisible(state != State.INVINCIBLE);

        switch (state)
        {
            case State.IDLE:
                UpdateIdle();
                break;
            case State.CHARGE:
                UpdateCharge();
                break;
            case State.CRYSTALED:
                UpdateCrystaled();
                break;
            case State.INVINCIBLE:
                UpdateInvincible();
                break;
        }
    }

    void FixedUpdate()
    {
        stateElapsedFixedTime += Time.fixedDeltaTime;

        switch (state)
        {
            case State.ATTACK:
                FixedUpdateAttack();
                break;
            case State.SPECIAL_ATTACK:
                FixedUpdateSpecialAttack();
                break;
            case State.KNOCKBACK:
                FixedUpdateKnockback();
                break;
        }
    }

    private void PrepareNewState(State newState)
    {
        //LogDebug ("leave " + state + ", enter " + newState);

        switch (state)
        {          
            case State.IDLE:
                LeaveIdle();
                break;
            case State.ATTACK:
                LeaveAttack();
                break;
            case State.CHARGE:
                LeaveCharge();
                break;
            case State.SPECIAL_ATTACK:
                LeaveSpecialAttack();
                break;
            case State.KNOCKBACK:
                LeaveKnockback();
                break;
            case State.CRYSTALED:
                LeaveCrystaled();
                break;
            case State.INVINCIBLE:
                LeaveInvincible();
                break;
        }

        state = newState;
        stateElapsedTime = 0.0f;
        stateElapsedFixedTime = 0.0f;
    }

    private void SetIdleState()
    {
        PrepareNewState(State.IDLE);
        EnterIdle();
    }

    private void SetAttackState()
    {
        PrepareNewState(State.ATTACK);
        EnterAttack();
    }

    private void SetChargeState()
    {
        PrepareNewState(State.CHARGE);
        EnterCharge();
    }

    private void SetSpecialAttackState()
    {
        PrepareNewState(State.SPECIAL_ATTACK);
        EnterSpecialAttack();
    }

    private void SetKnockbackState(AimDirection direction)
    {
        PrepareNewState(State.KNOCKBACK);
        EnterKnockback(direction);
    }

    private void SetCrystaledState(float duration)
    {
        PrepareNewState(State.CRYSTALED);
        EnterCrystaled(duration);
    }

    private void SetInvincibleState(float duration)
    {
        PrepareNewState(State.INVINCIBLE);
        EnterInvincible(duration);
    }

    //
    // CRYSTALED
    //

    private bool initialSpawn = true;
    private float crystaledTime;
    private bool respawnWarningTriggered;

    private void EnterCrystaled(float duration)
    {     
        crystaledTime = duration;
        respawnWarningTriggered = false;

        movementController.setMovementEnabled(false);
        movementController.setJumpEnabled(false);
    }

    private void LeaveCrystaled()
    {
        movementController.setMovementEnabled(true);
        movementController.setJumpEnabled(true);
    }

    private void UpdateCrystaled()
    {
        if (!respawnWarningTriggered && stateElapsedTime > crystaledTime - respawnWarningTime)
        {            
            statusProvider.setRespawnWarning(initialSpawn);
            respawnWarningTriggered = true;
        }
            
        if (stateElapsedTime > crystaledTime)
        {
            if (initialSpawn)
            {
                initialSpawn = false;
                SetIdleState();
            }
            else
            {                
                SetInvincibleState(invincibleAfterSpawnTime);
            }
        }
    }

    //
    // IDLE
    //

    private void EnterIdle()
    {
    }

    private void LeaveIdle()
    {
    }

    private void UpdateIdle()
    {               
        float aimX = inputManager.AxisValue(playerId, InputManager.Horizontal);
        float aimY = inputManager.AxisValue(playerId, InputManager.Vertical);        
        float aimAngle = Mathf.Atan2(aimY, aimX) * Mathf.Rad2Deg;          
        if (aimAngle < 0.0f)
        {
            aimAngle += 360.0f;
        }

        if (aimAngle > 270.0f - (verticalAimThresholdDegree / 2.0f) && aimAngle < 270.0f + (verticalAimThresholdDegree / 2.0f))
        {
            aimDirection = AimDirection.UP;
        }
        else if (aimAngle > 90.0f - (verticalAimThresholdDegree / 2.0f) && aimAngle < 90.0f + (verticalAimThresholdDegree / 2.0f))
        {
            aimDirection = AimDirection.DOWN;
        }
        else
        {
            aimDirection = AimDirection.FORWARD;
        }
        
        // attack
		if (inputManager.WasPressed(playerId, InputManager.BUTTON_ATTACK))
        {
            if (attackCooldown == 0.0f)
            {
                SetAttackState();
                return;
            }
            LogDebug("attack on CD");
            statusProvider.setAttackFailed ();
        }

        // charge
		if (specialAttackEnabled && inputManager.IsHeld(playerId, InputManager.BUTTON_CHARGE))
        {
            if (attackCooldown == 0.0f)
            {
                SetChargeState();
                return;
            }
            else if (inputManager.WasPressed (playerId, InputManager.BUTTON_CHARGE))
            {
                LogDebug("charge on CD");
                statusProvider.setAttackFailed ();				
            }
        }

        // attack cooldown
        if (attackCooldown > 0.0f)
        {
            attackCooldown -= Time.deltaTime;
            if (attackCooldown < 0.0f)
            {
                attackCooldown = 0.0f;
            }
        }
    }

    //
    // ATTACK
    //

    private Collider2D attackCollider;
    private float attackTime;
    private float attackCancelTimeMin;
    private float attackHitboxStart;
    private float attackHitboxEnd;
    private bool hasAttackBeenCancelled;

    private void EnterAttack()
    { 
        hasAttackBeenCancelled = false;

        switch (aimDirection)
        {
            case AimDirection.UP:
                attackTime = attackUpTimeMax;
                attackCancelTimeMin = attackTime; // not used
                attackCollider = attackUpCollider;
                attackHitboxStart = attackUpHitboxStart;
                attackHitboxEnd = attackUpHitboxEnd;

                movementController.resetVelocity(false, true);
                movementController.setJumpEnabled(false);
                statusProvider.setAttackStart(PlayerStatusProvider.AttackType.UP, Vector2.up);
                break;
            case AimDirection.DOWN:
                attackTime = attackDownTimeMax;
                attackCancelTimeMin = attackDownCancelTimeMin;
                attackCollider = attackDownCollider;
                attackHitboxStart = attackDownHitboxStart;
                attackHitboxEnd = attackDownHitboxEnd;

                movementController.resetVelocity(false, true);
                statusProvider.setAttackStart(PlayerStatusProvider.AttackType.DOWN, Vector2.down);

                if (onGround)
                {
                    // was already on wall, cancel the attack (hit event wouldn't be triggered otherwise)
                    statusProvider.setCollision(PlayerStatusProvider.CollisionType.GROUND_ATTACK, Vector2.down);
                    hasAttackBeenCancelled = true;
                }
                break;
            case AimDirection.FORWARD:
                attackTime = attackForwardTimeMax;
                attackCancelTimeMin = attackTime; // not used
                attackCollider = attackForwardCollider;
                attackHitboxStart = attackForwardHitboxStart;
                attackHitboxEnd = attackForwardHitboxEnd;

                movementController.resetVelocity(true, false);
                movementController.setMovementEnabled(false);
                movementController.setGravityFactor(0.0f);
                movementController.setJumpEnabled(false);

                Vector2 direction = movementController.isFacingRight() ? Vector2.right : Vector2.left;
                statusProvider.setAttackStart(PlayerStatusProvider.AttackType.FORWARD, direction);

                if (onWall)
                {
                    // was already on wall, fire an hit event (hit event wouldn't be triggered otherwise)
                    statusProvider.setCollision(PlayerStatusProvider.CollisionType.WALL_ATTACK, direction);
                }
                break;
        }
    }
    
    private void LeaveAttack()
    {
        attackCollider.enabled = false;
        switch (aimDirection)
        {
            case AimDirection.UP:
                movementController.setJumpEnabled(true);
                break;
            case AimDirection.DOWN:
                break;
            case AimDirection.FORWARD:
                movementController.setGravityFactor(1.0f);
                movementController.setMovementEnabled(true);
                movementController.setJumpEnabled(true);
                break;
        }
        
        attackCooldown = attackCooldownTime;
    }

    private void FixedUpdateAttack()
    {      
        // if attack time is over
        if (stateElapsedFixedTime > attackTime)
        {
            SetIdleState();
            return;
        }

        if (hasAttackBeenCancelled && stateElapsedFixedTime > attackCancelTimeMin)
        {
            SetIdleState();
            return;
        }

        float attackPct = stateElapsedFixedTime / attackTime;

        if (!attackCollider.enabled && attackPct >= attackHitboxStart && attackPct < attackHitboxEnd)
        {
            attackCollider.enabled = true;
        }
        else if (attackCollider.enabled && attackPct >= attackHitboxEnd)
        {
            attackCollider.enabled = false;
        }

        switch (aimDirection)
        {
            case AimDirection.UP:
                {
                    float force = (attackUpForceMin + attackUpCurve.Evaluate(attackPct) * (attackUpForceMax - attackUpForceMin)) * Time.fixedDeltaTime;
                    movementController.applyForce(new Vector2(0.0f, force));
                    break;
                }
            case AimDirection.DOWN:
                {
                    float force = (attackDownForceMin + attackDownCurve.Evaluate(attackPct) * (attackDownForceMax - attackDownForceMin)) * Time.fixedDeltaTime;
                    movementController.applyForce(new Vector2(0.0f, -force));
                    break;
                }
            case AimDirection.FORWARD:
                {
                    float force = (attackForwardForceMin + attackForwardCurve.Evaluate(attackPct) * (attackForwardForceMax - attackForwardForceMin)) * Time.fixedDeltaTime;
                    movementController.applyForce(new Vector2(movementController.isFacingRight() ? force : -force, 0.0f));
                    movementController.setGravityFactor(attackPct);
                    break;
                }
        }
    }

    //
    // CHARGE
    //
    
	private bool chargeReady;
    private float chargeForceRatio;
    private bool hasChargeBeenCompleted;

    private void EnterCharge()
    {
		chargeReady = false;
        hasChargeBeenCompleted = false;
        movementController.setMovementEnabled(false);
        movementController.setJumpEnabled(false);
        
        statusProvider.setChargeStart();
    }
    
    private void LeaveCharge()
    {
        movementController.setMovementEnabled(true);
        movementController.setJumpEnabled(true);
        movementController.setGravityFactor(1.0f);

        attackCooldown = attackCooldownTime;

        statusProvider.setChargeStop(hasChargeBeenCompleted);
    }
    
    private void UpdateCharge()
    {
		// gradually reduce gravity
		if(stateElapsedTime < chargeGravityReductionTime)
		{
            movementController.setGravityFactor(stateElapsedTime / chargeGravityReductionTime);
		} 
		else 
		{
			movementController.resetVelocity();
			movementController.setGravityFactor(0.0f);
		}

		// detect if charge is ready
        if (stateElapsedTime >= chargeReadyTime && !chargeReady) {
			chargeReady = true;
			statusProvider.setChargeReady ();
		}

		// cancel charge if max time exceeded
        if (stateElapsedTime > chargeOverflowTime)
		{
            SetIdleState();
			return;
		}

        // start or cancel charge when button is released and cancel time expired
        if(stateElapsedTime > chargeInputCancelTimeMin && !inputManager.IsHeld(playerId, InputManager.BUTTON_CHARGE))
		{
            if(stateElapsedTime >= chargeReadyTime)
			{			
				LaunchSpecialAttack();
				return;
			}
			else
			{
                SetIdleState();
				return;
			}
		}

        if (stateElapsedTime < chargeReadyTime)
        {
            float statePct = stateElapsedTime / chargeReadyTime;
            statusProvider.setChargeUpdate(PlayerStatusProvider.ChargeState.LOAD, statePct, 0.0f);
        }
        else if (stateElapsedTime < chargeFullTime)
        {
            float statePct = (stateElapsedTime - chargeReadyTime) / (chargeFullTime - chargeReadyTime);
            float forceRatio = CalculateChargeForceRatio();
            statusProvider.setChargeUpdate(PlayerStatusProvider.ChargeState.READY, statePct, forceRatio);
        }
        else
        {
            float statePct = (stateElapsedTime - chargeFullTime) / (chargeOverflowTime - chargeFullTime);
            statusProvider.setChargeUpdate(PlayerStatusProvider.ChargeState.FULL, statePct, 1.0f);
        }

				
    }       

	private void LaunchSpecialAttack()
	{      
        chargeForceRatio = CalculateChargeForceRatio();
        hasChargeBeenCompleted = true;
        SetSpecialAttackState();
	}

    private float CalculateChargeForceRatio()
    {
        float chargePct = Mathf.Clamp((stateElapsedTime - chargeReadyTime) / (chargeFullTime - chargeReadyTime), 0.0f, 1.0f);
        return chargeReadyForceRatioCurve.Evaluate(chargePct);
    }

    //
    // SPECIAL ATTACK
    //

    private Vector2 specialAttackVector = new Vector2();
    private bool hasSpecialAttackBeenCancelled;

    private void EnterSpecialAttack()
    {     
        hasSpecialAttackBeenCancelled = false;

        movementController.resetVelocity();
        movementController.setGravityFactor(0.0f);
        movementController.setFrictionEnabled(false);
        movementController.setMovementEnabled(false);
        movementController.setJumpEnabled(false);
        
        specialAttackVector.x = inputManager.AxisValue(playerId, InputManager.Horizontal);
        specialAttackVector.y = -inputManager.AxisValue(playerId, InputManager.Vertical);
        if (Mathf.Abs(specialAttackVector.x) < Mathf.Epsilon && Mathf.Abs(specialAttackVector.y) < Mathf.Epsilon)
        {
            specialAttackVector.x = movementController.isFacingRight() ? 1.0f : -1.0f;
            specialAttackVector.y = 0.0f;
        }
        else
        {
            specialAttackVector.Normalize();
        }


        float angle = Mathf.Sign(specialAttackVector.y) * Vector2.Angle(specialAttackVector.x < 0.0f ? Vector2.left : Vector2.right, specialAttackVector);
        specialAttackBase.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        movementController.setFacingRight(specialAttackVector.x >= 0.0f);
        statusProvider.setAttackStart(PlayerStatusProvider.AttackType.SPECIAL, specialAttackVector);		       
    }

    private void LeaveSpecialAttack()
    {
        movementController.resetVelocity();
        movementController.setGravityFactor(1.0f);
        movementController.setFrictionEnabled(true);
        movementController.setMovementEnabled(true);
        movementController.setJumpEnabled(true);

        specialAttackCollider.enabled = false;
        
        attackCooldown = attackCooldownTime;

        statusProvider.setAttackStop(PlayerStatusProvider.AttackType.SPECIAL, hasSpecialAttackBeenCancelled);
    }

    private void FixedUpdateSpecialAttack()
    {           
        // if attack time is over
        if (stateElapsedFixedTime > specialAttackTime)
        {
            SetIdleState();
            return;
        }
        
		// if special attack hit solid
        if (!hasSpecialAttackBeenCancelled && movementController.IsHittingSolid(specialAttackBase, specialAttackSolidChecks, specialAttackVector))
        {
            hasSpecialAttackBeenCancelled = true;
            statusProvider.setCollision (PlayerStatusProvider.CollisionType.SPECIAL_ATTACK, specialAttackVector);
        }

        if (hasSpecialAttackBeenCancelled && stateElapsedFixedTime > specialAttackCancelTimeMin)
        {
            SetIdleState();
            return;
        }
            
        // calculate attack percentage
        float attackPct = stateElapsedFixedTime / specialAttackTime;

        // enable/disable hitbox
        if (!specialAttackCollider.enabled && attackPct >= specialAttackHitboxStart && attackPct < specialAttackHitboxEnd)
        {
            specialAttackCollider.enabled = true;
        }
        else if (specialAttackCollider.enabled && attackPct >= specialAttackHitboxEnd)
        {
            specialAttackCollider.enabled = false;
        }

        // change velocity
        float forcePct = specialAttackCurve.Evaluate(attackPct) * chargeForceRatio;
        float hForce = (Mathf.Sign(specialAttackVector.x) * specialAttackForceMin + specialAttackVector.x * forcePct * (specialAttackForceMax - specialAttackForceMin)) * Time.fixedDeltaTime;
        float vForce = (Mathf.Sign(specialAttackVector.y) * specialAttackForceMin + specialAttackVector.y * forcePct * (specialAttackForceMax - specialAttackForceMin)) * Time.fixedDeltaTime;
		movementController.setVelocity(new Vector2(hForce, vForce));
    }

    //
    // KNOCKBACK
    //

    private AimDirection knockbackDirection;

    private void EnterKnockback(AimDirection dir)
    {
        knockbackDirection = dir;

        movementController.setMovementEnabled(false);
        movementController.setJumpEnabled(false);
        
        if (knockbackDirection == AimDirection.FORWARD)
        {
            statusProvider.setHorizontalKnockback();
        }
        else
        {
            statusProvider.setVerticalKnockback();
        }   
    }

    private void LeaveKnockback()
    {
        movementController.setMovementEnabled(true);
        movementController.setJumpEnabled(true);
    }

    private void FixedUpdateKnockback()
    {
        if (stateElapsedFixedTime > knockbackTime)
        {
            SetIdleState();
            return;
        }

        float knockbackPct = stateElapsedFixedTime / knockbackTime;
        float knockbackVelocityPct = knockbackCurve.Evaluate(knockbackPct);

        switch (knockbackDirection)
        {
            case AimDirection.UP:
                movementController.applyForce(new Vector2(0.0f, knockbackVelocityPct * Time.deltaTime * knockbackForceUp));
                break;
            case AimDirection.DOWN:
                movementController.applyForce(new Vector2(0.0f, knockbackVelocityPct * Time.deltaTime * -knockbackForceDown));
                break;
            case AimDirection.FORWARD:
                float velocity = movementController.isFacingRight() ? -knockbackForceForward : knockbackForceForward;
                movementController.applyForce(new Vector2(knockbackVelocityPct * Time.deltaTime * velocity, 0.0f));
                break;
        }
    }

	//
	// SHIELD
	//

	public void ActivateShield() {
		hasShield = true;
		shieldAnimation.gameObject.SetActive (true);
		shieldAnimation.Create ();
	}

	public void DisableShield() {
		hasShield = false;
		shieldAnimation.Break ();
	}

    //
    // INVINCIBLE
    //

    private float invincibleTime;
    private float invisibleBlinkCounter = 0.0f;

    private void EnterInvincible(float duration)
    {
        LogDebug("EnterInvincible(" + duration + ")");
        invincibleTime = duration;
        invisibleBlinkCounter = invinsibleBlinkInterval;

        statusProvider.setInvincibleStatus(true);
    }

    private void LeaveInvincible()
    {
        SetVisible(true);

        statusProvider.setInvincibleStatus(false);
    }

    private void UpdateInvincible()
    {
        UpdateIdle();

        if (stateElapsedTime > invincibleTime)
        {
            SetVisible(true);
            SetIdleState();
            return;
        }

        invisibleBlinkCounter -= Time.deltaTime;
        if (invisibleBlinkCounter < 0.0f)
        {
            invisibleBlinkCounter += invinsibleBlinkInterval;
            SetVisible(!visible);
        }
    }

    //
    // EVENTS HANDLING
    //

    public void HandleOnCollide(Collider2D source, Collider2D other)
    {
        //LogDebug ("HandleOnCollide");
        PlayerStateController enemy = other.GetComponentInParent<PlayerStateController>();
        if (enemy != null)
        {
            //LogDebug ("collide with enemy " + enemy.playerId);
            if (enemy.IsPerformingAttack() && IsAimingOppositeDirection(enemy))
            {
                enemy.SetKnockbackState(aimDirection);
                SetKnockbackState(enemy.aimDirection);
            }
            else if (enemy.IsAttackable())
            {
                enemy.HitWithAttack(transform, aimDirection);
            }
            return;
        }
        
        BondLink bondLink = other.GetComponentInParent<BondLink>();
        if (bondLink != null)
        {
            //LogDebug ("collide with link");
            LevelManager levelManager = FindObjectOfType<LevelManager>();
            levelManager.bondHasBeenBrokenBy(this);
            return;
        }
        
        LogWarn("unexpected collision");
    }

    private void HandleOnGroundedStatusChanged(bool isGrounded)
    {
        onGround = isGrounded;

        if (!isGrounded)
        {
            return;
        }

        if (state == State.ATTACK && aimDirection == AimDirection.DOWN)
        {
            hasAttackBeenCancelled = true;
            statusProvider.setCollision(PlayerStatusProvider.CollisionType.GROUND_ATTACK, Vector2.down);
        }
        else
        {
            statusProvider.setCollision(PlayerStatusProvider.CollisionType.GROUND, Vector2.down);
        }
    }

    private void HandleOnOnWallStatusChanged(bool isOnWall)
    {    
        onWall = isOnWall;

        if (!isOnWall)
        {
            return;
        }

        if (state == State.ATTACK && aimDirection == AimDirection.FORWARD)
        {
            statusProvider.setCollision(PlayerStatusProvider.CollisionType.WALL_ATTACK, movementController.isFacingRight() ? Vector2.right : Vector2.left);
        }
        else
        {
            statusProvider.setCollision(PlayerStatusProvider.CollisionType.WALL, movementController.isFacingRight() ? Vector2.right : Vector2.left);
        }
    }

    //
    // HELPERS
    //

    private void HitWithAttack(Transform source, AimDirection dir)
    {
        PlayerStateController sourceState = source.GetComponent<PlayerStateController>();

        Vector2 attackVector;
        switch (sourceState.aimDirection)
        {
            case AimDirection.UP:
                attackVector = new Vector2(0.0f, 1.0f);
                break;
            case AimDirection.DOWN:
                attackVector = new Vector2(0.0f, -1.0f);
                break;
            case AimDirection.FORWARD:      
                attackVector = new Vector2(sourceState.movementController.isFacingRight() ? 1.0f : -1.0f, 0.0f);
                break;
            default:
                attackVector = new Vector2(0.0f, 0.0f); // unreachable
                LogWarn("unreachable code");
                break;
        }

        SetCrystaledState(respawnCrystaledTime);

        // this is kind of mixing logic and display but cannot rely on other component to trigger die event
        TimeManager.Pause(diePauseTime, delegate()
        {
            statusProvider.setDieWarning(source, attackVector);
        }, delegate()
        {
            statusProvider.setDie(source, attackVector);
        });
    }
 
    private bool IsPerformingAttack()
    {
        switch (state)
        {
            case State.ATTACK:
                return attackCollider.enabled;          
            default:
                return false;
        }
    }

    private bool IsAttackable()
    {
		if (hasShield)
        {
            return false;
        }
    
		return state == State.IDLE || state == State.CHARGE || state == State.ATTACK || state == State.KNOCKBACK;
    }

    private bool IsAimingOppositeDirection(PlayerStateController enemy)
    {
        if (aimDirection == AimDirection.UP && enemy.aimDirection == AimDirection.DOWN)
            return true;

        if (aimDirection == AimDirection.DOWN && enemy.aimDirection == AimDirection.UP)
            return true;

        if (aimDirection == AimDirection.FORWARD && enemy.aimDirection == AimDirection.FORWARD)
            return movementController.isFacingRight() != enemy.movementController.isFacingRight();

        return false;
    }

    private void SetVisible(bool visible)
    {
        if (this.visible != visible)
        {
            foreach (SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>())
            {
                sprite.enabled = visible;
            }
            this.visible = visible;
        }
    }

    void LogDebug(string text)
    {
        print("p" + playerId + ": " + text);
    }

    void LogWarn(string text)
    {
        Debug.LogWarning("p" + playerId + ": " + text, this);
    }

    //
    // Debug DRAW
    //

    private static float DEBUG_ALPHA = 0.5f;

    public void OnDrawGizmos()
    {
        if (!awake)
        {
            return;
        }

        switch (state)
        {
            case State.IDLE:
                DrawColliderRect(bodyCollider, Color.white);
                break;
            case State.ATTACK:
                DrawColliderRect(bodyCollider, Color.white);
                if(attackCollider.enabled)
                {
                    DrawColliderRect(attackCollider, Color.red);
                }
                break;
            case State.SPECIAL_ATTACK:
                DrawColliderRect(bodyCollider, Color.white);
                if(specialAttackCollider.enabled)
                {
                    DrawColliderRect(specialAttackCollider, Color.red);
                }
                foreach(Transform solidCheck in specialAttackSolidChecks)
                {
                    DrawRect(solidCheck.position.x, solidCheck.position.y, 0.2f, 0.2f, Color.yellow);
                }
                break;
            case State.CHARGE:
                DrawColliderRect(bodyCollider, Color.white);
                break;
            case State.KNOCKBACK:
                DrawColliderRect(bodyCollider, Color.grey);
                break;
            case State.CRYSTALED:
                DrawColliderRect(bodyCollider, Color.gray);
                break;
            case State.INVINCIBLE:
                DrawColliderRect(bodyCollider, Color.gray);
                break;
        }
    }

    private static void DrawColliderRect(Collider2D collider, Color color)
    {
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(collider.transform.position, collider.transform.rotation, collider.transform.lossyScale);
        Gizmos.matrix = rotationMatrix; 
        Gizmos.color = new Color(color.r, color.g, color.b, DEBUG_ALPHA);
        Gizmos.DrawCube(collider.offset, ((BoxCollider2D)collider).size);
    }

    private static void DrawRect(float x, float y, float width, float height, Color color)
    {
        Gizmos.matrix = Matrix4x4.identity; 
        Gizmos.color = new Color(color.r, color.g, color.b, DEBUG_ALPHA);
        Gizmos.DrawCube(new Vector3(x, y, 0.0f), new Vector3(width, height, 0.0f));
    }

    public void SetGameOver()
    {
        movementController.gameOver = true;
    }
}

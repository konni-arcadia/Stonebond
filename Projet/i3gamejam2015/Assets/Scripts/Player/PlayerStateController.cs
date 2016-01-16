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
    
    // 
    // GLOBAL STATE
    //

    public int playerId;

    private enum State
    {
        SPAWN,
        IDLE,
        ATTACK,
        CHARGE,
        SPECIAL_ATTACK,
        KNOCKBACK,
        CRYSTALED,
        INVINCIBLE
    }

    private State state = State.SPAWN;
    private float stateTime = 0.0f;
    private bool isBond = false;
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
    // SPAWN
    //

    public float initialSpawnTime = 1.6f;
    public float respawnTime = 1.3f;

    //
    // ATTACK
    // 

    public float attackUpTimeMin = 0.25f;
    public float attackUpTimeMax = 0.25f;
    public float attackUpForceMin = 0.0f;
    public float attackUpForceMax = 46000.0f;
    public float attackUpHitboxStart = 0.0f;
    public float attackUpHitboxEnd = 0.9f;
    public AnimationCurve attackUpCurve;
    public Collider2D attackUpCollider;

    public float attackDownTimeMin = 0.25f;
    public float attackDownTimeMax = 0.25f;
    public float attackDownForceMin = 0.0f;
    public float attackDownForceMax = 46000.0f;
    public float attackDownHitboxStart = 0.0f;
    public float attackDownHitboxEnd = 1.0f;
    public AnimationCurve attackDownCurve;
    public Collider2D attackDownCollider;

    public float attackForwardTimeMin = 0.2f;
    public float attackForwardTimeMax = 0.2f;
    public float attackForwardForceMin = 0.0f;
    public float attackForwardForceMax = 30000.0f;
    public float attackForwardHitboxStart = 0.0f;
    public float attackForwardHitboxEnd = 0.6f;
    public AnimationCurve attackForwardCurve;
    public Collider2D attackForwardCollider;
    public float attackCooldownTime = 0.3f;

    // 
    // CHARGE
    //

    public float chargeTime = 1.0f;
	public float chargeGravityReductionTime = 0.08f; // secs
	public bool chargeReady = false;
	public float chargeReadyTime; // secs
	public float chargeReadyMaxTime = 2.0f; // max time player can stay with charge ready in secs

    //
    // SPECIAL ATTACK
    // 

    public float specialAttackTime = 0.35f;
    public float specialAttackForceMin = 0.0f;
    public float specialAttackForceMax = 30000.0f;
    public float specialAttackHitboxStart = 0.0f;
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
    public float crystaledTime = 2.0f;

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

        // start in spawn state
        SetState(State.SPAWN);
    }

    //
    // PUBLIC
    //

    public bool IsCrystaled()
    {
        return state == State.CRYSTALED || state == State.SPAWN;
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
                return stateTime / attackUpTimeMax;
            case AimDirection.DOWN:
                return stateTime / attackDownTimeMax;
            case AimDirection.FORWARD:
                return stateTime / attackForwardTimeMax;
            default:
                return 0.0f;
        }
    }

    public float GetChargePct()
    {
        if (state != State.CHARGE)
        {
            return 0.0f;
        }

        return stateTime / chargeTime;
    }

    public float GetSpecialAttackPct()
    {
        if (state != State.SPECIAL_ATTACK)
        {
            return 0.0f;
        }
        
        return stateTime / specialAttackTime;
    }

    public AimDirection GetAimDirection()
    {
        return aimDirection;
    }

    public void SetBondLink(BondLink bondLink)
    {
        isBond = bondLink != null;
        statusProvider.setBoundStatus(bondLink != null);
    }

    //
    // STATE MACHINE
    //

    void Update()
    {
        switch (state)
        {
            case State.SPAWN:       
                UpdateSpawn();
                break;
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

    private void SetState(State newState)
    {
        //LogDebug ("leave " + state + ", enter " + newState);

        switch (state)
        {
            case State.SPAWN:       
                LeaveSpawn();
                break;
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
        switch (state)
        {
            case State.SPAWN:       
                EnterSpawn();
                break;
            case State.IDLE:
                EnterIdle();
                break;
            case State.ATTACK:
                EnterAttack();
                break;
            case State.CHARGE:
                EnterCharge();
                break;
            case State.SPECIAL_ATTACK:
                EnterSpecialAttack();
                break;
            case State.KNOCKBACK:
                EnterKnockback();
                break;
            case State.CRYSTALED:
                EnterCrystaled();
                break;
            case State.INVINCIBLE:
                EnterInvincible();
                break;
        }
    }

    //
    // SPAWN
    //

    private bool initialSpawn = true;

    private void EnterSpawn()
    {
        stateTime = initialSpawn ? initialSpawnTime : respawnTime;
        movementController.setMovementEnabled(false);
        movementController.setJumpEnabled(false);

        statusProvider.setRespawn(initialSpawn);
    }

    private void LeaveSpawn()
    {
        movementController.setMovementEnabled(true);
        movementController.setJumpEnabled(true);
    }

    private void UpdateSpawn()
    {
        stateTime -= Time.deltaTime;
        if (stateTime <= 0.0f)
        {
            if (initialSpawn)
            {
                SetState(State.IDLE);
                initialSpawn = false;
            }
            else
            {
                stateTime = invincibleAfterSpawnTime; // FIXME state arg
                SetState(State.INVINCIBLE);
            }
        }
    }

    //
    // CRYSTALED
    //

    private void EnterCrystaled()
    {
        stateTime = crystaledTime;
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
        stateTime -= Time.deltaTime;
        if (stateTime <= 0.0f)
        {
            SetState(State.SPAWN);
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
                SetState(State.ATTACK);
                return;
            }
            else
            {
                print("p" + playerId + ": attack on CD");
                statusProvider.setAttackFailed();
            }
        }

        // charge
        if (inputManager.WasPressed(playerId, InputManager.BUTTON_CHARGE))
        {
            if (attackCooldown == 0.0f)
            {
                SetState(State.CHARGE);
                return;
            }
            else
            {
                print("p" + playerId + ": charge on CD");
                statusProvider.setAttackFailed();
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
    private float attackHitboxStart;
    private float attackHitboxEnd;

    private void EnterAttack()
    { 
        stateTime = 0.0f;

        switch (aimDirection)
        {
            case AimDirection.UP:
                attackCollider = attackUpCollider;
                attackHitboxStart = attackUpHitboxStart;
                attackHitboxEnd = attackUpHitboxEnd;

                movementController.resetVelocity(false, true);
                movementController.setJumpEnabled(false);
                statusProvider.setAttackUp();
                break;
            case AimDirection.DOWN:
                attackCollider = attackDownCollider;
                attackHitboxStart = attackDownHitboxStart;
                attackHitboxEnd = attackDownHitboxEnd;

                if (onGround)
                {
                    // can't perform down attack while on ground
                    statusProvider.setAttackFailed();
                    SetState(State.IDLE);
                }
                else
                {
                    movementController.resetVelocity(false, true);
                    statusProvider.setAttackDown();
                }
                break;
            case AimDirection.FORWARD:
                attackCollider = attackForwardCollider;
                attackHitboxStart = attackForwardHitboxStart;
                attackHitboxEnd = attackForwardHitboxEnd;

                movementController.resetVelocity(true, false);
                movementController.setMovementEnabled(false);
                movementController.setGravityFactor(0.0f);
                movementController.setJumpEnabled(false);
                statusProvider.setAttackForward();

                if (onWall)
                {
                    // was already on wall, fire an hit event (hit event wouldn't be triggered otherwise)
                    statusProvider.setCollision(PlayerStatusProvider.CollisionType.WALL_ATTACK, new Vector2());
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
        float attackMinTime = 0.0f;
        float attackMaxTime = 0.0f;

        switch (aimDirection)
        {
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

        stateTime += Time.fixedDeltaTime;
    
        // if attack time is over
        if (stateTime > attackMaxTime || (stateTime > attackMinTime && !inputManager.IsHeld(playerId, InputManager.BUTTON_ATTACK)))
        {
            SetState(State.IDLE);
            return;
        }

        float attackPct = stateTime / attackMaxTime;

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
    
    private void EnterCharge()
    {
        stateTime = 0.0f;
        
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
    }
    
    private void UpdateCharge()
    {
		// increase state time
		stateTime += Time.deltaTime;

		// increase charge ready time if ready
		if(chargeReady)
			chargeReadyTime += Time.deltaTime;

		// gradually reduce gravity
		if(stateTime < chargeGravityReductionTime)
		{
			movementController.setGravityFactor(stateTime / chargeGravityReductionTime);
		} 
		else 
		{
			movementController.resetVelocity();
			movementController.setGravityFactor(0.0f);
		}

		// stop charge if charge button is released before charge is ready
		// or charge ready time exceeds max charge time
		if (!chargeReady && !inputManager.IsHeld(playerId, InputManager.BUTTON_CHARGE)
			|| (chargeReady && chargeReadyTime >= chargeReadyMaxTime))
		{
			chargeReady = false;
            statusProvider.setChargeStop(false);
            SetState(State.IDLE);
            return;
        }

		// detect if charge is ready ?
        if (stateTime >= chargeTime && !chargeReady)
        {
			chargeReady = true;
			chargeReadyTime = 0;
            statusProvider.setChargeReady();
        }

		// trigger special attack if charge button is released when charge is ready
		if(chargeReady && !inputManager.IsHeld(playerId, InputManager.BUTTON_CHARGE))
		{
			chargeReady = false;
			statusProvider.setChargeStop(true);
			SetState(State.SPECIAL_ATTACK);
		}
    }

    //
    // SPECIAL ATTACK
    //

    private Vector2 specialAttackVector = new Vector2();

    private void EnterSpecialAttack()
    {
        stateTime = 0.0f;

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

        statusProvider.setAttackSpecialStart(specialAttackVector);		       
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

        statusProvider.setAttackSpecialStop();
    }

    private void FixedUpdateSpecialAttack()
    {
        stateTime += Time.fixedDeltaTime;
        
        // if attack time is over
        if (stateTime > specialAttackTime)
        {
            SetState(State.IDLE);
            return;
        }
        
		// if special attack hit solid
        if (movementController.IsHittingSolid(specialAttackBase, specialAttackSolidChecks, specialAttackVector))
        {
            statusProvider.setCollision (PlayerStatusProvider.CollisionType.SPECIAL_ATTACK, specialAttackVector);
            SetState (State.IDLE);
            return;
        }

        float attackPct = stateTime / specialAttackTime;

        if (!specialAttackCollider.enabled && attackPct >= specialAttackHitboxStart && attackPct < specialAttackHitboxEnd)
        {
            specialAttackCollider.enabled = true;
        }
        else if (specialAttackCollider.enabled && attackPct >= specialAttackHitboxEnd)
        {
            specialAttackCollider.enabled = false;
        }

        float forcePct = specialAttackCurve.Evaluate(attackPct);
        float hForce = (Mathf.Sign(specialAttackVector.x) * specialAttackForceMin + specialAttackVector.x * forcePct * (specialAttackForceMax - specialAttackForceMin)) * Time.fixedDeltaTime;
        float vForce = (Mathf.Sign(specialAttackVector.y) * specialAttackForceMin + specialAttackVector.y * forcePct * (specialAttackForceMax - specialAttackForceMin)) * Time.fixedDeltaTime;
        movementController.setVelocity(new Vector2(hForce, vForce));
    }

    //
    // KNOCKBACK
    //

    private AimDirection knockbackDirection;

    private void EnterKnockback()
    {
        stateTime = knockbackTime;

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
        stateTime -= Time.fixedDeltaTime;
        if (stateTime <= 0.0f)
        {
            SetState(State.IDLE);
            return;
        }

        float knockbackPct = 1.0f - stateTime / knockbackTime;
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
    // INVINCIBLE
    //

    private float invisibleBlinkCounter = 0.0f;

    private void EnterInvincible()
    {
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

        stateTime -= Time.deltaTime;
        if (stateTime <= 0.0f)
        {
            SetState(State.IDLE);
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
        LogDebug ("HandleOnCollide");
        PlayerStateController enemy = other.GetComponentInParent<PlayerStateController>();
        if (enemy != null)
        {
            LogDebug ("collide with enemy " + enemy.playerId);
            if (enemy.IsPerformingAttack() && IsAimingOppositeDirection(enemy))
            {
                enemy.Knockback(aimDirection);
                Knockback(enemy.aimDirection);
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
            LogDebug ("collide with link");
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
            SetState(State.IDLE);
            statusProvider.setCollision(PlayerStatusProvider.CollisionType.GROUND_ATTACK, new Vector2());
        }
        else
        {
            statusProvider.setCollision(PlayerStatusProvider.CollisionType.GROUND, new Vector2());
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
            statusProvider.setCollision(PlayerStatusProvider.CollisionType.WALL_ATTACK, new Vector2());
        }
        else
        {
            statusProvider.setCollision(PlayerStatusProvider.CollisionType.WALL, new Vector2());
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

        SetState(State.CRYSTALED);

        // this is kind of mixing logic and display but cannot rely on other component to trigger die event
        TimeManager.Pause(diePauseTime, delegate()
        {
            statusProvider.setDieWarning(source, attackVector);
        }, delegate()
        {
            statusProvider.setDie(source, attackVector);
        });
    }

    private void Knockback(AimDirection dir)
    {
        knockbackDirection = dir; // FIXME state arg
        SetState(State.KNOCKBACK);
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
        if (isBond)
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

using UnityEngine;

public class PlayerFXManager : MonoBehaviour
{
    //
    // CHROMA
    //

    public float chromaLightNormal = 0.6f;
    public float chromaLightMin = 0.0f;
    public float chromaLightMax = 1.0f;    

    public AnimationCurve attackForwardChromaCurve;
    public AnimationCurve attackUpChromaCurve;
    public AnimationCurve attackDownChromaCurve;
    public AnimationCurve attackCooldownChromaCurve;
    public AnimationCurve respawnChromaCurve;
    public AnimationCurve killedChromaCurve;    

    //
    // OVERLAY
    //

    public string overlaySortingLayerName = "HUD";
    public int overlaySortingOrder = 100;

    //
    // REFERENCES
    //

    private PlayerStatusProvider statusProvider;
    private PlayerStateController stateController;
    private PlayerMovementController movementController;
    private Transform body;
    private SpriteRenderer bodyRenderer;
    private string originalSortingLayerName;
    private int originalSortingOrder;
    private GameState.PlayerInfo playerInfo;

    //
    // GLOBAL STATE
    //

    private bool overlay = false;

    
    //
    // INIT
    //
    
    void Awake()
    {
        statusProvider = transform.GetComponent<PlayerStatusProvider>();
        stateController = transform.GetComponent<PlayerStateController>();
        movementController = transform.GetComponent<PlayerMovementController>();
        body = transform.Find("CharacterSprites").Find("Body");
        bodyRenderer = body.GetComponent<SpriteRenderer>();

        statusProvider.OnHorizontalKnockbackAction += HandleOnHorizontalKnockback;
        statusProvider.OnVerticalKnockbackAction += HandleOnVerticalKnockback;
        statusProvider.OnDieWarningAction += HandleOnDieWarning;
        statusProvider.OnDieAction += HandleOnDie;
        statusProvider.OnCollisionAction += HandleOnCollisionAction;
        statusProvider.OnAttackStartAction += HandleOnAttackStartAction;
        statusProvider.OnAttackStopAction += HandleOnAttackStopAction;

        originalSortingLayerName = bodyRenderer.sortingLayerName;
        originalSortingOrder = bodyRenderer.sortingOrder;
    }

    void Start()
    {
        playerInfo = GameState.Instance.Player(stateController.GetPlayerId());
        SetChromaLightCurveValue(0.0f);
    }

    void Update()
    {
        if (overlay)
        {
            // don't change the sprite color while in overlay mode
        }
        else if(stateController.IsCrystaled())
        {
            UpdateCrystalized();            
        }
        else if (stateController.GetAttackPct() > 0.0f)
        {
            switch (stateController.GetAimDirection())
            {
                case PlayerStateController.AimDirection.UP:
                    SetChromaLightCurveValue(attackUpChromaCurve.Evaluate(stateController.GetAttackPct()));
           
                    break;
                case PlayerStateController.AimDirection.DOWN:
                    SetChromaLightCurveValue(attackDownChromaCurve.Evaluate(stateController.GetAttackPct()));
                    break;
                case PlayerStateController.AimDirection.FORWARD:
                    SetChromaLightCurveValue(attackForwardChromaCurve.Evaluate(stateController.GetAttackPct()));
                    break;
            }

        }                       
        else if(stateController.GetAttackCooldownPct() > 0.0f)
        {
            SetChromaLightCurveValue(chromaLightNormal);
        }
        else
        {
            SetChromaLightCurveValue(chromaLightNormal);
        }

        bodyRenderer.material.SetFloat("_NormalYModifier", transform.GetComponent<PlayerMovementController>().isFacingRight() ? -1.0f : 1.0f);

        if(movementController.isOnWall())
        {
            // fix sprite position while player is sliding on wall
            body.transform.localPosition = new Vector3(0.18f, 0.0f, 0.0f);
        }
        else
        {
            body.transform.localPosition = Vector3.zero;
        }
    }

    //
    // CRYSTALIZED
    //

    private float crystalizedElapsedTime = 100.0f; // small hack to prevent killedChromaCurve on first spawn
    public float killedChromaDuration = 0.4f;

    private void UpdateCrystalized()
    {
        crystalizedElapsedTime += Time.deltaTime;

        if(crystalizedElapsedTime < killedChromaDuration)
        {
            SetChromaLightCurveValue(killedChromaCurve.Evaluate(crystalizedElapsedTime / killedChromaDuration));
        }
        else
        {
            SetChromaLightCurveValue(respawnChromaCurve.Evaluate(stateController.GetRespawnPct()));
        }
    }

    //
    // EVENTS
    //

    private void HandleOnHorizontalKnockback()
    {
        ScreenShake.ShakeX(0.75f, 2.0f);
    }

    private void HandleOnVerticalKnockback()
    {
        ScreenShake.ShakeY(0.75f, 2.0f);
    }

    private void HandleOnDieWarning(Transform source, Vector2 attackDirection)
    {
        Overlay.ShowFlash(stateController.diePauseTime);
        SetOverlay(true);
        SetShadow();

        if(source != null)
        {
            PlayerFXManager sourceFXManager = source.GetComponent<PlayerFXManager>();
            sourceFXManager.SetOverlay(true);
            sourceFXManager.SetShadow();
        }        
    }

    private void HandleOnDie(Transform source, Vector2 attackDirection)
    {
        if(attackDirection != Vector2.zero)
        {
            ScreenShake.ShakeXY(Mathf.Abs(attackDirection.x) * 0.4f, 0.5f, Mathf.Abs(attackDirection.y) * 0.4f, 0.5f);
        }
        SetOverlay(false);

        if(source != null)
        {
            PlayerFXManager sourceFXManager = source.GetComponent<PlayerFXManager>();
            sourceFXManager.SetOverlay(false);
        }

        PlayerAnimationBreakDeath breakableStatue = gameObject.GetComponentInChildren<PlayerAnimationBreakDeath> ();
        breakableStatue.hitDirection = attackDirection;
        breakableStatue.Die ();

        crystalizedElapsedTime = 0.0f;
    }

    private void HandleOnCollisionAction(PlayerStatusProvider.CollisionType collisionType, Vector2 normal)
    {
        switch (collisionType)
        {
            case PlayerStatusProvider.CollisionType.SPECIAL_ATTACK:
                ScreenShake.ShakeXY(Mathf.Abs(normal.x) * 0.32f, Mathf.Abs(normal.x) * 1.7f, Mathf.Abs(normal.y) * 0.32f, Mathf.Abs(normal.y) * 1.7f);
                break;
            case PlayerStatusProvider.CollisionType.WALL_ATTACK:
                ScreenShake.ShakeX(0.24f, 1.7f);
                break;
            case PlayerStatusProvider.CollisionType.GROUND_ATTACK:
                ScreenShake.ShakeY(0.3f, 1.7f);
                break;
        }
    }

    private void HandleOnAttackStartAction(PlayerStatusProvider.AttackType attackType, Vector2 direction)
    {
        // nothing to do
    }

    private void HandleOnAttackStopAction(PlayerStatusProvider.AttackType attackType, bool cancelled)
    {
        // nothing to do
    }

    //
    // HELPERS
    //

    private void SetChromaLightCurveValue(float curveValue)
    {
        float value;
        if(curveValue < 0.0f)
        {
            value = chromaLightNormal + curveValue * (chromaLightNormal - chromaLightMin);
        }
        else if(curveValue > 0.0f)
        {
            value = chromaLightNormal + curveValue * (chromaLightMax - chromaLightNormal);
        }
        else
        {
            value = chromaLightNormal;
        }

        bodyRenderer.material.SetColor("_BodyColor", playerInfo.BodyColor);
        bodyRenderer.material.SetColor("_ChromaColor", playerInfo.Color);
        bodyRenderer.material.SetFloat("_ChromaLightPct", Mathf.Clamp(value, 0.0f, 1.0f));
    }

    private void SetShadow()
    {
        bodyRenderer.material.SetColor("_BodyColor", Color.black);
        bodyRenderer.material.SetColor("_ChromaColor", Color.black);
        bodyRenderer.material.SetFloat("_ChromaLightPct", 0.0f);
    }

    private void SetOverlay(bool overlay)
    {
        if (overlay)
        {
            bodyRenderer.sortingLayerName = overlaySortingLayerName;
            bodyRenderer.sortingOrder = overlaySortingOrder;
        }
        else
        {
            bodyRenderer.sortingLayerName = originalSortingLayerName;
            bodyRenderer.sortingOrder = originalSortingOrder;
        }

        this.overlay = overlay;
    }
}

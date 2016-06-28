using UnityEngine;

public class PlayerFXManager : MonoBehaviour
{
    //
    // CHROMA
    //

    //public float bodyColorDarkPct = 0.1f;
    //public float bodyColorBrightPct = 0.1f;
    //public float chromaColorDarkPct = 1.0f;
    //public float chromaColorBrightPct = 0.9f;

    public float normalChromaLight = 0.6f;

    //
    // ATTACK
    //

    public AnimationCurve attackForwardChromaCurve;
    public AnimationCurve attackUpChromaCurve;
    public AnimationCurve attackDownChromaCurve;
    public AnimationCurve attackSpecialChromaCurve;
    public AnimationCurve attackCooldownChromaCurve;

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
    private SpriteRenderer bodyRenderer;
    private string originalSortingLayerName;
    private int originalSortingOrder;

    //
    // GLOBAL STATE
    //

  //  private Color bodyColorNormal;
   // private Color bodyColorMin;
    //private Color bodyColorMax;
  //  private Color chromaColorNormal;
   // private Color chromaColorMin;
  //  private Color chromaColorMax;

    // TODO use a state machine
    private bool overlay = false;
    private bool charge = false;

    private GameState.PlayerInfo playerInfo;

    
    //
    // INIT
    //
    
    void Awake()
    {
        statusProvider = transform.GetComponent<PlayerStatusProvider>();
        stateController = transform.GetComponent<PlayerStateController>();
        bodyRenderer = transform.Find("CharacterSprites").Find("Body").GetComponent<SpriteRenderer>();

        statusProvider.OnHorizontalKnockbackAction += HandleOnHorizontalKnockback;
        statusProvider.OnVerticalKnockbackAction += HandleOnVerticalKnockback;
        statusProvider.OnDieWarningAction += HandleOnDieWarning;
        statusProvider.OnDieAction += HandleOnDie;
        statusProvider.OnCollisionAction += HandleOnCollisionAction;
        statusProvider.OnAttackStartAction += HandleOnAttackStartAction;
        statusProvider.OnAttackStopAction += HandleOnAttackStopAction;
        statusProvider.OnChargeStartAction += HandleOnChargeStartAction;
        statusProvider.OnChargeStopAction += HandleOnChargeStopAction;
        statusProvider.OnChargeUpdateAction += HandleOnChargeUpdateAction;

        originalSortingLayerName = bodyRenderer.sortingLayerName;
        originalSortingOrder = bodyRenderer.sortingOrder;
    }

    void Start()
    {
        playerInfo = GameState.Instance.Player(stateController.GetPlayerId());

        //bodyColorNormal = playerInfo.BodyColor;
        //bodyColorMin = Color.Lerp(bodyColorNormal, Color.black, bodyColorDarkPct);
        //bodyColorMax = Color.Lerp(bodyColorNormal, Color.white, bodyColorBrightPct);

        //chromaColorNormal = playerInfo.Color;
        //chromaColorMin = Color.Lerp(chromaColorNormal, Color.black, chromaColorDarkPct);
        //chromaColorMax = Color.Lerp(chromaColorNormal, Color.white, chromaColorBrightPct);

        //   SetBodyColor(playerInfo.BodyColor);
        // SetChromaColor(playerInfo.Color);
        SetChromaLight(normalChromaLight);
    }

    void Update()
    {
        if (overlay || charge)
        {
            // don't change the sprite color while in overlay/charge mode
        }
        else if (stateController.GetAttackPct() > 0.0f)
        {
            switch (stateController.GetAimDirection())
            {
                case PlayerStateController.AimDirection.UP:
                    SetChromaLight(attackUpChromaCurve.Evaluate(stateController.GetAttackPct()));
           
                    break;
                case PlayerStateController.AimDirection.DOWN:
                    SetChromaLight(attackDownChromaCurve.Evaluate(stateController.GetAttackPct()));
                    break;
                case PlayerStateController.AimDirection.FORWARD:
                    SetChromaLight(attackForwardChromaCurve.Evaluate(stateController.GetAttackPct()));
                    break;
            }

        }
        else if (stateController.GetSpecialAttackPct() > 0.0f)
        {
            SetChromaLight(attackSpecialChromaCurve.Evaluate(stateController.GetSpecialAttackPct()));
        }
        else if (!stateController.IsCrystaled() && stateController.GetAttackCooldownPct() > 0.0f)
        {
            SetChromaLight(attackCooldownChromaCurve.Evaluate(stateController.GetAttackCooldownPct()));
        }
        else if(stateController.IsCrystaled())
        {
            SetChromaLight(0.2f);
            //SetBodyColor(bodyColorNormal);
            //Color.Lerp(chromaColorNormal, Color.black, 0.75f);
        }
        else
        {
            SetChromaLight(normalChromaLight);
           // SetBodyColor(bodyColorNormal);
            //SetChromaColor(chromaColorNormal);
        }
    }

    //
    // CHARGE
    //

    public float chargeLoadChromaMin = 0.05f;
    public float chargeLoadChromaMax = 0.5f;
    public float chargeReadyChromaMin = 0.55f;
    public float chargeReadyChromaMax = 1.0f;
    public float chargeOverflowBlinkInterval = 0.01f;
    public AnimationCurve chargeLoadChromaCurve;

    private float chargeOverflowBlinkCounter;
    private bool chargeOverflowBlinkVisible;

    private void HandleOnChargeStartAction()
    {
        charge = true;
        chargeOverflowBlinkCounter = 0.0f;
        chargeOverflowBlinkVisible = true;
    }

    private void HandleOnChargeStopAction(bool complete)
    {
        charge = false;
    }

    private void HandleOnChargeUpdateAction(PlayerStatusProvider.ChargeState state, float statePct, float forceRatio)
    {
        if (overlay)
        {
            return;
        }

        switch (state)
        {
            case PlayerStatusProvider.ChargeState.LOAD:
                SetChromaLight(chargeLoadChromaMin + chargeLoadChromaCurve.Evaluate(statePct) * (chargeLoadChromaMax - chargeLoadChromaMin));
                break;
            case PlayerStatusProvider.ChargeState.READY:
                SetChromaLight(chargeReadyChromaMin + forceRatio * (chargeReadyChromaMax - chargeReadyChromaMin));
                break;
            case PlayerStatusProvider.ChargeState.FULL:
                chargeOverflowBlinkCounter -= Time.deltaTime;
                if (chargeOverflowBlinkCounter <= 0.0)
                {
                    chargeOverflowBlinkVisible = !chargeOverflowBlinkVisible;
                    chargeOverflowBlinkCounter += chargeOverflowBlinkInterval;
                }
                SetChromaLight(chargeOverflowBlinkVisible ? chargeLoadChromaMax : -1.0f);
                break;
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
        Flash.Show(stateController.diePauseTime);
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
        if (attackType == PlayerStatusProvider.AttackType.SPECIAL)
        {
            float angle = Mathf.Sign(direction.y) * Vector2.Angle(direction.x < 0.0f ? Vector2.left : Vector2.right, direction);
            bodyRenderer.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            bodyRenderer.transform.parent.Translate(new Vector3(0.0f, 1.0f, 0.0f));
        }
    }

    private void HandleOnAttackStopAction(PlayerStatusProvider.AttackType attackType, bool cancelled)
    {
        if (attackType == PlayerStatusProvider.AttackType.SPECIAL)
        {
            bodyRenderer.transform.rotation = Quaternion.AngleAxis(0.0f, Vector3.forward);
            bodyRenderer.transform.parent.Translate(new Vector3(0.0f, -1.0f, 0.0f));
        }
    }

    //
    // HELPERS
    //

    private void SetChromaLight(float value)
    {
        bodyRenderer.material.SetColor("_BodyColor", playerInfo.BodyColor);
        bodyRenderer.material.SetColor("_ChromaColor", playerInfo.Color);
        bodyRenderer.material.SetFloat("_ChromaLight", Mathf.Clamp(value, 0.0f, 1.0f));
        //SetBodyColor(value);
        //SetChromaColor(value);
    }

    private void SetShadow()
    {
        bodyRenderer.material.SetColor("_BodyColor", Color.black);
        bodyRenderer.material.SetColor("_ChromaColor", Color.black);
        bodyRenderer.material.SetFloat("_ChromaLight", 0.0f);
    }

    //private void SetChromaColor(float value)
    //{
    //    if (value < 0.0f)
    //    {
    //        SetChromaColor(Color.Lerp(chromaColorNormal, chromaColorMin, Mathf.Min(1.0f, Mathf.Abs(value))));
    //    }
    //    else if (value > 0.0f)
    //    {
    //        SetChromaColor(Color.Lerp(chromaColorNormal, chromaColorMax, Mathf.Min(1.0f, value)));
    //    }
    //    else
    //    {
    //        SetChromaColor(chromaColorNormal);
    //    }
    //}

    //private void SetBodyColor(float value)
    //{
    //    if (value < 0.0f)
    //    {
    //        SetBodyColor(Color.Lerp(bodyColorNormal, bodyColorMin, Mathf.Min(1.0f, Mathf.Abs(value))));
    //    }
    //    else if (value > 0.0f)
    //    {
    //        SetBodyColor(Color.Lerp(bodyColorNormal, bodyColorMax, Mathf.Min(1.0f, value)));
    //    }
    //    else
    //    {
    //        SetBodyColor(chromaColorNormal);
    //    }
    //}

    //private void SetBodyColor(Color color)
    //{
    //    bodyRenderer.material.SetColor("_BodyColor", color);
    //}

    //private void SetChromaColor(Color color)
    //{
    //    bodyRenderer.material.SetColor("_ChromaColor", color);
    //}

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

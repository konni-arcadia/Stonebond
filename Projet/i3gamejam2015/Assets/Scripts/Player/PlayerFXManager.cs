using UnityEngine;

public class PlayerFXManager : MonoBehaviour
{
    // 
    // PARAMETERS
    //
    
    public float bodyColorDarkPct = 0.1f;
    public float bodyColorBrightPct = 0.1f;
    public float chromaColorDarkPct = 1.0f;
    public float chromaColorBrightPct = 0.9f;
    public AnimationCurve attackForwardChromaCurve;
    public AnimationCurve attackUpChromaCurve;
    public AnimationCurve attackDownChromaCurve;
    public AnimationCurve attackSpecialChromaCurve;
    public AnimationCurve chargeChromaCurve;
    public AnimationCurve attackCooldownChromaCurve;
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

    private Color bodyColorNormal;
    private Color bodyColorMin;
    private Color bodyColorMax;
    private Color chromaColorNormal;
    private Color chromaColorMin;
    private Color chromaColorMax;
    private bool overlay = false;

    
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
        statusProvider.OnHitGroundAction += HandleOnHitGroundAction;
        statusProvider.OnHitWallAction += HandleOnHitWallAction;

        originalSortingLayerName = bodyRenderer.sortingLayerName;
        originalSortingOrder = bodyRenderer.sortingOrder;
    }

    void Start()
    {
        GameState.PlayerInfo playerInfo = GameState.Instance.Player(stateController.GetPlayerId());

        bodyColorNormal = playerInfo.BodyColor;
        bodyColorMin = Color.Lerp(bodyColorNormal, Color.black, bodyColorDarkPct);
        bodyColorMax = Color.Lerp(bodyColorNormal, Color.white, bodyColorBrightPct);

        chromaColorNormal = playerInfo.Color;
        chromaColorMin = Color.Lerp(chromaColorNormal, Color.black, chromaColorDarkPct);
        chromaColorMax = Color.Lerp(chromaColorNormal, Color.white, chromaColorBrightPct);

        SetBodyColor(bodyColorNormal);
        SetChromaColor(Color.black);
    }

    void Update()
    {
        if (overlay)
        {
            // don't change the sprite color while in overlay mode
        }
        else if (stateController.GetAttackPct() > 0.0f)
        {
            //SetBodyColor (Color.Lerp (bodyColorMax, bodyColorMin, stateController.GetAttackPct ()));

            switch (stateController.GetAimDirection())
            {
                case PlayerStateController.AimDirection.UP:
                    SetBodyColor(stateController.GetAttackPct(), attackUpChromaCurve);
                    SetChromaColor(stateController.GetAttackPct(), attackUpChromaCurve);
                    break;
                case PlayerStateController.AimDirection.DOWN:
                    SetBodyColor(stateController.GetAttackPct(), attackDownChromaCurve);
                    SetChromaColor(stateController.GetAttackPct(), attackDownChromaCurve);
                    break;
                case PlayerStateController.AimDirection.FORWARD:
                    SetBodyColor(stateController.GetAttackPct(), attackForwardChromaCurve);
                    SetChromaColor(stateController.GetAttackPct(), attackForwardChromaCurve);
                    break;
            }

        }
        else if (stateController.GetChargePct() > 0.0f)
        {
            SetBodyColor(stateController.GetChargePct(), chargeChromaCurve);
            SetChromaColor(stateController.GetChargePct(), chargeChromaCurve);
        }
        else if (stateController.GetSpecialAttackPct() > 0.0f)
        {
            SetBodyColor(stateController.GetSpecialAttackPct(), attackSpecialChromaCurve);
            SetChromaColor(stateController.GetSpecialAttackPct(), attackSpecialChromaCurve);
        }
        else if (stateController.GetAttackCooldownPct() > 0.0f)
        {
            SetBodyColor(stateController.GetAttackCooldownPct(), attackCooldownChromaCurve);
            SetChromaColor(stateController.GetAttackCooldownPct(), attackCooldownChromaCurve);
        }
        else
        {
            SetBodyColor(bodyColorNormal);
            SetChromaColor(chromaColorNormal);
        }
    }

    //
    // EVENTS
    //

    private void HandleOnHorizontalKnockback()
    {
        ScreenShake.ShakeX(0.5f, 2.0f);
    }

    private void HandleOnVerticalKnockback()
    {
        ScreenShake.ShakeY(0.5f, 2.0f);
    }

    private void HandleOnDieWarning(Transform source, Vector2 attackDirection)
    {   
        ScreenShake.SetEnabled(false);
        Flash.flash();
        SetOverlay(true);
        SetBodyColor(Color.black);
        SetChromaColor(Color.black);

        PlayerFXManager sourceFXManager = source.GetComponent<PlayerFXManager>();
        sourceFXManager.SetOverlay(true);
        sourceFXManager.SetBodyColor(Color.black);
        sourceFXManager.SetChromaColor(Color.black);
    }

    private void HandleOnDie(Transform source, Vector2 attackDirection)
    {
        ScreenShake.SetEnabled(true);
        ScreenShake.ShakeXY(0.5f, 2.0f, 0.5f, 2.0f);
        SetOverlay(false);

        PlayerFXManager sourceFXManager = source.GetComponent<PlayerFXManager>();
        sourceFXManager.SetOverlay(false);
    }

    private void HandleOnHitWallAction(PlayerStatusProvider.WallCollisionType collisionType, Vector2 velocity)
    {
        if (collisionType == PlayerStatusProvider.WallCollisionType.SPECIAL_ATTACK)
        {
            ScreenShake.ShakeX(0.3f, 1.5f);
        }
        else if (collisionType == PlayerStatusProvider.WallCollisionType.ATTACK)
        {
            ScreenShake.ShakeX(0.15f, 1.5f);
        }
    }
    
    private void HandleOnHitGroundAction(PlayerStatusProvider.GroundCollisionType collisionType, Vector2 velocity)
    {
        if (collisionType == PlayerStatusProvider.GroundCollisionType.ATTACK)
        {
            ScreenShake.ShakeY(0.25f, 1.5f);
        }
    }

    //
    // HELPERS
    //

    private void SetChromaColor(float pct, AnimationCurve curve)
    {
        float value = curve.Evaluate(pct);
        if (value < 0.0f)
        {
            SetChromaColor(Color.Lerp(chromaColorNormal, chromaColorMin, Mathf.Min(1.0f, Mathf.Abs(value))));
        }
        else if (value > 0.0f)
        {
            SetChromaColor(Color.Lerp(chromaColorNormal, chromaColorMax, Mathf.Min(1.0f, value)));
        }
        else
        {
            SetChromaColor(chromaColorNormal);
        }
    }

    private void SetBodyColor(float pct, AnimationCurve curve)
    {
        float value = curve.Evaluate(pct);
        if (value < 0.0f)
        {
            SetBodyColor(Color.Lerp(bodyColorNormal, bodyColorMin, Mathf.Min(1.0f, Mathf.Abs(value))));
        }
        else if (value > 0.0f)
        {
            SetBodyColor(Color.Lerp(bodyColorNormal, bodyColorMax, Mathf.Min(1.0f, value)));
        }
        else
        {
            SetBodyColor(chromaColorNormal);
        }
    }

    private void SetBodyColor(Color color)
    {
        bodyRenderer.material.SetColor("_Color", color);
    }

    private void SetChromaColor(Color color)
    {
        bodyRenderer.material.SetColor("_ChromaTexColor", color);
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

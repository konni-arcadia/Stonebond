using UnityEngine;

public class PlayerFXManager : MonoBehaviour
{
	//
	// REFERENCES
	//

	private PlayerStatusProvider statusProvider;
	private PlayerStateController stateController;
	private SpriteRenderer bodyRenderer;

	// 
	// GLOBAL STATE
	//

	public float bodyColorDarkPct = 0.1f;
	public float bodyColorBrightPct = 0.1f;

	public float chromaColorDarkPct = 1.0f;
	public float chromaColorBrightPct = 0.9f;

	private Color bodyColorNormal;
	private Color bodyColorMin;
	private Color bodyColorMax;

	private Color chromaColorNormal;
	private Color chromaColorMin;
	private Color chromaColorMax;
	
	//
	// INIT
	//
	
	void Awake()
	{
		statusProvider = transform.GetComponent<PlayerStatusProvider>();
		stateController = transform.GetComponent<PlayerStateController> ();
		bodyRenderer = transform.Find ("CharacterSprites").Find("Body").GetComponent<SpriteRenderer>();

		statusProvider.OnHorizontalKnockbackAction += OnHorizontalKnockback;
		statusProvider.OnVerticalKnockbackAction += OnVerticalKnockback;
		statusProvider.OnDieAction += OnDie;
	}
	
	void Start()
	{
		GameState.PlayerInfo playerInfo = GameState.Instance.Player (stateController.GetPlayerId ());

		bodyColorNormal = playerInfo.BodyColor;
		bodyColorMin = Color.Lerp (bodyColorNormal, Color.black, bodyColorDarkPct);
		bodyColorMax = Color.Lerp (bodyColorNormal, Color.white, bodyColorBrightPct);

		chromaColorNormal = playerInfo.Color;
		chromaColorMin = Color.Lerp (chromaColorNormal, Color.black, chromaColorDarkPct);
		chromaColorMax = Color.Lerp (chromaColorNormal, Color.white, chromaColorBrightPct);

		SetBodyColor (bodyColorNormal);
		SetChromaColor (Color.black);
	}

	void Update()
	{
		if (stateController.GetAttackPct () > 0.0f)
		{
			SetBodyColor (Color.Lerp (bodyColorMax, bodyColorMin, stateController.GetAttackPct ()));
			SetChromaColor (Color.Lerp (chromaColorMax, chromaColorMin, stateController.GetAttackPct ()));
		}
		else if (stateController.GetChargePct () > 0.0f)
		{
			SetBodyColor (Color.Lerp (bodyColorNormal, bodyColorMax, stateController.GetChargePct ()));
			SetChromaColor (Color.Lerp (chromaColorNormal, chromaColorMax, stateController.GetChargePct ()));
		}
		else if (stateController.GetSpecialAttackPct () > 0.0f)
		{
			SetBodyColor (Color.Lerp (bodyColorMax, bodyColorMin, stateController.GetSpecialAttackPct ()));
			SetChromaColor (Color.Lerp (chromaColorMax, chromaColorMin, stateController.GetSpecialAttackPct ()));
		}
		else if (stateController.GetAttackCooldownPct() > 0.0f)
		{
			SetBodyColor (bodyColorMin);
			SetChromaColor (chromaColorMin);
		}
		else
		{
			SetBodyColor (bodyColorNormal);
			SetChromaColor (chromaColorNormal);
		}
	}

	void OnHorizontalKnockback() {
		ScreenShake.ShakeX (0.5f, 2.0f);
	}

	void OnVerticalKnockback() {
		ScreenShake.ShakeY (0.5f, 2.0f);
	}

	void OnDie(Vector2 attackDirection) {	
		Flash.flash ();
		SlowMotion.slowMotion ();
		ScreenShake.ShakeXY (0.5f, 2.0f, 0.5f, 2.0f);
	}

	//
	// HELPERS
	//

	private void SetBodyColor(Color color) {
		bodyRenderer.material.SetColor("_Color", color);
	}

	private void SetChromaColor(Color color) {
		bodyRenderer.material.SetColor("_ChromaTexColor", color);
	}
}

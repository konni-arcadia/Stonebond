using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WinChar : MonoBehaviour {
	public float bodyColorDarkPct = 0.1f;
	public float bodyColorBrightPct = 0.1f;
	public float chromaColorDarkPct = 1.0f;
	public float chromaColorBrightPct = 0.9f;
	[SerializeField] private Image winCharImg;
    [SerializeField] public Image winTextImg;
    public int playerId = 1;
	private Color bodyColorNormal;
	//private Color bodyColorMin;
	//private Color bodyColorMax;
	private Color chromaColorNormal;
	//private Color chromaColorMin;
	//private Color chromaColorMax;
	// Use this for initialization
	public void UpdateColor () {
		GameState.PlayerInfo playerInfo = GameState.Instance.Player(playerId);

		bodyColorNormal = playerInfo.BodyColor;
		//bodyColorMin = Color.Lerp(bodyColorNormal, Color.black, bodyColorDarkPct);
		//bodyColorMax = Color.Lerp(bodyColorNormal, Color.white, bodyColorBrightPct);

		chromaColorNormal = playerInfo.Color;
		//chromaColorMin = Color.Lerp(chromaColorNormal, Color.black, chromaColorDarkPct);
		//chromaColorMax = Color.Lerp(chromaColorNormal, Color.white, chromaColorBrightPct);

		SetBodyColor(bodyColorNormal);
		SetChromaColor(chromaColorNormal);
	}
	

	private void SetBodyColor(Color color)
	{
		winCharImg.material.SetColor("_Color", color);
	}

	private void SetChromaColor(Color color)
	{
		winCharImg.material.SetColor("_ChromaTexColor", color);
	}
	public void HideCharacter()
	{
		winCharImg.enabled = false;
        winTextImg.enabled = false;
	}

}

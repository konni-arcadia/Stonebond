using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {
	private const float AxisDeadZone = 0.6f;
	// Buttons
	public const string A = "Jump";
    public const string START = "START";
	public const string BUTTON_ATTACK = "Attack";
    public const string B = "Cancel";

	// Axis
	public const string Horizontal = "Horizontal";
	public const string Vertical = "Vertical";
	public const string DpadHorizontal = "DpadHorizontal";
	public const string DpadVertical = "DpadVertical";

	void Start () {
	}
	
	void Update () {
	}

	public float AxisValue(int playerId, string axisName) {
		int controllerId = GameState.Instance.Player(playerId).ControllerId;
/*		float value = Input.GetAxis(axisName + playerId);
		if (value > 0) return Mathf.Max(0, value - AxisDeadZone) / (1 - AxisDeadZone);
		else return Mathf.Min(0, value + AxisDeadZone) / (1 - AxisDeadZone);*/
		return Input.GetAxis(axisName + controllerId);
	}
	
	public bool IsHeld(int playerId, string keyName) {
		int controllerId = GameState.Instance.Player(playerId).ControllerId;
		return Input.GetButton(keyName + " P" + controllerId);
	}

	public bool WasPressed(int playerId, string keyName) {
		int controllerId = GameState.Instance.Player(playerId).ControllerId;
		return Input.GetButtonDown(keyName + " P" + controllerId);
	}
}

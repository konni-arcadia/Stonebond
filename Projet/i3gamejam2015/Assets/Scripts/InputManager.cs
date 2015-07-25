using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {
	private const float AxisDeadZone = 0.6f;
	// Buttons
	public const string A = "Jump";
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
/*		float value = Input.GetAxis(axisName + playerId);
		if (value > 0) return Mathf.Max(0, value - AxisDeadZone) / (1 - AxisDeadZone);
		else return Mathf.Min(0, value + AxisDeadZone) / (1 - AxisDeadZone);*/
		return Input.GetAxis(axisName + playerId);
	}
	
	public bool IsHeld(int playerId, string keyName) {
		return Input.GetButtonDown(keyName + " P" + playerId);
	}
}

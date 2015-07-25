using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

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
		return Input.GetAxis(axisName + playerId);
	}
	
	public bool IsHeld(int playerId, string keyName) {
		return Input.GetButtonDown(keyName + " P" + playerId);
	}
}

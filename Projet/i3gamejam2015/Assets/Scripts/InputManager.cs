using UnityEngine;
using System.Collections;


public class InputManager : Singleton<InputManager> {
	//private const float AxisDeadZone = 0.6f;
	public const float AxisDeadZone = 0.1f;
	// Buttons
	public const string A = "Jump";
	public const string START = "START";
	public const string BUTTON_ATTACK = "Attack";
	public const string BUTTON_CHARGE = "Charge";
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

	public float AxisValue(int playerId, string axisName)
	{
		int controllerId = GameState.Instance.Player(playerId).ControllerId;
		return AxisValueCtrl (controllerId, axisName);
	}

	public bool WasPressed(int playerId, string keyName)
	{
		int controllerId = GameState.Instance.Player(playerId).ControllerId;
		return WasPressedCtrl (controllerId, keyName);
	}
	private float CapAxis(float axisValue)
	{
		if (Mathf.Abs (axisValue) > 1.0f)
			axisValue = axisValue / Mathf.Abs (axisValue);
		return axisValue;
	}
	public float AxisValueCtrl(int controllerId, string axisName) {
		float axisValue = 0.0f;
		if (InControl.InputManager.IsSetup) {
			var inputDevice = (InControl.InputManager.Devices.Count > controllerId - 1) ? InControl.InputManager.Devices [controllerId - 1] : null;
			if (inputDevice != null) {
				switch (axisName) {
				case InputManager.Horizontal:
					axisValue += inputDevice.LeftStickX;
					break;
				case InputManager.Vertical:
					axisValue -= inputDevice.LeftStickY;
					break;
				}
			}
		} else {
			//Debug.Log ("InControl Input Manager missing, contact Elias.");
		}
		axisValue += Input.GetAxis (axisName + controllerId);
		
		return CapAxis(axisValue);
	}
	
	public bool IsHeld(int playerId, string keyName) {
		int controllerId = GameState.Instance.Player(playerId).ControllerId;
		bool buttonValue = false;
		if (InControl.InputManager.IsSetup) {
			var inputDevice = (InControl.InputManager.Devices.Count > controllerId - 1) ? InControl.InputManager.Devices [controllerId - 1] : null;
			if (inputDevice != null) {
				switch (keyName) {
				case InputManager.A: 
					buttonValue = buttonValue || inputDevice.Action1.IsPressed;
					break;
				case InputManager.B:
					buttonValue = buttonValue || inputDevice.Action2.IsPressed;
					break;
				case InputManager.BUTTON_ATTACK:
					buttonValue = buttonValue || inputDevice.Action3.IsPressed;
					break;
				case InputManager.BUTTON_CHARGE:
					buttonValue = buttonValue || inputDevice.Action4.IsPressed || inputDevice.Action2.IsPressed;
					break;
				}
			}
		} else {
			Debug.Log ("InControl Input Manager missing, contact Elias.");
		}
		buttonValue = buttonValue || Input.GetButton (keyName + " P" + controllerId);
		return buttonValue;
	}

	public bool WasPressedCtrl(int controllerId, string keyName) {
		
		bool buttonValue = false;
		if (InControl.InputManager.IsSetup) {
			var inputDevice = (InControl.InputManager.Devices.Count > controllerId - 1) ? InControl.InputManager.Devices [controllerId - 1] : null;
			if (inputDevice != null) {
				switch (keyName) {
				case InputManager.A: 
					buttonValue = buttonValue || inputDevice.Action1.WasPressed;
					break;
				case InputManager.B:
					buttonValue = buttonValue || inputDevice.Action2.WasPressed;
					break;
				case InputManager.BUTTON_ATTACK:
					buttonValue = buttonValue || inputDevice.Action3.WasPressed;
					break;
				case InputManager.BUTTON_CHARGE:
					buttonValue = buttonValue || inputDevice.Action4.WasPressed || inputDevice.Action2.WasPressed;
					break;
				case InputManager.START:
					buttonValue = buttonValue || inputDevice.MenuWasPressed;
					break;
				
				}
			}
		} else {
			//Debug.Log ("InControl Input Manager missing, contact Elias.");
		}
		buttonValue = buttonValue || Input.GetButtonDown (keyName + " P" + controllerId);
		return buttonValue;
	}
}

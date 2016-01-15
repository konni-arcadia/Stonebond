using UnityEngine;
using System.Collections;


public class InputManager : MonoBehaviour {
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
	public float AxisValueCtrl(int controllerId, string axisName) {
		
		/*		float value = Input.GetAxis(axisName + playerId);
		if (value > 0) return Mathf.Max(0, value - AxisDeadZone) / (1 - AxisDeadZone);
		else return Mathf.Min(0, value + AxisDeadZone) / (1 - AxisDeadZone);*/
		float axisValue = 0.0f;
		var inputDevice = (InControl.InputManager.Devices.Count > controllerId-1) ? InControl.InputManager.Devices[controllerId-1] : null;
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
		axisValue += Input.GetAxis (axisName + controllerId);
		
		return axisValue;
	}
	
	public bool IsHeld(int playerId, string keyName) {
		int controllerId = GameState.Instance.Player(playerId).ControllerId;
		bool axisValue = false;
		var inputDevice = (InControl.InputManager.Devices.Count > controllerId-1) ? InControl.InputManager.Devices[controllerId-1] : null;
		if (inputDevice != null) {
			switch (keyName) {
			case InputManager.A: 
				axisValue = axisValue || inputDevice.Action1.IsPressed;
				break;
			case InputManager.B:
				axisValue = axisValue || inputDevice.Action2.IsPressed;
				break;
			case InputManager.BUTTON_ATTACK:
				axisValue = axisValue || inputDevice.Action3.IsPressed;
				break;
			case InputManager.BUTTON_CHARGE:
				axisValue = axisValue || inputDevice.Action4.IsPressed;
				break;
			}
		}
		axisValue = axisValue || Input.GetButton (keyName + " P" + controllerId);
		return axisValue;
	}
	public bool WasPressed(int playerId, string keyName)
	{
		int controllerId = GameState.Instance.Player(playerId).ControllerId;
		return WasPressedCtrl (controllerId, keyName);
	}
	public bool WasPressedCtrl(int controllerId, string keyName) {
		
		bool axisValue = false;
		var inputDevice = (InControl.InputManager.Devices.Count > controllerId-1) ? InControl.InputManager.Devices[controllerId-1] : null;
		if (inputDevice != null) {
			switch (keyName) {
			case InputManager.A: 
				axisValue = axisValue || inputDevice.Action1.WasPressed;
				break;
			case InputManager.B:
				axisValue = axisValue || inputDevice.Action2.WasPressed;
				break;
			case InputManager.BUTTON_ATTACK:
				axisValue = axisValue || inputDevice.Action3.WasPressed;
				break;
            case InputManager.BUTTON_CHARGE:
                axisValue = axisValue || inputDevice.Action4.WasPressed;
                break;
			case InputManager.START:
				axisValue = axisValue || inputDevice.MenuWasPressed;
				break;
				
			}
		}
		axisValue = axisValue || Input.GetButtonDown (keyName + " P" + controllerId);
		return axisValue;
	}
}

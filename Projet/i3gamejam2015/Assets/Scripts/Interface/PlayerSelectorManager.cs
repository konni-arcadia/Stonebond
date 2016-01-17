using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerSelectorManager : MonoBehaviour {

	float waitBeforeAllowingSelection;
    public int PlayerNumber = 1;
    public GameObject PlayerSelection;
    //public Text Name;
    public Text PressStart;
    public SpriteRenderer Player;
    public Text Ready;

    public bool HasChoosen = false;
    public bool HasPressedStart = false;

	public GameObject playableChar;

	InputManager inputManager;
	// Use this for initialization
	void Start () {
        
		inputManager = GetComponent<InputManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (waitBeforeAllowingSelection > 0) {
			waitBeforeAllowingSelection -= Time.deltaTime;
			return;
		}
		CheckPlayerController();
	}

    void CheckPlayerController()
    {
        if (!HasChoosen)
        {
			// Allow action button too for testing
			if (HasPressedStart && inputManager.WasPressed(PlayerNumber, InputManager.START))
            {
				
                HasChoosen = true;
                Ready.enabled = true;
            }
        }
    }

	// Called from PlayersSelectorManager
    public void SetChoosenState()
    {
		playableChar.SetActive (true);
		AudioSingleton<SfxAudioManager>.Instance.PlayValidate();
        HasPressedStart = true;
        PressStart.enabled = false;
        //Name.enabled = true;
        PlayerSelection.SetActive(true);
        Ready.enabled = false;
		// Do not allow validating the second time immediately (READY)
		waitBeforeAllowingSelection = 0.2f;
    }

    public void SetInitialState()
    {
        PlayerSelection.SetActive(false);
        //Name.enabled = false;
        Ready.enabled = false;
    }
}

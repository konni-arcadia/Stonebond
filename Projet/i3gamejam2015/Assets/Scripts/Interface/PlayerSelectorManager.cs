using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerSelectorManager : MonoBehaviour {

    public int PlayerNumber = 1;
    public GameObject PlayerSelection;
    //public Text Name;
    public Text PressStart;
    public SpriteRenderer Player;
    public Text Ready;

    public bool HasChoosen = false;
    public bool HasPressedStart = false;

	InputManager inputManager;
	// Use this for initialization
	void Start () {
        
		inputManager = GetComponent<InputManager> ();
	}
	
	// Update is called once per frame
	void Update () {
        CheckPlayerController();
	}

    void CheckPlayerController()
    {
        if (!HasChoosen)
        {
			// Allow action button too for testing
			if (!HasPressedStart && (inputManager.WasPressed(PlayerNumber, InputManager.START) || inputManager.WasPressed(PlayerNumber, InputManager.A)))
            {
                SetChoosenState();
            }
			else if (HasPressedStart && inputManager.WasPressed(PlayerNumber, InputManager.A))
            {
                HasChoosen = true;
                Ready.enabled = true;
            }
        }
    }

    public void SetChoosenState()
    {
        SoundManager.Instance.Validate_Play();
        HasPressedStart = true;
        PressStart.enabled = false;
        //Name.enabled = true;
        PlayerSelection.SetActive(true);
        Ready.enabled = false;
    }

    public void SetInitialState()
    {
        SoundManager.Instance.Validate_Play();
        PlayerSelection.SetActive(false);
        //Name.enabled = false;
        Ready.enabled = false;
    }
}

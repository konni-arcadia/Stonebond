using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerSelectorManager : MonoBehaviour {

    public int PlayerNumber = 1;
    public GameObject PlayerSelection;
    public Text Name;
    public Text PressStart;
    public SpriteRenderer Player;
    public Text Ready;

    public bool HasChoosen = false;
    public bool HasPressedStart = false;

	// Use this for initialization
	void Start () {
        
	
	}
	
	// Update is called once per frame
	void Update () {
        CheckPlayerController();
	}

    void CheckPlayerController()
    {
        if (!HasChoosen)
        {
            if (!HasPressedStart && Input.GetButtonDown(InputManager.START + " P" + PlayerNumber))
            {
                SetChoosenState();
            }
            else if (HasPressedStart && Input.GetButtonDown(InputManager.A + " P" + PlayerNumber))
            {
                HasChoosen = true;
                Ready.enabled = true;
            }
        }
    }

    public void SetChoosenState()
    {
        HasPressedStart = true;
        PressStart.enabled = false;
        Name.enabled = true;
        PlayerSelection.SetActive(true);
        Ready.enabled = false;
    }

    public void SetInitialState()
    {
        PlayerSelection.SetActive(false);
        Name.enabled = false;
        Ready.enabled = false;
    }
}

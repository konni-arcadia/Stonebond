using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayersSelectorManager : MonoBehaviour {

    private PlayerSelectorManager[] listPlayerSelector;
    public int minPlayerNeeded = 3;
	public int alreadySelectedPlayers = 0;

    public const string PlayerSeleted = "PlayerSelected";
	private InputManager inputManager;
	private bool[] selectedControllers;

	// Use this for initialization
	void Start () {
		inputManager = GetComponent<InputManager> ();
        listPlayerSelector = GetComponentsInChildren<PlayerSelectorManager>();

        InitializePlayerSelection();
		AudioSingleton<VoiceAudioManager>.Instance.SelectCharacterPlay();
		AudioSingleton<MenuAudioManager>.Instance.SetSelectCharacterSnapshot();

		selectedControllers = new bool[listPlayerSelector.Length];
	}
	
	// Update is called once per frame
	void Update () {
		int nbReady = 0;
		bool canStart = true;
		int nbChosen = 0;
		for (int i = 0; i < listPlayerSelector.Length; i++) {
			
			CheckGoBackPrevious(i + 1);
			if (listPlayerSelector [i].HasChoosen) {
				nbChosen++;
			}
			if (listPlayerSelector[i].HasPressedStart) {
				if (listPlayerSelector[i].HasChoosen) {
					nbReady++;
				}
				else {
					canStart = false;
				}
			}
		}

		if (canStart && (nbReady >= minPlayerNeeded) && (nbReady == nbChosen)) {
			
			string playerReadyString = "";

			for (int i = 0; i < listPlayerSelector.Length; i++)
				if (listPlayerSelector[i].HasChoosen)
					playerReadyString += "1";
				else
					playerReadyString += "0";

			// Assign remaining controllers to non chosen gargoyles
			for (; alreadySelectedPlayers < GameState.Instance.NumPlayers; alreadySelectedPlayers++) {
				GameState.Instance.Player(alreadySelectedPlayers + 1).ControllerId = PickNextAvailableController();
			}

			PlayerPrefs.SetString(PlayerSeleted, playerReadyString);
			SceneManager.LoadSceneAsync("SelectLvl", LoadSceneMode.Additive);

			Destroy(gameObject);
		}

		// Check sub-players controllers
		for (int i = 0; i < selectedControllers.Length; i++) {
			CheckControllerPressedStart(i + 1);
			// Already selected
			if (selectedControllers[i]) continue;
			// Not selected yet, could assign the next gargoyle
			if (CheckControllerPressedStart(i + 1)) {
				// Next gargoyle (1, 2, 3, 4) is now choosen
				listPlayerSelector[alreadySelectedPlayers].SetChoosenState();
				// The controller that pressed start is associated to the next gargoyle
				GameState.Instance.Player(alreadySelectedPlayers + 1).ControllerId = i + 1;
				// And the controller is not selectable anymore
				selectedControllers[i] = true;
				alreadySelectedPlayers += 1;
				return;
			}
		}



        
	}

    void CheckGoBackPrevious(int noControler)
    {
		if (inputManager.WasPressedCtrl(noControler, InputManager.B))
        {
            PlayerPrefs.SetString("ComeFromMenu", "");
			SceneManager.LoadSceneAsync("SelectOption", LoadSceneMode.Additive);
			AudioSingleton<SfxAudioManager>.Instance.PlayCancel();
            Destroy(gameObject);
        }
    }

	bool CheckControllerPressedStart(int noController) {
		// Allow action button too for testing
		if ((inputManager.WasPressedCtrl(noController, InputManager.START)||inputManager.WasPressedCtrl(noController, InputManager.A))) {
			Debug.Log("Controller " + noController + " was pressed");
			return true;
		}
		return false;
	}

    void InitializePlayerSelection() {
		// By default, assign controller 1 to player 1, 2 to player 2, etc. we'll change the order later
		GameState.Instance.ResetPlayerControllersAndScore();
		if (PlayerPrefs.HasKey(PlayerSeleted)) {
			char[] listPlayer = PlayerPrefs.GetString(PlayerSeleted).ToCharArray();
			for (int i = 0; i < listPlayerSelector.Length; i++) {
				if (listPlayer[i] == '0') {
					listPlayerSelector[i].SetInitialState();
				}
				else {
					listPlayerSelector[i].SetChoosenState();
				}
			}
		}
		else {

			for (int i = 0; i < listPlayerSelector.Length; i++) {
				listPlayerSelector[i].SetInitialState();
			}
		}
    }

	int PickNextAvailableController() {
		for (int i = 0; i < selectedControllers.Length; i++) {
			if (!selectedControllers[i]) {
				selectedControllers[i] = true;
				return i + 1;
			}
		}
		return 0;
	}
}

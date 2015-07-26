using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WinScreenManager : MonoBehaviour {

    enum StartMenuItem { Restart, LvlSelection, MenuSelection, Quit };
    private string dpadHorizontal = "Horizontal";
    private string dpadVertical = "Vertical";
    private Color32 highlithed = new Color32(107, 107, 107, 255);
    private Color32 normal = new Color32(0, 0, 0, 255);
    
    public GameObject StartButtonArea;
    private Outline[] buttonList;

    private StartMenuItem menuSelectedItem = StartMenuItem.Restart;

    private bool[] wasPressed = new bool[4];

    private bool isDisplayed = false;
    public GameObject menu;
	private float timeSinceStart = 0;
	public Sprite[] playerTextSprites;
	
	// TEMP TODO refactor end of game jam alert
	public static int IdOfWonP1 = 1, IdOfWonP2 = 2, IdOfLevelToRestartTo;

	// Use this for initialization
	void Start () {
        buttonList = StartButtonArea.GetComponentsInChildren<Outline>();

        menu.SetActive(false);
		transform.Find("P1").GetComponent<Image>().sprite = playerTextSprites[IdOfWonP1 - 1];
		transform.Find("P2").GetComponent<Image>().sprite = playerTextSprites[IdOfWonP2 - 1];
	}
	
	// Update is called once per frame
	void Update () {
		timeSinceStart += Time.deltaTime;
        for (int i = 1; i < 5; i++ )
            CheckControlerStartMenu(i);
	}

	void CheckControlerStartMenu(int noControler) {
		if (!isDisplayed) {
			// First time: display the overlay
			if (Input.GetButtonDown(InputManager.A + " P" + noControler) && timeSinceStart >= 1) {
				showScreen();
				return;
			}
		}
        else {
            if (Input.GetButtonDown(InputManager.A + " P" + noControler)) {
                switch (menuSelectedItem)
                {

                    case StartMenuItem.Quit: Application.Quit();
                        break;

					case StartMenuItem.Restart: Application.LoadLevel(IdOfLevelToRestartTo);
                                                break;

                    case StartMenuItem.LvlSelection: PlayerPrefs.SetInt("ComeFromLVL", 0); 
                                                Application.LoadLevel("Menu");
                                                break;

                    case StartMenuItem.MenuSelection: Application.LoadLevel("Menu");
                                                break;
                }
                SoundManager.Instance.Validate_Play();

            }

            if (!wasPressed[noControler - 1] && Input.GetAxis(dpadVertical + noControler) < 0)
            {
                if (menuSelectedItem != (StartMenuItem)0)
                {
                    buttonList[(int)menuSelectedItem].effectColor = normal;
                    menuSelectedItem -= 1;
                    buttonList[(int)menuSelectedItem].effectColor = highlithed;
                    wasPressed[noControler - 1] = true;
                    SoundManager.Instance.Cursor_Play();
                }

            }
            else if (!wasPressed[noControler - 1] && Input.GetAxis(dpadVertical + noControler) > 0)
            {
                if (menuSelectedItem != StartMenuItem.Quit)
                {
                    buttonList[(int)menuSelectedItem].effectColor = normal;
                    menuSelectedItem += 1;
                    buttonList[(int)menuSelectedItem].effectColor = highlithed;
                    wasPressed[noControler - 1] = true;
                    SoundManager.Instance.Cursor_Play();
                }

            }
            else if (Input.GetAxis(dpadVertical + noControler) == 0 && wasPressed[noControler - 1])
            {
                wasPressed[noControler - 1] = false;
            }
        }
    }

	// Call this when a player has won
	private void showScreen() {
		isDisplayed = true;
		menu.SetActive(true);
	}
}

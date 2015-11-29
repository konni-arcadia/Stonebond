﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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

    private bool isMenuDisplayed = false, isSceneDisplayed = false;
    public GameObject menu;
	public Canvas canvas;
	private float timeSinceStart;
	private float minTimeBeforeShowingMenu = 1;
	public Sprite[] playerTextSprites;

	private InputManager inputManager;
	
    public Image Winner1;
    public Image Winner2;
    public Image Looser3;
    public Image Looser4;

    public List<Text> Scores;

	// TEMP TODO refactor end of game jam alert
	public int IdOfWonP1 = 1, IdOfWonP2 = 2, IdOfLevelToRestartTo;

	// Use this for initialization
	void Start () {

		inputManager = GetComponent<InputManager> ();
		// NOTE: instantiated at the very beginning of the game

		buttonList = StartButtonArea.GetComponentsInChildren<Outline>(true);

		canvas.enabled = false;
        menu.SetActive(false);
		//transform.Find("P1").GetComponent<Image>().sprite = playerTextSprites[IdOfWonP1 - 1];
		//transform.Find("P2").GetComponent<Image>().sprite = playerTextSprites[IdOfWonP2 - 1];
	}

	// Update is called once per frame
	void Update () 
	{
		// Only updated when visible
		if (!isSceneDisplayed)
			return;

		// Increase time since shown
		// Time.deltaTime can't be used here because of Time.timeScale set to 0 to pause the game.
		timeSinceStart += Time.unscaledDeltaTime;

		// Check all controllers inputs
        for (int i = 1; i < 5; i++ )
            CheckControlerStartMenu(i);
	}

	void CheckControlerStartMenu(int noControler)
	{
		if (!isMenuDisplayed)
		{
			// First time: display the overlay (but only after a certain time)
			if ((inputManager.WasPressedCtrl(noControler, InputManager.A) 
			  || inputManager.WasPressedCtrl(noControler, InputManager.START)) 
			  && timeSinceStart > minTimeBeforeShowingMenu)
			{
				isMenuDisplayed = true;
				menu.SetActive(true);
				return;
			}
		}
        else 
		{
			if (inputManager.WasPressedCtrl(noControler, InputManager.A) 
			 || inputManager.WasPressedCtrl(noControler, InputManager.START)) 
			{
				GameObject InControlObject = GameObject.Find("InControl");

                switch (menuSelectedItem)
                {

                    case StartMenuItem.Quit: Application.Quit();
                        break;

				case StartMenuItem.Restart: if(InControlObject != null)
												Destroy(InControlObject);
											Application.LoadLevel(IdOfLevelToRestartTo);
												
                                               break;

                    case StartMenuItem.LvlSelection: PlayerPrefs.SetInt("ComeFromLVL", 0);
												if(InControlObject != null)
													Destroy(InControlObject);
                                                Application.LoadLevel("Menu");

                                                break;

				case StartMenuItem.MenuSelection: if(InControlObject != null)
													Destroy(InControlObject);
												Application.LoadLevel("Menu");
																				
                                                break;
                }
                Time.timeScale = 1.0f;
                SoundManager.Instance.Validate_Play();
            }

			// Select previous button
			if (!wasPressed[noControler - 1] && inputManager.AxisValueCtrl(noControler, InputManager.Vertical) < -InputManager.AxisDeadZone)
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
			// Select next button
			else if (!wasPressed[noControler - 1] && inputManager.AxisValueCtrl(noControler, InputManager.Vertical) > InputManager.AxisDeadZone)
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
			else if (inputManager.AxisValueCtrl(noControler, InputManager.Vertical) < InputManager.AxisDeadZone &&
			         inputManager.AxisValueCtrl(noControler, InputManager.Vertical) > -InputManager.AxisDeadZone  && wasPressed[noControler - 1])
            {
                wasPressed[noControler - 1] = false;
            }
        }
    }

	// Call this when a player has won
	public void showScreen() 
	{
        PauseManager pauseMenu = FindObjectOfType<PauseManager>();
        if (pauseMenu != null)
        {
            pauseMenu.RemovePauseScreen();
        }

		canvas.enabled = true;
		timeSinceStart = 0;
		isSceneDisplayed = true;
        Time.timeScale = 0.0f; // Stop the game

        Winner1.sprite = playerTextSprites[IdOfWonP1 - 1];
        Scores[0].text = GameState.Instance.Player(IdOfWonP1).TotalScore.ToString() ;
        Winner2.sprite = playerTextSprites[IdOfWonP2 - 1];
        Scores[1].text = GameState.Instance.Player(IdOfWonP2).TotalScore.ToString();

        List<int> idList = new List<int>(new int[] { 1, 2, 3, 4 });

        idList.Remove(IdOfWonP1);
        idList.Remove(IdOfWonP2);

        Looser3.sprite = playerTextSprites[idList[0] - 1];
        Scores[2].text = GameState.Instance.Player(idList[0]).TotalScore.ToString();
        Looser4.sprite = playerTextSprites[idList[1] - 1];
        Scores[3].text = GameState.Instance.Player(idList[1]).TotalScore.ToString();
    }
}

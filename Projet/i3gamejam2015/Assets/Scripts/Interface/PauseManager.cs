using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Audio;

public class PauseManager : MonoBehaviour {

    enum StartMenuItem { Resume, LvlSelection, MenuSelection, Quit };
    private Color32 highlithed = new Color32(107, 107, 107, 255);
    private Color32 normal = new Color32(0, 0, 0, 255);
    
    public GameObject StartButtonArea;
    private Outline[] buttonList;

    private StartMenuItem menuSelectedItem = StartMenuItem.Resume;

    private bool[] wasPressed = new bool[4];

    private bool isDisplayed = false;
    public Canvas menu;
	private InputManager inputManager;

	// Use this for initialization
	void Start () {
		inputManager = GetComponent<InputManager> ();
		buttonList = StartButtonArea.GetComponentsInChildren<Outline>(true);

        menu.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 1; i < 5; i++ )
            CheckControlerStartMenu(i);
	}

    void CheckControlerStartMenu(int noControler)
    {

        if (isDisplayed)
        {
			if (inputManager.WasPressedCtrl(noControler, InputManager.A))
            {
				GameObject InControlObject = GameObject.Find("InControl");
                switch (menuSelectedItem)
                {

                    case StartMenuItem.Quit: 
						// TODO create event for that
						//Reset the nice EQ in pause screen
						AudioSingleton<MusicAudioManager>.Instance.SetMainDefaultSnapshot();
						//Reset the music to none
						AudioSingleton<MusicAudioManager>.Instance.SetMusicDefaultSnapshot();
						//Set the Main menu music in background
						AudioSingleton<MenuAudioManager>.Instance.SetMainMenuSnapshot();
						Application.Quit();
                        break;

                    case StartMenuItem.Resume:
						// TODO create event for that
						AudioSingleton<MenuAudioManager>.Instance.SetDefaultSnapshot();
						//Reset the nice EQ in pause screen to go back to game state
						AudioSingleton<MusicAudioManager>.Instance.SetMainDefaultSnapshot();

						isDisplayed = false;
                                                menu.enabled = false;

                                                break;

                    case StartMenuItem.LvlSelection: 
						// TODO create event for that
						//Reset the nice EQ in pause screen
						AudioSingleton<MusicAudioManager>.Instance.SetMainDefaultSnapshot();
						//Reset the Music
						AudioSingleton<MusicAudioManager>.Instance.SetMusicDefaultSnapshot();
						//Reset the bound if it is still playing
						AudioSingleton<SfxAudioManager>.Instance.SetSfxDefaultSnapshot();
						//Set the level Snapshot
						AudioSingleton<MenuAudioManager>.Instance.SetSelectStageSnapshot();
						PlayerPrefs.SetInt("ComeFromLVL", 0);
						SceneManager.LoadScene("Menu");
						if(InControlObject != null)
							Destroy(InControlObject);
						break;

                    case StartMenuItem.MenuSelection:
						// TODO create event for that
						//Reset the nice EQ in pause screen
						AudioSingleton<MusicAudioManager>.Instance.SetMainDefaultSnapshot();
						//Reset the Music
						AudioSingleton<MusicAudioManager>.Instance.SetMusicDefaultSnapshot();
						//Set the Main menu music
						AudioSingleton<MenuAudioManager>.Instance.SetMainMenuSnapshot();
						SceneManager.LoadScene("Menu");
						if(InControlObject != null)
							Destroy(InControlObject);
						break;
                }
                Time.timeScale = 1.0f;
				AudioSingleton<SfxAudioManager>.Instance.PlayValidate();

            }
			else if (inputManager.WasPressedCtrl(noControler, InputManager.START))
            {
				AudioSingleton<MusicAudioManager>.Instance.SetMainDefaultSnapshot();
                isDisplayed = false;
                menu.enabled = false;
                Time.timeScale = 1.0f;
            }

			//float horizontalAxis = inputManager.AxisValueCtrl(noControler, InputManager.Horizontal);
			float verticalAxis = inputManager.AxisValueCtrl(noControler, InputManager.Vertical);
			if (!wasPressed[noControler - 1] && verticalAxis < -InputManager.AxisDeadZone)
            {
                if (menuSelectedItem != (StartMenuItem)0)
                {
                    buttonList[(int)menuSelectedItem].effectColor = normal;
                    menuSelectedItem -= 1;
                    buttonList[(int)menuSelectedItem].effectColor = highlithed;
                    wasPressed[noControler - 1] = true;
					AudioSingleton<SfxAudioManager>.Instance.PlayCursor();
                }

            }
			else if (!wasPressed[noControler - 1] && verticalAxis > InputManager.AxisDeadZone)
            {
                if (menuSelectedItem != StartMenuItem.Quit)
                {
                    buttonList[(int)menuSelectedItem].effectColor = normal;
                    menuSelectedItem += 1;
                    buttonList[(int)menuSelectedItem].effectColor = highlithed;
                    wasPressed[noControler - 1] = true;
					AudioSingleton<SfxAudioManager>.Instance.PlayCursor();
                }

            }
			else if (verticalAxis < InputManager.AxisDeadZone &&
			         verticalAxis > -InputManager.AxisDeadZone && 
			         wasPressed[noControler - 1])
            {
                wasPressed[noControler - 1] = false;
            }
        }
        else
        {
			if (inputManager.WasPressedCtrl( noControler, InputManager.START))
            {
				// TODO create event for that
				AudioSingleton<MenuAudioManager>.Instance.SetDefaultSnapshot();
				AudioSingleton<MusicAudioManager>.Instance.SetPauseSnapshot();

                isDisplayed = true;
                menu.enabled = true;
                Time.timeScale = 0.0f;
            }
        }
    }

    public void RemovePauseScreen()
    {
        Destroy(gameObject);
    }
}

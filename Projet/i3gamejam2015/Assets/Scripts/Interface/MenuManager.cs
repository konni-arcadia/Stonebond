using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    enum StartMenuItem { GameStart, HowTo, Credits, Quit };
    private Color32 highlithed = new Color32(107, 107, 107, 255);
    private Color32 normal = new Color32(0, 0, 0, 255);
    
    public GameObject PressStartButtonArea, StartButtonArea;
    private Outline[] buttonList;

    private StartMenuItem menuSelectedItem = StartMenuItem.GameStart;

    private bool[] wasPressed = new bool[4];
	private InputManager inputManager;
	private bool pressStartActive = false;

	// Use this for initialization
	void Start () {
		inputManager = GetComponent<InputManager> ();
        buttonList = StartButtonArea.GetComponentsInChildren<Outline>(true);

        
		AudioSingleton<MenuAudioManager>.Instance.SetMainMenuSnapshot();


        if (PlayerPrefs.HasKey("ComeFromMenu"))
        {
            PressStartButtonArea.SetActive(false);
            StartButtonArea.SetActive(true);
        }
        else
        {
            StartButtonArea.SetActive(false);
            PressStartButtonArea.SetActive(true);
            StartCoroutine(ShowPressStartAfterDelay());
        }

        PlayerPrefs.DeleteAll();
    }
	
	// Update is called once per frame
	void Update () {
        for (int i = 1; i < 5; i++ )
            CheckControlerStartMenu(i);
	}

	IEnumerator ShowPressStartAfterDelay() {
		pressStartActive = true;
		while (pressStartActive) {
			PressStartButtonArea.SetActive(true);
			yield return new WaitForSeconds(1);
			PressStartButtonArea.SetActive(false);
			yield return new WaitForSeconds(0.5f);
		}
	}

	IEnumerator ShowMenuAfterDelay() {
		PressStartButtonArea.SetActive(false);
		AudioSingleton<VoiceAudioManager>.Instance.PlayTitle();
		yield return new WaitForSeconds(0.2f);
		StartButtonArea.SetActive(true);
	}

    void CheckControlerStartMenu(int noControler)
    {
		if (inputManager.WasPressedCtrl(noControler,InputManager.A) || inputManager.WasPressedCtrl(noControler, InputManager.START))
        {
			if (!StartButtonArea.activeSelf) {
				pressStartActive = false;
				StartCoroutine(ShowMenuAfterDelay());
				return;
			}
			else {
				switch (menuSelectedItem) {
					case StartMenuItem.Quit:
						Application.Quit();
						break;

					case StartMenuItem.GameStart:
						SceneManager.LoadSceneAsync("SelectPlayers", LoadSceneMode.Additive);
						AudioSingleton<MenuAudioManager>.Instance.SetDefaultSnapshot();
						break;

					case StartMenuItem.Credits:
						SceneManager.LoadSceneAsync("Credits", LoadSceneMode.Additive);
						AudioSingleton<MenuAudioManager>.Instance.SetDefaultSnapshot();
						break;

					case StartMenuItem.HowTo:
						return;
				}
				AudioSingleton<SfxAudioManager>.Instance.PlayValidate();
				Destroy(gameObject);
			}            
        }


        if (StartButtonArea.activeSelf)
        {
			if (!wasPressed[noControler - 1] && inputManager.AxisValueCtrl(noControler, InputManager.Vertical) < -InputManager.AxisDeadZone)
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
            else if (!wasPressed[noControler - 1] && inputManager.AxisValueCtrl(noControler, InputManager.Vertical) > InputManager.AxisDeadZone)
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
            else if (inputManager.AxisValueCtrl(noControler, InputManager.Vertical) < InputManager.AxisDeadZone &&
                     inputManager.AxisValueCtrl(noControler, InputManager.Vertical) > -InputManager.AxisDeadZone && wasPressed[noControler - 1])
            {
                wasPressed[noControler - 1] = false;
            }
        }
    }
}

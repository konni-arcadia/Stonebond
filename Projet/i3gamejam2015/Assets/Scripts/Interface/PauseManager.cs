using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour {

    enum StartMenuItem { Resume, LvlSelection, MenuSelection, Quit };
    private string dpadHorizontal = "Horizontal";
    private string dpadVertical = "Vertical";
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
						if(SoundManager.Instance != null) SoundManager.Instance.TriggerMenuBack();
						Application.Quit();
                        break;

                    case StartMenuItem.Resume:
						// TODO create event for that
						if(SoundManager.Instance != null) SoundManager.Instance.TriggerResume();
						isDisplayed = false;
                                                menu.enabled = false;

                                                break;

                    case StartMenuItem.LvlSelection: 
						// TODO create event for that
						if(SoundManager.Instance != null) SoundManager.Instance.TriggerMenuBack();
						PlayerPrefs.SetInt("ComeFromLVL", 0); 
                                                Application.LoadLevel("Menu");
												if(InControlObject != null)
													Destroy(InControlObject);
                                                break;

                    case StartMenuItem.MenuSelection:
						// TODO create event for that
						if(SoundManager.Instance != null) SoundManager.Instance.TriggerMenuBack();
						Application.LoadLevel("Menu");
												if(InControlObject != null)
													Destroy(InControlObject);
                                                break;
                }
                Time.timeScale = 1.0f;
                SoundManager.Instance.Validate_Play();

            }
			else if (inputManager.WasPressedCtrl(noControler, InputManager.START))
            {
                isDisplayed = false;
                menu.enabled = false;
                Time.timeScale = 1.0f;
            }

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
			         inputManager.AxisValueCtrl(noControler, InputManager.Vertical) > -InputManager.AxisDeadZone && 
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
				if(SoundManager.Instance != null) SoundManager.Instance.TriggerPause();

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

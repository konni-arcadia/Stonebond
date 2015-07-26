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


	// Use this for initialization
	void Start () {
        buttonList = StartButtonArea.GetComponentsInChildren<Outline>();

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
            if (Input.GetButtonDown(InputManager.START + " P" + noControler))
            {
                switch (menuSelectedItem)
                {

                    case StartMenuItem.Quit: Application.Quit();
                        break;

                    case StartMenuItem.Resume: isDisplayed = false;
                                                menu.enabled = false;
                                                Time.timeScale = 1.0f;
                                                break;

                    case StartMenuItem.LvlSelection: PlayerPrefs.SetInt("ComeFromLVL", 0); 
                                                Application.LoadLevel("Menu");
                                                break;

                    case StartMenuItem.MenuSelection: Application.LoadLevel("Menu");
                                                break;
                }

            }

            if (!wasPressed[noControler - 1] && Input.GetAxis(dpadVertical + noControler) < 0)
            {
                if (menuSelectedItem != (StartMenuItem)0)
                {
                    buttonList[(int)menuSelectedItem].effectColor = normal;
                    menuSelectedItem -= 1;
                    buttonList[(int)menuSelectedItem].effectColor = highlithed;
                    wasPressed[noControler - 1] = true;
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
                }

            }
            else if (Input.GetAxis(dpadVertical + noControler) == 0 && wasPressed[noControler - 1])
            {
                wasPressed[noControler - 1] = false;
            }
        }
        else
        {
            if (Input.GetButtonDown(InputManager.START + " P" + noControler))
            {
                isDisplayed = true;
                menu.enabled = true;
                Time.timeScale = 0.0f;
            }
        }
    }
}

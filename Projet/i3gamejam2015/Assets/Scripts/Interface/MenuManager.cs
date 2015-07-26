using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    enum StartMenuItem { GameStart, HowTo, Credits, Quit };
    private string dpadHorizontal = "Horizontal";
    private string dpadVertical = "Vertical";
    private Color32 highlithed = new Color32(107, 107, 107, 255);
    private Color32 normal = new Color32(0, 0, 0, 255);
    
    public GameObject StartButtonArea;
    private Outline[] buttonList;

    private StartMenuItem menuSelectedItem = StartMenuItem.GameStart;

    private bool[] wasPressed = new bool[4];


	// Use this for initialization
	void Start () {
        buttonList = StartButtonArea.GetComponentsInChildren<Outline>();

        PlayerPrefs.DeleteAll();
        SoundManager.Instance.PressStart_Play();
	
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 1; i < 5; i++ )
            CheckControlerStartMenu(i);
	}

    void CheckControlerStartMenu(int noControler)
    {

        if (Input.GetButtonDown(InputManager.A + " P" + noControler))
        {
            switch (menuSelectedItem){
            
                case StartMenuItem.Quit : Application.Quit();
                    break;

                case StartMenuItem.GameStart: Application.LoadLevelAdditiveAsync("SelectPlayers");
                    SoundManager.Instance.PressStart_Stop();
                    break;

                case StartMenuItem.Credits: Application.LoadLevelAdditiveAsync("Credits");
                    SoundManager.Instance.PressStart_Stop();
                    break;
            }
            SoundManager.Instance.Validate_Play();
            Destroy(gameObject);
            
        }

        if (!wasPressed[noControler-1] && Input.GetAxis(dpadVertical + noControler) < 0)
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

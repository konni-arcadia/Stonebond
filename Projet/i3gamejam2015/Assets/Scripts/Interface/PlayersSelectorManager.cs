using UnityEngine;
using System.Collections;

public class PlayersSelectorManager : MonoBehaviour {

    private PlayerSelectorManager[] listPlayerSelector;
    public int minPlayerNeeded = 3;

    public const string PlayerSeleted = "PlayerSelected";

	// Use this for initialization
	void Start () {

        listPlayerSelector = GetComponentsInChildren<PlayerSelectorManager>();

        InitializePlayerSelection();
        SoundManager.Instance.CharacterSelect_Play();
	}
	
	// Update is called once per frame
	void Update () {

        

        int nbReady = 0;
        bool canStart = true;
        for (int i = 0; i < listPlayerSelector.Length; i++)
        {
            CheckControlerStartMenu(i + 1);
            
            if (listPlayerSelector[i].HasPressedStart)
            {
                if (listPlayerSelector[i].HasChoosen)
                {
                    nbReady++;
                }
                else
                {
                    canStart = false;
                }
            }

        }

        if (canStart && (nbReady >= minPlayerNeeded))
        {
            string playerReadyString = "";

            for (int i = 0; i < listPlayerSelector.Length; i++)
                if (listPlayerSelector[i].HasChoosen)
                    playerReadyString += "1";
                else
                    playerReadyString += "0";

            PlayerPrefs.SetString(PlayerSeleted, playerReadyString);
            Application.LoadLevelAdditiveAsync("SelectLvl");
            
            Destroy(gameObject);
        }
	
	}

    void CheckControlerStartMenu(int noControler)
    {
        if (Input.GetButtonDown(InputManager.B + " P" + noControler))
        {
            Application.LoadLevelAdditiveAsync("SelectOption");
            SoundManager.Instance.Cancel_Play();
            Destroy(gameObject);
        }
    }

    void InitializePlayerSelection()
    {
        if (PlayerPrefs.HasKey(PlayerSeleted))
        {
            char[] listPlayer = PlayerPrefs.GetString(PlayerSeleted).ToCharArray();
            for (int i = 0; i < listPlayerSelector.Length; i++)
            {
                if (listPlayer[i] == '0')
                {
                    listPlayerSelector[i].SetInitialState();
                }
                else
                {
                    listPlayerSelector[i].SetChoosenState();
                }
            }
        }
        else
        {

            for (int i = 0; i < listPlayerSelector.Length; i++)
            {
                listPlayerSelector[i].SetInitialState();
            }
        }
    }
}

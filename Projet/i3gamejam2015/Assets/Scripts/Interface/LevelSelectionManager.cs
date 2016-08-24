﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelSelectionManager : MonoBehaviour {

	// The two MUST MATCH (in order, except for the random entry)
    public enum LvlSelectionItem { Random, Pipes, Forest, Spire, Cathedrale, Catacombs };
	private static readonly string[] RandomLevelList = { "LevelOrgan", "LevelForest", "LevelRoof", "LevelCathedrale", "LevelCatacombs" };

    public Image selectedLevelImage;
    public List<Sprite> levelList;

    private LvlSelectionItem menuSelectedItem;

    private bool[] wasPressed = new bool[4];
	private InputManager inputManager;
	public static LvlSelectionItem idOfLastChosenLevel = LvlSelectionItem.Pipes;

    // Use this for initialization
    void Start() {
		inputManager = GetComponent<InputManager> ();
		AudioSingleton<MenuAudioManager>.Instance.SetSelectStageSnapshot();
		menuSelectedItem = idOfLastChosenLevel;
		updateGui();
	}

	// Update is called once per frame
	void Update()
    {
        for (int i = 1; i < 5; i++)
            CheckControlerStartMenu(i);
	}

    private void CheckControlerStartMenu(int noControler)
    {

		if (inputManager.WasPressedCtrl(noControler, InputManager.A))
        {
			GameObject InControlObject = GameObject.Find("InControl");
			if(InControlObject != null)
				Destroy(InControlObject);

			//reset menu music to default as best practice
			AudioSingleton<MenuAudioManager>.Instance.SetDefaultSnapshot();
			idOfLastChosenLevel = menuSelectedItem;
			SceneManager.LoadScene(NameOfLevelScene(menuSelectedItem));
        }
		else if (inputManager.WasPressedCtrl(noControler, InputManager.B ))
		{
			SceneManager.LoadSceneAsync("SelectPlayers", LoadSceneMode.Additive);
			AudioSingleton<MenuAudioManager>.Instance.SetDefaultSnapshot();
			AudioSingleton<SfxAudioManager>.Instance.PlayCancel();

            Destroy(gameObject);
        }

		if (!wasPressed[noControler - 1] && inputManager.AxisValueCtrl(noControler, InputManager.Horizontal) < -InputManager.AxisDeadZone)
        {
            if (menuSelectedItem != (LvlSelectionItem)0)
            {
				AudioSingleton<SfxAudioManager>.Instance.PlayCursor();
                menuSelectedItem -= 1;
				updateGui();
				wasPressed[noControler - 1] = true;
            }

        }
		else if (!wasPressed[noControler - 1] && inputManager.AxisValueCtrl(noControler, InputManager.Horizontal) > InputManager.AxisDeadZone)
        {
            if (menuSelectedItem != LvlSelectionItem.Catacombs)
            {
				AudioSingleton<SfxAudioManager>.Instance.PlayCursor();
                menuSelectedItem += 1;
				updateGui();
                wasPressed[noControler - 1] = true;
            }

        }
		else if (inputManager.AxisValueCtrl(noControler, InputManager.Horizontal) < InputManager.AxisDeadZone &&
		         inputManager.AxisValueCtrl(noControler, InputManager.Horizontal) > -InputManager.AxisDeadZone
		         && wasPressed[noControler - 1])
        {
            wasPressed[noControler - 1] = false;
        }
    }

	private void updateGui() {
		Debug.Log(menuSelectedItem);
		selectedLevelImage.sprite = levelList[(int)menuSelectedItem];
	}

	public static string NameOfLevelScene(LvlSelectionItem menuSelectedItem) {
		if (menuSelectedItem == LvlSelectionItem.Random)
			return RandomLevelList[Random.Range(0, RandomLevelList.Length)];
		else
			return RandomLevelList[ 1 + (int)menuSelectedItem ];
	}
}

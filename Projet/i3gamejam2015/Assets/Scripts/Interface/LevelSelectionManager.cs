﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelSelectionManager : MonoBehaviour {

    enum LvlSelectionItem { Spire, Pipes, Cathedrale, Forest};

    private string dpadHorizontal = "Horizontal";
    private string dpadVertical = "Vertical";
    private Color32 highlithed = new Color32(152, 152, 152, 255);
    private Color32 normal = new Color32(0, 0, 0, 0);

    public Image selectedLevelImage;
    public List<Sprite> levelList;

    private LvlSelectionItem menuSelectedItem = LvlSelectionItem.Spire;

    private bool[] wasPressed = new bool[4];
	private InputManager inputManager;

    // Use this for initialization
    void Start()
    {
		inputManager = GetComponent<InputManager> ();
		AudioSingleton<MenuAudioManager>.Instance.SetSelectStageSnapshot();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 1; i < 5; i++)
            CheckControlerStartMenu(i);
    }

    void CheckControlerStartMenu(int noControler)
    {

		if (inputManager.WasPressedCtrl(noControler, InputManager.A))
        {
			GameObject InControlObject = GameObject.Find("InControl");
			if(InControlObject != null)
				Destroy(InControlObject);

			AudioSingleton<MenuAudioManager>.Instance.SetDefaultSnapshot();
            switch (menuSelectedItem)
            {
                case LvlSelectionItem.Cathedrale: Application.LoadLevel("LevelCathedrale");
					AudioSingleton<MusicAudioManager>.Instance.Stage_Play(Constants.StageEnum.RosetteOfTheWingedOnes);
                    break;

                case LvlSelectionItem.Forest: Application.LoadLevel("LevelForest"); ;
					AudioSingleton<MusicAudioManager>.Instance.Stage_Play(Constants.StageEnum.CloisterOfTheSilence);
                    break;

                case LvlSelectionItem.Pipes: Application.LoadLevel("LevelOrgan"); ;
					AudioSingleton<MusicAudioManager>.Instance.Stage_Play(Constants.StageEnum.PipesOfAwakening);
                    break;

                case LvlSelectionItem.Spire: Application.LoadLevel("LevelRoof"); ;
					AudioSingleton<MusicAudioManager>.Instance.Stage_Play(Constants.StageEnum.SpireHigh);
                    break;
            }
        }
		else if (inputManager.WasPressedCtrl(noControler, InputManager.B ))
        {
            Application.LoadLevelAdditiveAsync("SelectPlayers");
            //TODO not sure what it was supposed to do : SoundManager.Instance.StageSelect_Stop();
			AudioSingleton<MenuAudioManager>.Instance.SetDefaultSnapshot();
			AudioSingleton<SfxAudioManager>.Instance.PlayCancel();

            Destroy(gameObject);
        }

		if (!wasPressed[noControler - 1] && inputManager.AxisValueCtrl(noControler, InputManager.Vertical) < -InputManager.AxisDeadZone)
        {
            if (menuSelectedItem != (LvlSelectionItem)0)
            {
				AudioSingleton<SfxAudioManager>.Instance.PlayCursor();
                menuSelectedItem -= 1;
                selectedLevelImage.sprite = levelList[(int)menuSelectedItem];
                wasPressed[noControler - 1] = true;
            }

        }
		else if (!wasPressed[noControler - 1] && inputManager.AxisValueCtrl(noControler, InputManager.Vertical) > InputManager.AxisDeadZone)
        {
            if (menuSelectedItem != LvlSelectionItem.Forest)
            {
				AudioSingleton<SfxAudioManager>.Instance.PlayCursor();
                menuSelectedItem += 1;
                selectedLevelImage.sprite = levelList[(int)menuSelectedItem];
                wasPressed[noControler - 1] = true;
            }

        }
		else if (inputManager.AxisValueCtrl(noControler, InputManager.Vertical) < InputManager.AxisDeadZone &&
		         inputManager.AxisValueCtrl(noControler, InputManager.Vertical) > -InputManager.AxisDeadZone
		         && wasPressed[noControler - 1])
        {
            wasPressed[noControler - 1] = false;
        }
    }
}

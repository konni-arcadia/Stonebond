using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelSelectionManager : MonoBehaviour {

    enum LvlSelectionItem { Spire, Pipes, Cathedrale, Forest};

    public Image selectedLevelImage;
    public List<Sprite> levelList;

    private LvlSelectionItem menuSelectedItem = LvlSelectionItem.Cathedrale;

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

			//reset menu music to default as best practice
			AudioSingleton<MenuAudioManager>.Instance.SetDefaultSnapshot();
            switch (menuSelectedItem)
            {
                case LvlSelectionItem.Cathedrale:
					SceneManager.LoadScene("LevelCathedrale");
                    break;

                case LvlSelectionItem.Forest:
					SceneManager.LoadScene("LevelForest");
                    break;

                case LvlSelectionItem.Pipes:
					SceneManager.LoadScene("LevelOrgan");
                    break;

                case LvlSelectionItem.Spire:
					SceneManager.LoadScene("LevelRoof");
                    break;
            }
        }
		else if (inputManager.WasPressedCtrl(noControler, InputManager.B ))
		{
			SceneManager.LoadSceneAsync("SelectPlayers", LoadSceneMode.Additive);
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

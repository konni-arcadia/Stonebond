using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelSelectionManager : MonoBehaviour {

    enum LvlSelectionItem { Cathedrale, Foret};

    private string dpadHorizontal = "Horizontal";
    private string dpadVertical = "Vertical";
    private Color32 highlithed = new Color32(152, 152, 152, 255);
    private Color32 normal = new Color32(0, 0, 0, 0);

    public GameObject StartButtonArea;
    private Outline[] buttonList;

    private LvlSelectionItem menuSelectedItem = LvlSelectionItem.Cathedrale;

    private bool[] wasPressed = new bool[4];


    // Use this for initialization
    void Start()
    {
        buttonList = StartButtonArea.GetComponentsInChildren<Outline>();
        SoundManager.Instance.StageSelect_Play();

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 1; i < 5; i++)
            CheckControlerStartMenu(i);
    }

    void CheckControlerStartMenu(int noControler)
    {

        if (Input.GetButtonDown(InputManager.A + " P" + noControler))
        {
            SoundManager.Instance.StageSelect_Stop();
            switch (menuSelectedItem)
            {

                case LvlSelectionItem.Cathedrale: Application.LoadLevel("LevelCathedrale");
                    SoundManager.Instance.Stage_Play(SoundManager.StageEnum.One);
                    break;

                case LvlSelectionItem.Foret: Application.LoadLevel("LevelForest"); ;
                    SoundManager.Instance.Stage_Play(SoundManager.StageEnum.Two);
                    break;
            }
        }
        else if (Input.GetButtonDown(InputManager.B + " P" + noControler))
        {
            Application.LoadLevelAdditiveAsync("SelectPlayers");
            SoundManager.Instance.StageSelect_Stop();
            Destroy(gameObject);
        }

        if (!wasPressed[noControler - 1] && Input.GetAxis(dpadVertical + noControler) < 0)
        {
            if (menuSelectedItem != (LvlSelectionItem)0)
            {
                buttonList[(int)menuSelectedItem].effectColor = normal;
                menuSelectedItem -= 1;
                buttonList[(int)menuSelectedItem].effectColor = highlithed;
                wasPressed[noControler - 1] = true;
            }

        }
        else if (!wasPressed[noControler - 1] && Input.GetAxis(dpadVertical + noControler) > 0)
        {
            if (menuSelectedItem != LvlSelectionItem.Foret)
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
}

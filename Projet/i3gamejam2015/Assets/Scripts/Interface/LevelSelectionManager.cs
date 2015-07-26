using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelSelectionManager : MonoBehaviour {

    enum LvlSelectionItem { Spire, Forest, Pipes, Cathedrale};

    private string dpadHorizontal = "Horizontal";
    private string dpadVertical = "Vertical";
    private Color32 highlithed = new Color32(152, 152, 152, 255);
    private Color32 normal = new Color32(0, 0, 0, 0);

    public Image selectedLevelImage;
    public List<Sprite> levelList;

    private LvlSelectionItem menuSelectedItem = LvlSelectionItem.Spire;

    private bool[] wasPressed = new bool[4];


    // Use this for initialization
    void Start()
    {
        //SoundManager.Instance.StageSelect_Play();

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

                case LvlSelectionItem.Forest: Application.LoadLevel("LevelForest"); ;
                    SoundManager.Instance.Stage_Play(SoundManager.StageEnum.Two);
                    break;

                case LvlSelectionItem.Pipes: Application.LoadLevel("LevelOrgan"); ;
                    SoundManager.Instance.Stage_Play(SoundManager.StageEnum.Three);
                    break;

                case LvlSelectionItem.Spire: Application.LoadLevel("LevelRoof"); ;
                    SoundManager.Instance.Stage_Play(SoundManager.StageEnum.Four);
                    break;
            }
        }
        else if (Input.GetButtonDown(InputManager.B + " P" + noControler))
        {
            Application.LoadLevelAdditiveAsync("SelectPlayers");
            SoundManager.Instance.StageSelect_Stop();
            SoundManager.Instance.Cancel_Play();
            Destroy(gameObject);
        }

        if (!wasPressed[noControler - 1] && Input.GetAxis(dpadVertical + noControler) < 0)
        {
            if (menuSelectedItem != (LvlSelectionItem)0)
            {
                menuSelectedItem -= 1;
                selectedLevelImage.sprite = levelList[(int)menuSelectedItem];
                wasPressed[noControler - 1] = true;
            }

        }
        else if (!wasPressed[noControler - 1] && Input.GetAxis(dpadVertical + noControler) > 0)
        {
            if (menuSelectedItem != LvlSelectionItem.Cathedrale)
            {
                menuSelectedItem += 1;
                selectedLevelImage.sprite = levelList[(int)menuSelectedItem];
                wasPressed[noControler - 1] = true;
            }

        }
        else if (Input.GetAxis(dpadVertical + noControler) == 0 && wasPressed[noControler - 1])
        {
            wasPressed[noControler - 1] = false;
        }
    }
}

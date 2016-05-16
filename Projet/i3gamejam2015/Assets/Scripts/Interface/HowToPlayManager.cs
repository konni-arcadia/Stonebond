using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class HowToPlayManager : MonoBehaviour {

    private InputManager inputManager;
    private float displayTime;

    // Use this for initialization
    void Start () {
        inputManager = GetComponent<InputManager>();
        displayTime = Time.time;
    }
	
	// Update is called once per frame
	void Update () {
        for (int i = 1; i < 5; i++)
            CheckControlerStartMenu(i);
    }

    void CheckControlerStartMenu(int noControler)
    {

        if (inputManager.WasPressedCtrl(noControler, InputManager.B))
        {
            
            if (PlayerPrefs.HasKey("FromPlayerSelection"))
            {
                PlayerPrefs.DeleteKey("FromPlayerSelection");
                SceneManager.LoadSceneAsync("SelectPlayers", LoadSceneMode.Additive);
            }
            else
            {
                PlayerPrefs.SetString("ComeFromMenu", "");
                SceneManager.LoadSceneAsync("SelectOption", LoadSceneMode.Additive);
            }
            AudioSingleton<SfxAudioManager>.Instance.PlayCancel();

            Destroy(gameObject);
        }
        else if(inputManager.WasPressedCtrl(noControler, InputManager.START))
        {
            if (PlayerPrefs.HasKey("FromPlayerSelection") && (Time.time > (displayTime + 2)))
            {
                PlayerPrefs.DeleteKey("FromPlayerSelection");
                SceneManager.LoadSceneAsync("SelectLvl", LoadSceneMode.Additive);
                Destroy(gameObject);
            }
        }
    }
}

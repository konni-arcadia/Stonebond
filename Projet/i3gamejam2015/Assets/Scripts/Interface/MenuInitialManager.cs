using UnityEngine;
using System.Collections;

public class MenuInitialManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if (PlayerPrefs.HasKey("ComeFromLVL"))
        {
            PlayerPrefs.DeleteKey("ComeFromLVL");
            Application.LoadLevelAdditiveAsync("SelectLvl");
        }
        else
        {
            if (GameState.Instance != null)
                GameState.Instance.ClearScores();
            Application.LoadLevelAdditiveAsync("SelectOption");
        }

        Destroy(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

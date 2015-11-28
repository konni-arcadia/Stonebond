using UnityEngine;
using System.Collections;

public class CreditsManager : MonoBehaviour {

	private InputManager inputManager;
	// Use this for initialization
	void Start () {
		inputManager = GetComponent<InputManager> ();
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 1; i < 5; i++ )
            CheckControlerStartMenu(i);
	}

    void CheckControlerStartMenu(int noControler)
    {

        if (Input.GetButtonDown(InputManager.B + " P" + noControler))
        {
            Application.LoadLevelAdditiveAsync("SelectOption");

            Destroy(gameObject);

        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class CompanyLogoScene : MonoBehaviour {

	private float timeRemaining = 5.0f;
	public GameObject Panel;

	// Use this for initialization
	void Start () {
		Panel.GetComponent<Image>().color = Color.black;
	}

	// Update is called once per frame
	void Update() {
		timeRemaining -= Time.deltaTime;
		for (int i = 1; i <= 4; i++)
			CheckControlerStartMenu(i);
		if (timeRemaining < 0)
			SceneManager.LoadScene("Menu");
	}

	private void CheckControlerStartMenu(int noControler) {
		// Skip with start button
		if (Input.GetButtonDown(InputManager.A + " P" + noControler) || Input.GetButtonDown(InputManager.START + " P" + noControler)) {
			timeRemaining = 0;
		}
	}
}

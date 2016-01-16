using UnityEngine;
using System.Collections;

public class CreditsManager : MonoBehaviour {

	private InputManager inputManager;

    public RectTransform canvas;
    public RectTransform viewport;

    public int ScrollSpeed = 3;

	// Use this for initialization
	void Start () {
		inputManager = GetComponent<InputManager> ();
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 1; i < 5; i++ )
            CheckControlerStartMenu(i);

        if (canvas.sizeDelta.y < canvas.anchoredPosition.y)
        {
            canvas.anchoredPosition = new Vector3(0, -viewport.sizeDelta.y, 0);
        }
        else
        {
            canvas.anchoredPosition = new Vector3(0, canvas.anchoredPosition.y + ScrollSpeed, 0);
        }
	}

    void CheckControlerStartMenu(int noControler)
    {

		if (inputManager.WasPressedCtrl(noControler, InputManager.B))
        {
            Application.LoadLevelAdditiveAsync("SelectOption");

            Destroy(gameObject);

        }
    }
}

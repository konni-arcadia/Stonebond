﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class MenuInitialManager : MonoBehaviour {

	public GameObject FadePanel;

	// Use this for initialization
	void Start () {
        if (PlayerPrefs.HasKey("ComeFromLVL"))
        {
            PlayerPrefs.DeleteKey("ComeFromLVL");
			SceneManager.LoadSceneAsync("SelectLvl", LoadSceneMode.Additive);
        }
        else
        {
            if (GameState.Instance != null)
                GameState.Instance.ClearScores();
			FadePanel.GetComponent<Image>().color = Color.black;
			StartCoroutine(FadeLater());
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private IEnumerator FadeLater() {
		yield return new WaitForSeconds(0.05f);
		FadePanel.GetComponent<Image>().CrossFadeAlpha(0, 1.0f, true);
		SceneManager.LoadSceneAsync("SelectOption", LoadSceneMode.Additive);
		Destroy(gameObject);
	}
}

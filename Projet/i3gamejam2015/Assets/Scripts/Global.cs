using System;
using UnityEngine.SceneManagement;

public static class Global {

	public static void ReturnToLevelScreen() {
		SceneManager.LoadSceneAsync("SelectLvl", LoadSceneMode.Additive);
	}

}

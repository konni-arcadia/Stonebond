using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.SceneManagement;

class EditorScrips : EditorWindow {

    [MenuItem("Stonebond/Play from startup _%h")]
    public static void RunMainScene() {
		EditorSceneManager.OpenScene("Assets/Scenes/Startup.unity");
        EditorApplication.isPlaying = true;
    }

    [MenuItem("Stonebond/Open WinScene")]
    public static void OpenWinScene() {
		EditorSceneManager.OpenScene("Assets/Scenes/Menu/WinScreen.unity");
    }

    [MenuItem("Stonebond/Open SelectPlayer")]
    public static void OpenSelectPlayer() {
		EditorSceneManager.OpenScene("Assets/Scenes/Menu/SelectPlayers.unity");
    }

}


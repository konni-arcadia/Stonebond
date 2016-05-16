using UnityEngine;
using UnityEditor;
using System.Collections;

class EditorScrips : EditorWindow {

    [MenuItem("Stonebond/Play from startup _%h")]
    public static void RunMainScene() {
        EditorApplication.OpenScene("Assets/Scenes/Startup.unity");
        EditorApplication.isPlaying = true;
    }

    [MenuItem("Stonebond/Open WinScene")]
    public static void OpenWinScene() {
        EditorApplication.OpenScene("Assets/Scenes/Menu/WinScreen.unity");
    }

}


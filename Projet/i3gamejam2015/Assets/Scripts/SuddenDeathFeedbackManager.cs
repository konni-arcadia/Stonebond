using UnityEngine;
using System.Collections;

public class SuddenDeathFeedbackManager : MonoBehaviour
{
	public void OnSuddenDeathTextShown()
	{
        Overlay.ShowFlash();
		AudioSingleton<VoiceAudioManager>.Instance.PlaySuddenDeathVoice();
	}
}

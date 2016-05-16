using UnityEngine;
using System.Collections;

public class SuddenDeathFeedbackManager : MonoBehaviour
{
	public void OnSuddenDeathTextShown()
	{
		Flash.Show();
		AudioSingleton<VoiceAudioManager>.Instance.PlaySuddenDeathVoice();
	}
}

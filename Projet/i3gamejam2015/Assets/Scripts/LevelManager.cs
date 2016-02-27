using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {

	private PlayerStateController[] players;
	private bool bondMode = false;
	public GameObject bondLinkPrefab;
	public GameObject gaugeInner, gaugeFrame;
	private const float gaugeDecreaseFactor = 0.006f; // previously 0.00025
	public float introDuration = 2;
	public float pauseWinDuration = 1.0f;
	private BondLink bondLink;
	private float bondLinkGauge, appearedSinceSec;
	private WinScreenManager WinScreenManager {
		get { return FindObjectOfType<WinScreenManager>(); }
	}
	private bool hasAlreadyShownWinScreen, allowsCreateBond;

	// Requires the objects to have already been spawned (PlayerSpawner::Awake, which is executed before)
	void Start () {
        //Load the pause menu
		SceneManager.LoadScene("Pause", LoadSceneMode.Additive);
		SceneManager.LoadScene("WinScreen", LoadSceneMode.Additive);
	}

	void Update() {
		GatherPlayers();
		// Check if we are only two left, this would initiate the "bond mode"
		List<PlayerStateController> activePlayers = new List<PlayerStateController>();
		foreach (PlayerStateController player in players) {
			if (!player.IsCrystaled())
				activePlayers.Add(player);
		}
		// Only allows the creation of the bond if all players have been active since the last cut
		allowsCreateBond = allowsCreateBond ||
			(activePlayers.Count == players.Length && !bondMode)
			// Once many players have been killed, we can't create a bond anymore unless all players get back on track
			&& activePlayers.Count >= 2;

		if (activePlayers.Count == 2 && !bondMode && allowsCreateBond)
			EnterBondMode(activePlayers);

		if (bondMode && !TimeManager.isPaused) {
			gaugeFrame.SetActive(true);
			gaugeInner.SetActive(true);
			gaugeInner.transform.localScale = new Vector3(bondLinkGauge, 1, 1);
			appearedSinceSec += Time.deltaTime; // NB: the bond is updated only after the pause

			if (appearedSinceSec >= BondLink.bondCreateDelayAfterMiddleBlastDuration) {
				var distance = Vector3.Distance(bondLink.playerA.transform.position, bondLink.playerB.transform.position);
				bondLinkGauge += distance * gaugeDecreaseFactor * Time.deltaTime;
				bondLink.completion = bondLinkGauge;

				// A winner is designated
				if (bondLinkGauge > 1) {
					bondLinkGauge = 1;


					if (!hasAlreadyShownWinScreen) {
						//Remove any music by setting default snapshot
						AudioSingleton<MusicAudioManager>.Instance.SetMusicDefaultSnapshot();
						//Remove bound by setting default snapshot
						AudioSingleton<SfxAudioManager>.Instance.SetSfxDefaultSnapshot();
						AudioSingleton<MusicAudioManager>.Instance.PlayVictoryJingle();
						//This call should be replaced by observing events rather than using invoke with time
						AudioSingleton<SfxAudioManager>.Instance.SetNoSfxOnMainMixerAfterVictory(1);
						//Set the main default snapshot to restore initial set up
						AudioSingleton<MenuAudioManager>.Instance.SetMainMenuSnapshot();

						var p1 = bondLink.playerAStateController;
						var p2 = bondLink.playerBStateController;
						WinScreenManager.IdOfWonP1 = p1.GetPlayerId();
						WinScreenManager.IdOfWonP2 = p2.GetPlayerId();
						GameState.Instance.NotifyWinners(p1.GetPlayerId(), p2.GetPlayerId());

						foreach (PlayerStateController player in players) {
							player.SetGameOver ();
						}
						hasAlreadyShownWinScreen = true;
						TimeManager.Pause(pauseWinDuration, null, () => {
							AudioSingleton<VoiceAudioManager>.Instance.PlayGameOver();
							WinScreenManager.showScreen ();
						});
                    }
				}
			}
		}
		else {
			gaugeFrame.SetActive(false);
			gaugeInner.SetActive(false);
		}
	}

	public void bondHasBeenBrokenBy(PlayerStateController player) {
		var p1 = bondLink.playerAStateController;
		var p2 = bondLink.playerBStateController;
		if (p1 == player || p2 == player) {
			Debug.Log("Discarding slash by bonded player");
			return;
		}
		ExitBondMode(p1, p2);

		ScreenShake.ShakeXY (1.0f, 2.0f, 1.0f, 2.0f);
		Flash.Show ();
	}

	private void EnterBondMode(List<PlayerStateController> activePlayers) {
		Debug.Log("Entering bond mode");
		bondMode = true;
		// Create a bond object linking the two players
		GameObject obj = Instantiate(bondLinkPrefab);
		bondLink = obj.GetComponent<BondLink>();
		bondLink.LinkPlayers ( activePlayers[0].gameObject, activePlayers[1].gameObject);
		activePlayers[0].SetBondLink(bondLink);
		activePlayers[1].SetBondLink(bondLink);
		bondLinkGauge = 0;
		appearedSinceSec = 0;

		TimeManager.Pause(BondLink.bondCreateGameplayPauseDuration);
	}

	private void ExitBondMode(PlayerStateController p1, PlayerStateController p2) {
		Destroy(bondLink.gameObject);
		p1.SetBondLink(null);
		p2.SetBondLink(null);
		p1.DisableShield ();
		p2.DisableShield ();
		Debug.Log("Leaving bond mode");
		bondMode = false;
		allowsCreateBond = false;

		TimeManager.Pause(BondLink.bondBreakGameplayPauseDuration);
	}

	private void GatherPlayers() {
		if (players != null && players.Length > 0) return;

		// Build the list of players
		var list = FindObjectsOfType<PlayerStateController>();
		players = new PlayerStateController[list.Length];
		foreach (var p in list) {
			if (players[p.GetPlayerId() - 1] == null)
				players[p.GetPlayerId() - 1] = p;
			else
				throw new InvalidOperationException("Player ID " + p.GetPlayerId() + " more than once in this scene");
		}
	}

	public Constants.StageEnum GetCurrentStageEnum()
	{
		return (Constants.StageEnum) Enum.Parse(typeof(Constants.StageEnum), (string)SceneManager.GetActiveScene().name);
	}
}

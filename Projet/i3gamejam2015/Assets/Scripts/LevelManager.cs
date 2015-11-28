using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {

	private PlayerStateController[] players;
	private bool bondMode = false;
	public GameObject bondLinkPrefab;
	public GameObject gaugeInner, gaugeFrame;
	public float gaugeDecreaseFactor;
	public float increaseStartCooldownSecs = 2;
	public float introDuration = 2;
	private BondLink bondLink;
	private float bondLinkGauge, appearedSinceSec;
	private WinScreenManager WinScreenManager {
		get { return FindObjectOfType<WinScreenManager>(); }
	}
	private bool hasAlreadyShownWinScreen, allowsCreateBond;

	// Requires the objects to have already been spawned (PlayerSpawner::Awake, which is executed before)
	void Start () {
        //Load the pause menu
        Application.LoadLevelAdditive("Pause");
		Application.LoadLevelAdditive("WinScreen");
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

		if (bondMode) {
			gaugeFrame.active = gaugeInner.active = true;
			gaugeInner.transform.localScale = new Vector3(bondLinkGauge, 1, 1);
			appearedSinceSec += Time.deltaTime;

			if (appearedSinceSec >= increaseStartCooldownSecs) {
				var distance = Vector3.Distance(bondLink.emitterA.transform.position, bondLink.emitterB.transform.position);
				bondLinkGauge += distance * gaugeDecreaseFactor;

				// A winner is designated
				if (bondLinkGauge > 1) {
					bondLinkGauge = 1;
					if (!hasAlreadyShownWinScreen) {
						var p1 = bondLink.emitterA.GetComponent<PlayerStateController>();
						var p2 = bondLink.emitterB.GetComponent<PlayerStateController>();
						WinScreenManager.IdOfWonP1 = p1.GetPlayerId();
						WinScreenManager.IdOfWonP2 = p2.GetPlayerId();
						WinScreenManager.IdOfLevelToRestartTo = Application.loadedLevel;
						GameState.Instance.NotifyWinners(p1.GetPlayerId(), p2.GetPlayerId());
						WinScreenManager.showScreen();
						hasAlreadyShownWinScreen = true;
					}
				}
			}
		}
		else {
			gaugeFrame.active = gaugeInner.active = false;
		}
	}

	public void bondHasBeenBrokenBy(PlayerStateController player) {
		var p1 = bondLink.emitterA.GetComponent<PlayerStateController>();
		var p2 = bondLink.emitterB.GetComponent<PlayerStateController>();
		if (p1 == player || p2 == player) {
			Debug.Log("Discarding slash by bonded player");
			return;
		}
		ExitBondMode(p1, p2);

		MyLittlePoney.shake (1.0f, 1.0f, 2.0f, 2.0f);
		Flash.flash (0.0f, 0.0f, 0.0f);
	}

	private void EnterBondMode(List<PlayerStateController> activePlayers) {
		Debug.Log("Entering bond mode");
		bondMode = true;
		// Create a bond object linking the two players
		GameObject obj = Instantiate(bondLinkPrefab);
		bondLink = obj.GetComponent<BondLink>();
		bondLink.emitterA = activePlayers[0].gameObject;
		bondLink.emitterB = activePlayers[1].gameObject;
		activePlayers[0].setBondLink(bondLink);
		activePlayers[1].setBondLink(bondLink);
		bondLinkGauge = 0;
		appearedSinceSec = 0;

		MyLittlePoney.slowMotion ();
	}

	private void ExitBondMode(PlayerStateController p1, PlayerStateController p2) {
		Destroy(bondLink.gameObject);
		p1.setBondLink(null);
		p2.setBondLink(null);
		Debug.Log("Leaving bond mode");
		bondMode = false;
		allowsCreateBond = false;
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
}

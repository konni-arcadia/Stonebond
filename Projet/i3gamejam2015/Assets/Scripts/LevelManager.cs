using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {

	private PlayerStateController[] players;
	private bool bondMode = false;
	public GameObject bondLinkPrefab;
	private BondLink bondLink;

	// Use this for initialization
	void Start () {
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

	void Update() {
		// Check if we are only two left, this would initiate the "bond mode"
		List<PlayerStateController> activePlayers = new List<PlayerStateController>();
		foreach (PlayerStateController player in players) {
			if (!player.IsSlashed())
				activePlayers.Add(player);
		}

		if (/*activePlayers.Count == 2 &&*/ !bondMode) {
			Debug.Log("Entering bond mode");
			bondMode = true;
			// Create a bond object linking the two players
			GameObject obj = Instantiate(bondLinkPrefab);
			bondLink = obj.GetComponent<BondLink>();
			bondLink.emitterA = activePlayers[0].gameObject;
			bondLink.emitterB = activePlayers[1].gameObject;
			activePlayers[0].setBondLink(bondLink);
			activePlayers[1].setBondLink(bondLink);
		}
	}

	public void bondHasBeenSlashed() {
		Destroy(bondLink.gameObject);
			bondLink.emitterA.GetComponent<PlayerStateController>().setBondLink(null);
		bondLink.emitterB.GetComponent<PlayerStateController>().setBondLink(null);
	}
}

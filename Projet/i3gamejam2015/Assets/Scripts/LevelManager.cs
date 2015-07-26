﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {

	private PlayerStateController[] players;
	private bool bondMode = false;
	public GameObject bondLinkPrefab;
	public GameObject gaugeInner, gaugeFrame;
	public float gaugeDecreaseFactor;
	private BondLink bondLink;
	private float bondLinkGauge;

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

		if (activePlayers.Count == 2 && !bondMode)
			EnterBondMode(activePlayers);

		if (bondMode) {
			gaugeFrame.active = gaugeInner.active = true;
			gaugeInner.transform.localScale = new Vector3(bondLinkGauge, 1, 1);

			var distance = Vector3.Distance(bondLink.emitterA.transform.position, bondLink.emitterB.transform.position);
			bondLinkGauge = Mathf.Max(0, bondLinkGauge - distance * gaugeDecreaseFactor);
		}
		else {
			gaugeFrame.active = gaugeInner.active = false;
		}
	}

	public void bondHasBeenSlashedBy(PlayerStateController player) {
		var p1 = bondLink.emitterA.GetComponent<PlayerStateController>();
		var p2 = bondLink.emitterB.GetComponent<PlayerStateController>();
		if (p1 == player || p2 == player) {
			Debug.Log("Discarding slash by bonded player");
			return;
		}
		ExitBondMode(p1, p2);
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
		bondLinkGauge = 1;
	}

	private void ExitBondMode(PlayerStateController p1, PlayerStateController p2) {
		Destroy(bondLink.gameObject);
		p1.setBondLink(null);
		p2.setBondLink(null);
		Debug.Log("Leaving bond mode");
		bondMode = false;
	}
}

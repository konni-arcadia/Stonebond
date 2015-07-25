﻿using UnityEngine;
using System.Collections;

public class PlayerSpawner : MonoBehaviour {

	public int playerId = 1;
	public GameObject playerPrefab;

	// Use this for initialization
	void Start () {
		GameObject obj = Instantiate<GameObject>(playerPrefab);
		PlayerStateController controller = obj.GetComponent<PlayerStateController>();
		obj.transform.position = transform.position;
		controller.playerId = playerId;
	}
}

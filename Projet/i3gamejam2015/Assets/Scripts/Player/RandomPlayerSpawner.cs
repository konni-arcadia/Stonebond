using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

// Purpose:
// Spawn a player at one of the spawn points. Spawn points are automatically detected (finds all game objects with tag Spawner on the scene).
// Verify that you have placed at least four PlayerSpawners.
public class RandomPlayerSpawner : MonoBehaviour {

	private LevelManager levelManager;
	private GameObject[] spawners;
	private System.Random random = new System.Random();
	private float timeBeforeSpawn;
	public GameObject PlayerPrefab;

	void Awake() {
		levelManager = FindObjectOfType<LevelManager>();
		spawners = GameObject.FindGameObjectsWithTag("Spawner");
		timeBeforeSpawn = levelManager.introDuration;
	}

	void Start() {}

	void Update() {
		// Just went under 0 - process with spawn
		if (timeBeforeSpawn >= 0 && timeBeforeSpawn < Time.deltaTime) {
			// Spawn players among available spawn points
			var availableSpawnPoints = new List<GameObject>(spawners);
			Assert.IsTrue(availableSpawnPoints.Count >= GameState.Instance.NumPlayers);
			// For player = 4, 3, 2, 1
			for (int i = GameState.Instance.NumPlayers; i > 0; i--) {
				// Select a location randomly, instantiate player and remove from available locations
				int next = random.Next(0, availableSpawnPoints.Count);
				InstantiatePlayer(i, availableSpawnPoints[next].transform.position);
				availableSpawnPoints.RemoveAt(next);
			}
		}
		timeBeforeSpawn -= Time.deltaTime;
	}

	void InstantiatePlayer(int playerId, Vector3 position) {
		GameObject obj = Instantiate<GameObject>(PlayerPrefab);
		PlayerStateController controller = obj.GetComponent<PlayerStateController>();
		obj.transform.position = position;
		controller.playerId = playerId;
	}
}

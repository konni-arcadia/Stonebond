using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GameState : Singleton<GameState> {
	// Info about the player (carried over all scenes).
	public class PlayerInfo {
		public string Name;
		public Color Color;
		public Color BodyColor;
		public int ControllerId;
		public int TotalScore = 0;
	}

	// May be moved somewhere else; just to ensure that during development we always have exactly 4 players.
	protected GameState() {
		AddPlayer("P1", predefinedPlayerColors[0], 1);
		AddPlayer("P2", predefinedPlayerColors[1], 2);
		AddPlayer("P3", predefinedPlayerColors[2], 3);
		AddPlayer("P4", predefinedPlayerColors[3], 4);
	}

	// Adds a new player.
	public void AddPlayer(string name, Color color, int controllerId) {
		var p = new PlayerInfo();
		p.Name = name;
		p.Color = color;
        p.BodyColor = color; // Color.Lerp(Color.white, color, 0.8f);
		p.ControllerId = controllerId;
		players.Add(p);
	}

	public void ClearScores() {
		foreach (PlayerInfo player in players) {
			player.TotalScore = 0;
		}
	}

	// Number of players.
	public int NumPlayers { get { return players.Count; } }

	// Increments the score of the two winners.
	public void NotifyWinners(int playerAId, int playerBId) {
		Player(playerAId).TotalScore += 1;
		Player(playerBId).TotalScore += 1;
	}

	// Get a given player.
	public PlayerInfo Player(int id) {
		id -= 1;
		return id >= 0 && id < players.Count ? players[id] : null;
	}

	public void ResetPlayerControllersAndScore() {
		for (int i = 1; i <= players.Count; i++) {
			Player(i).ControllerId = i;
			Player(i).TotalScore = 0;
		}
	}

	//
	// PRIVATE
	//
	private List<PlayerInfo> players = new List<PlayerInfo>();
	private readonly Color[] predefinedPlayerColors = {
		// Shred (red) 0xB3123B
		new Color(0.701f, 0.070f, 0.231f),
		// Buddy (blue) old color: 0x00ACB4
        new Color32(0x16, 0xF1, 0xFF, 0xFF),
		// Wise (green) 0x0B9300
		new Color(0.04f, 0.58f, 0f),
		// Dextrous (yellow) 0xBBAD00
		new Color(0.73f, 0.68f, 0f),
	};
}

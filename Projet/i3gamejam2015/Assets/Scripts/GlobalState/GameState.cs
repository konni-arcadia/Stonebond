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
		AddPlayer("P1", playerBodyColors[0], playerChromaColors[0], 1);
		AddPlayer("P2", playerBodyColors[1], playerChromaColors[1], 2);
		AddPlayer("P3", playerBodyColors[2], playerChromaColors[2], 3);
		AddPlayer("P4", playerBodyColors[3], playerChromaColors[3], 4);
	}

	// Adds a new player.
	private void AddPlayer(string name, Color bodyColor, Color chromaColor, int controllerId) {
		var p = new PlayerInfo();
		p.Name = name;
		p.Color = chromaColor;
        p.BodyColor = bodyColor;
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
	private readonly Color[] playerBodyColors = {
		// Shred (red) old color: 0xB3123B
        new Color32(0xFD, 0x5E, 0x86, 0xFF),
		// Buddy (blue) old color: 0x00ACB4
        new Color32(0x16, 0xF1, 0xFF, 0xFF),
		// Wise (green) old color: 0x0B9300
        new Color32(0x51, 0xFF, 0x43, 0xFF),
		// Dextrous (yellow) old color: 0xBBAD00
		new Color32(0xFF, 0xF1, 0x45, 0xFF),
    };

    private readonly Color[] playerChromaColors = {
		// Shred (red) a bit more green
        new Color32(0xFD, 0x90, 0x86, 0xFF),
		// Buddy (blue) same as body
        new Color32(0x16, 0xF1, 0xFF, 0xFF),
		// Wise (green) same as body
        new Color32(0x51, 0xFF, 0x43, 0xFF),
		// Dextrous (yellow) same as body
		new Color32(0xFF, 0xF1, 0x45, 0xFF),
    };
}

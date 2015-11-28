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
	}

	// May be moved somewhere else; just to ensure that during development we always have exactly 4 players.
	protected GameState() {
		AddPlayer("P1", predefinedPlayerColors[0]);
		AddPlayer("P2", predefinedPlayerColors[1]);
		AddPlayer("P3", predefinedPlayerColors[2]);
		AddPlayer("P4", predefinedPlayerColors[3]);
	}

	// Adds a new player.
	public void AddPlayer(string name, Color color) {
		var p = new PlayerInfo();
		p.Name = name;
		p.Color = color;
		players.Add(p);
	}

	// Get a given player.
	public PlayerInfo Player(int id) {
		id -= 1;
		return id >= 0 && id < players.Count ? players[id] : null;
	}

	//
	// PRIVATE
	//
	private List<PlayerInfo> players = new List<PlayerInfo>();
	private readonly Color[] predefinedPlayerColors = {
		// Shred
		Color.red,
		// Wise
		Color.green,
		// Buddy
		Color.cyan,
		// Dextrous
		Color.yellow
	};
}

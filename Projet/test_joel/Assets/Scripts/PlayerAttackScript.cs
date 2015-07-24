using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAttackScript : MonoBehaviour {
	
	public int playerId;
	public Collider2D bodyCollider;

	public float initTime = 1.0f;
	public float attackTime = 1.0f;
	public float cooldownTime = 4.0f;

	private List<PlayerAttackScript> enemies = new List<PlayerAttackScript>();
	private Collider2D  colliderRight;

	public enum State {
		READY,
		INIT,
		ATTACK,
		COOLDOWN
	}

	[HideInInspector]
	public State state = State.READY;

	private float stateTimer = 0.0f;

	// Use this for initialization
	void Awake () {
		colliderRight = transform.Find ("colliderAttackRight").GetComponent<Collider2D> ();
	}

	void Start() {
		PlayerAttackScript[] allPlayers = FindObjectsOfType<PlayerAttackScript> ();
		for(int i = 0; i < allPlayers.Length; ++i) {
			PlayerAttackScript player = allPlayers[i];
			if(player.playerId != playerId) {
				enemies.Add(player);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		switch (state) {
		case State.READY:
			updateReady();
			break;
		case State.INIT:
			updateInit ();
			break;
		case State.ATTACK:
			updateAttack();
			break;
		case State.COOLDOWN:
			updateCooldown ();
			break;
		}
	}

	private void updateReady() {
		if (Input.GetButtonDown ("Fire1") ) {
			print ("enter INIT state");
			state = State.INIT;
			stateTimer = initTime;
		}
	}

	private void updateInit() {
		stateTimer -= Time.deltaTime;
		if(stateTimer <= 0.0f) {
			print ("enter ATTACK state");
			state = State.ATTACK;
			stateTimer = attackTime;
		}
	}

	private void updateAttack() {
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0.0f) {
			print ("enter COOLDOWN state");
			state = State.COOLDOWN;
			stateTimer = cooldownTime;
			return;
		}

		bool knockedBack = false;
		for(int i = 0; i < enemies.Count; ++i) {
			PlayerAttackScript enemy = enemies[i];
			if (colliderRight.IsTouching (enemy.bodyCollider)) {
				if(enemy.state != State.INIT && enemy.state != State.ATTACK) {
					damage ();
				}
				else {
					enemy.knockback();
					knockedBack = true;
				}
			}
		}

		if (knockedBack) {
			knockback ();
		}
	}

	private bool checkCollision(PlayerAttackScript enemy, Collider2D collider) {
		if (colliderRight.IsTouching (enemy.bodyCollider)) {
			if(enemy.state != State.INIT && enemy.state != State.ATTACK) {
				damage ();
			}
			else {
				enemy.knockback();
				return true;
			}
		}

		return false;
	}

	private void updateCooldown() {
		stateTimer -= Time.deltaTime;
		if(stateTimer <= 0.0f) {
			print ("enter READY state");
			state = State.READY;
		}
	}
	
	public void knockback() {
		print ("enter COOLDOWN state");
		state = State.COOLDOWN;
		stateTimer = cooldownTime;

		// TODO call player.knockback();
	}

	public void damage() {
		// TODO call player.damage();
	}
}

﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour {
	
	private static int COMP_WAIT_TIME = 100;

	public enum MESSAGE { DRAW, DISCARD, DRAW_NEW_CARD, CHOOSE_AUGMENTATION, CHOOSE_ACTION };

	public CardBankScript cardBank;
	public PlayerScript[] players;
	public Card[] augmentations;
	public Text gameStatus;

	/* Whose turn was first, and who is going now? */
	private int first_turn, turn;

	/* Temporary counter to pretend we have animations */
	private int counter;

	/* State machine for the game */
	public enum STATE { DRAW_NEW_CARD, WAITING_ON_AUGMENTATION, WAITING_ON_ACTION, WAITING_ON_DISCARDS, RESOLVE_ACTIONS, ANIMATING_ACTION };
	public STATE state { get; private set; }

	void Awake () {
		first_turn = 0;
	}

	// Use this for initialization
	void Start () {
		state = STATE.WAITING_ON_AUGMENTATION;

		UpdateStatus ();

		for (int i = 0; i < players.Length; i++) {
			players [i].ID = i;
			players [i].health = 35;

			// Tell players to draw an initial hand
			for (int n = 0; n < 3; n ++)
				players[i].Message(MESSAGE.DRAW);
		}
	}

	void Update() {
		if (state == STATE.RESOLVE_ACTIONS) {
			ResolveNextAction ();
		} else if (state == STATE.ANIMATING_ACTION) {
			if (counter-- <= 0) {
				for (int i = players.Length - 1; i >= 0; i --) {
					players [i].actionDisplay.Clear ();
				}

				// Cycle the first turn
				first_turn = (turn + players.Length - 1) % players.Length;

				SetState (STATE.WAITING_ON_DISCARDS);
			}
		}
	}

	void SetState(STATE newState) {
		state = newState;
		UpdateStatus ();

		switch (state) {
		case STATE.DRAW_NEW_CARD:
			players [turn].Message (MESSAGE.DRAW_NEW_CARD);
			break;
		case STATE.WAITING_ON_DISCARDS:
			foreach (PlayerScript player in players) {
				// Tell player it's time to draw
				player.Message (MESSAGE.DISCARD, 1);
			}
			break;
		case STATE.WAITING_ON_ACTION:
			// Tell all players to move
			foreach (PlayerScript player in players) {
				player.Message (MESSAGE.CHOOSE_ACTION);
			}
			break;
		}
	}

	void UpdateStatus() {
		AirconsoleLogic.SyncState ();

		switch (state) {
		case STATE.DRAW_NEW_CARD:
			gameStatus.text = "Waiting on player " + turn + " to draw a card";
			break;
		case STATE.WAITING_ON_AUGMENTATION:
			gameStatus.text = "Waiting on player " + turn + " to choose augmentation";
			break;
		case STATE.WAITING_ON_DISCARDS:
			gameStatus.text = "Discard down to 1 card!";
			break;
		case STATE.WAITING_ON_ACTION:
			gameStatus.text = "Waiting on player actions";
			break;
		case STATE.RESOLVE_ACTIONS:
			gameStatus.text = "Resolving actions";
			break;
		case STATE.ANIMATING_ACTION:
			gameStatus.text = "Animating actions";
			break;
		default:
			Debug.LogError ("State unaccounted for: " + state);
			gameStatus.text = "Error!";
			break;
		}
	}

	void Flop () {
		Flop (3);
	}

	void Flop (int count) {
		cardBank.Flop (count);
		turn = first_turn;
		SetState (STATE.DRAW_NEW_CARD);
	}

	public bool DrawCard(int player, Card card) {
		if (state != STATE.DRAW_NEW_CARD) {
			Debug.LogError ("You shouldn't be able to draw new cards right now");
		}

		Debug.Log ("Player " + player + " draws " + card);
		if (player == turn) {
			players [player].DrawCard (card);

			turn = (turn + 1) % players.Length;
			// Are we done drawing?
			if (turn == first_turn && cardBank.GetAvailableCards().Count - 1 < players.Length) {
				cardBank.Clear ();

				players [turn].Message (MESSAGE.CHOOSE_AUGMENTATION);
				SetState (STATE.WAITING_ON_AUGMENTATION);
			} else {
				// Message the player
				// Calling setState here so that it messages the player
				// saying pick a card!
				SetState (STATE.DRAW_NEW_CARD);
			}

			return true;
		}

		// Not your turn
		return false;
	}

	public bool CanPickAugmentation (int player_id) {
		return state == STATE.WAITING_ON_AUGMENTATION && turn == player_id;
	}

	public void PlayedAugmentation (int player_id) {
		if (!CanPickAugmentation (player_id)) {
			Debug.LogError ("Player " + player_id + " played out of turn!");
			return;
		}

		Debug.Log ("PLayer " + player_id + " played " + players [player_id].augmentation.name);

		// Next turn...
		turn = (turn + 1) % players.Length;

		if (turn == first_turn) {
			// If everyone played we're gucci!
			SetState (STATE.WAITING_ON_ACTION);
		} else {
			UpdateStatus ();

			players [turn].Message (MESSAGE.CHOOSE_AUGMENTATION);
		}
	}

	public void PlayedAction (int player_id) {
		// Check all players
		foreach (PlayerScript player in players) {
			if (player.action == null) {
				return;
			}
		}

		// All players chose an action!
		SetState(STATE.RESOLVE_ACTIONS);
	}

	void ResolveNextAction () {
		Debug.Log ("Player 2 action: " + players [1].action.name);

		for (int i = 0; i < players.Length; i ++) {
			PlayerScript player = players [i];

			if (player.augmentation.BeforeAugmentation != null)
				player.augmentation.BeforeAugmentation (player.augmentation, players[1 - i].augmentation);
		}

		for (int i = 0; i < players.Length; i ++) {
			PlayerScript player = players [i];

			if (player.augmentation.BeforeAction != null)
				player.augmentation.BeforeAction (player.action);
		}

		// Do stuff
		Card.ActionResult[] results = new Card.ActionResult[players.Length];
		results [0] = new Card.ActionResult ();
		results [1] = new Card.ActionResult ();

		for (int i = 0; i < players.Length; i ++) {
			PlayerAction playerAction = players [i].action;
			PlayerAction otherAction = players [1 - i].action;

			int damage = playerAction.techAttack + playerAction.physicalAttack;

			// Special case
			if (playerAction.name == "Attack" && otherAction.name == "Counter") {
				damage = 0;
			} else if (playerAction.name == "Counter" && otherAction.name == "Attack") {
				damage = playerAction.counterAttack;
			}

			results [i].damage = damage;
			results [1 - i].damageToSelf = damage;
		}

		for (int i = 0; i < players.Length; i ++) {
			PlayerScript player = players [i];

			if (player.augmentation.AfterAction != null)
				player.augmentation.AfterAction (results[i], player, players[1 - i]);
		}

		for (int i = 0; i < players.Length; i ++) {
			PlayerScript player = players [i];
			Card.ActionResult result = results [i];

			Debug.Log ("Dealing " + result.damageToSelf + " damage to player " + i);
			player.health -= result.damageToSelf;
		}

		SetState(STATE.ANIMATING_ACTION);
		counter = COMP_WAIT_TIME;
	}

	/* Confirm that all players have discarded */
	public bool ShouldDiscard (int player_id) {
		return state == STATE.WAITING_ON_DISCARDS && players [player_id].hand.Size > 1;
	}

	public void Discarded() {
		foreach (PlayerScript player in players) {
			if (player.hand.Size > 1) {
				UpdateStatus ();
				return;
			}
		}

		foreach (PlayerScript player in players) {
			// Reset player
			player.augmentation = null;
			player.action = null;

			// Tell player it's time to draw
			player.Message (MESSAGE.DRAW);
		}

		Flop ();
	}
}

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour {
	
	private static int COMP_WAIT_TIME = 100;

	public enum MESSAGE { DRAW, DRAW_NEW_CARD, CHOOSE_AUGMENTATION, CHOOSE_ACTION };

	public CardBankScript cardBank;
	public PlayerScript[] players;
	public Card[] augmentations;
	public Text gameStatus;

	/* Whose turn was first, and who is going now? */
	private int first_turn, turn;

	/* Temporary counter to pretend we have animations */
	private int counter;

	/* State machine for the game */
	public enum STATE { DRAW_NEW_CARD, WAITING_ON_AUGMENTATION, WAITING_ON_ACTION, RESOLVE_ACTIONS, ANIMATING_ACTION };
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

			SetState(STATE.ANIMATING_ACTION);
			counter = COMP_WAIT_TIME;
		} else if (state == STATE.ANIMATING_ACTION) {
			if (counter-- <= 0) {
				for (int i = players.Length - 1; i >= 0; i --) {
					players [i].actionDisplay.Clear ();
				}

				// Cycle the first turn
				first_turn = (turn + players.Length - 1) % players.Length;
				Flop ();

				foreach (PlayerScript player in players) {
					// Reset player
					player.augmentation = null;
					player.action = null;

					// Tell player it's time to draw
					player.Message (MESSAGE.DRAW);
				}

				SetState (STATE.DRAW_NEW_CARD);
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
		case STATE.WAITING_ON_ACTION:
			// Tell all players to move
			foreach (PlayerScript player in players) {
				player.Message (MESSAGE.CHOOSE_ACTION);
			}
			break;
		}
	}

	void UpdateStatus() {
		switch (state) {
		case STATE.DRAW_NEW_CARD:
			gameStatus.text = "Waiting on player " + turn + " to draw a card";
			break;
		case STATE.WAITING_ON_AUGMENTATION:
			gameStatus.text = "Waiting on player " + turn + " to choose augmentation";
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

		if (player == turn) {
			players [player].DrawCard (card);

			turn = (turn + 1) % players.Length;
			// Are we done drawing?
			if (turn == first_turn && cardBank.GetAvailableCards().Count - 1 < players.Length) {
				cardBank.Clear ();

				SetState (STATE.WAITING_ON_ACTION);
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

	void ResolveNextAction () {
//		Card playerCard = action [PLAYER_ID] [action_num];
//		Card aiCard = action [1] [action_num];
//
//		for (int i = players.Length - 1; i >= 0; i--) {
//			players[i].AffectCard(action[i][action_num] ,action_num);
//		}
//
//		// Calculate Combo stuff
//		playerCard.Combo (players[0].manaManager);
//		aiCard.Combo (players[1].manaManager);
//
//		// Do the actual battle
//		playerCard.Action (aiCard, players [PLAYER_ID], players [1]);
//		aiCard.Action (playerCard, players [1], players [PLAYER_ID]);
//
//		for (int i = players.Length - 1; i >= 0; i --) {
//			Card card = action [i] [action_num];
//			players [i].actionDisplay.Display (card);
//
//			// Game over?
//			if (players [i].health <= 0) {
//				if (players [i].health < players [1 - i].health) {
//					PlayerPrefs.SetInt ("Winner", 2 - i);
//				} else if (players [i].health == players [1 - i].health) {
//					PlayerPrefs.SetInt ("Winner", -1);
//				}
//
//				SceneManager.LoadScene ("GameOver");
//			}
//		}
	}
}

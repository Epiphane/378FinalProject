using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour {
	
	private static int PLAYER_ID = 0;
	private enum STATE { DRAW_NEW_CARD, WAITING_ON_ACTION, RESOLVE_ACTIONS, ANIMATING_ACTION };
	private static int COMP_WAIT_TIME = 50;

	public CardBankScript cardBank;
	public PlayerScript[] players;
	public Card[][] action;
	public Text gameStatus;

	private static int NUM_ACTIONS = 2;
	private int first_turn, turn;
	private int action_num, counter;
	private bool[] acted;
	private STATE state;

	void Awake () {
		// 2 actions per turn
		action = new Card[players.Length] [];
		acted = new bool[players.Length];
		first_turn = 0;
	}

	// Use this for initialization
	void Start () {
		Flop (8);
		state = STATE.DRAW_NEW_CARD;

		UpdateStatus ();

		for (int i = 0; i < players.Length; i++) {
			players [i].ID = i;
		}
	}

	void Update() {
		if (state == STATE.DRAW_NEW_CARD && turn != PLAYER_ID) {
			if (counter-- <= 0) {
				// AI turn
				((AIScript) players[turn]).PickCard(cardBank);
			}
		} else if (state == STATE.RESOLVE_ACTIONS) {
			SetState(STATE.ANIMATING_ACTION);
			counter = COMP_WAIT_TIME;
		} else if (state == STATE.ANIMATING_ACTION) {
			if (counter-- <= 0) {
				if (++action_num == NUM_ACTIONS) {
					// Cycle the first turn
					first_turn = (turn + players.Length - 1) % players.Length;
					Flop ();
					SetState (STATE.DRAW_NEW_CARD);
				} else {
					SetState(STATE.RESOLVE_ACTIONS);
				}
			}
		}
	}

	void SetState(STATE newState) {
		state = newState;
		UpdateStatus ();
	}

	void UpdateStatus() {
		switch (state) {
		case STATE.DRAW_NEW_CARD:
			if (turn == PLAYER_ID) {
				gameStatus.text = "Pick a card!";
			} else {
				gameStatus.text = "Waiting on opponent";
			}
			break;
		case STATE.WAITING_ON_ACTION:
			if (acted [PLAYER_ID] == false) {
				gameStatus.text = "Make a move!";
			} else {
				gameStatus.text = "Waiting on opponent";
			}
			break;
		case STATE.RESOLVE_ACTIONS:
			gameStatus.text = "Resolving action " + action;
			break;
		case STATE.ANIMATING_ACTION:
			gameStatus.text = "Animating action " + action;
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
		state = STATE.DRAW_NEW_CARD;
		turn = first_turn;
		
		// Let the computer "think" for a bit
		counter = COMP_WAIT_TIME;
	}

	public bool DrawCard(int player, Card card) {
		if (state != STATE.DRAW_NEW_CARD) {
			Debug.LogError ("You shouldn't be able to draw new cards right now");
		}

		if (player == turn) {
			players [player].DrawCard (card);

			turn = (turn + 1) % players.Length;
			// Are we done drawing?
			if (turn == first_turn && cardBank.GetAvailableCards().Count < players.Length) {
				SetState (STATE.WAITING_ON_ACTION);
			} else {
				UpdateStatus ();
			}

			// Let the computer "think" for a bit
			counter = COMP_WAIT_TIME;

			return true;
		}

		// Not your turn
		return false;
	}

	public void SetAction(int player, Card[] cards) {
		if (cards.Length != NUM_ACTIONS) {
			Debug.LogError ("Incorrect number of cards sent: " + cards.Length);
		}

		action [player] = cards;
		acted [player] = true;

		// Has everybody acted?
		for (int i = 0; i < acted.Length; i++) {
			if (!acted [i]) {
				return;
			}
		}

		// Everybody has acted!
		SetState(STATE.RESOLVE_ACTIONS);
		action_num = 0;
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour {
	
	private static int PLAYER_ID = 0;
	private enum STATE { DRAW_NEW_CARD, WAITING_ON_ACTION, RESOLVE_ACTIONS, ANIMATING_ACTION };
	private static int COMP_WAIT_TIME = 100;

	public enum MESSAGE { DRAW, DRAW_NEW_CARD, MAKE_ACTION };

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
			players [i].health = 16;
		}
	}

	void Update() {
		if (state == STATE.RESOLVE_ACTIONS) {
			ResolveNextAction ();

			SetState(STATE.ANIMATING_ACTION);
			counter = COMP_WAIT_TIME;
		} else if (state == STATE.ANIMATING_ACTION) {
			if (counter-- <= 0) {
				if (++action_num == NUM_ACTIONS) {
					for (int i = players.Length - 1; i >= 0; i --) {
						players [i].actionDisplay.Clear ();
					}

					// Cycle the first turn
					first_turn = (turn + players.Length - 1) % players.Length;
					Flop ();

					// Tell players to draw
					foreach (PlayerScript player in players) {
						player.special = player.nextSpecial;
						player.nextSpecial = 0;

						player.Message (MESSAGE.DRAW);
					}

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

		switch (state) {
		case STATE.DRAW_NEW_CARD:
			players [turn].Message (MESSAGE.DRAW_NEW_CARD);
			break;
		case STATE.WAITING_ON_ACTION:
			// Tell all players to move
			foreach (PlayerScript player in players) {
				player.Message (MESSAGE.MAKE_ACTION, NUM_ACTIONS);
			}
			break;
		}
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
			gameStatus.text = "Resolving action " + action_num;
			break;
		case STATE.ANIMATING_ACTION:
			gameStatus.text = "Animating action " + action_num;
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

	void ResolveNextAction () {
		Card playerCard = action [PLAYER_ID] [action_num];
		Card aiCard = action [1] [action_num];

		for (int i = players.Length - 1; i >= 0; i--) {
			players[i].AffectCard(action[i][action_num] ,action_num);
		}

		// Calculate Combo stuff
		playerCard.Combo (players[0].manaManager);
		aiCard.Combo (players[1].manaManager);

		// Do the actual battle
		playerCard.Action (aiCard, players [PLAYER_ID], players [1]);
		aiCard.Action (playerCard, players [1], players [PLAYER_ID]);

		for (int i = players.Length - 1; i >= 0; i --) {
			Card card = action [i] [action_num];
			players [i].actionDisplay.Display (card);

			// Resolve special effects
			switch (card.GetSpecial()) {
			case Card.Special.ComboBreak:
				players [1 - i].manaManager.Clear ();
				break;
			case Card.Special.FutureShield:
				players [i].nextSpecial |= PlayerScript.FUTURE_SHIELD;
				break;
			case Card.Special.SeeOpponentHand:
				players [i].nextSpecial |= PlayerScript.SEE_OPPONENT_HAND;
				break;
			case Card.Special.SunderArmor:
				players [i].nextSpecial |= PlayerScript.SUNDER_ARMOR;
				break;
			}
		}
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
		for (int i = 0; i < acted.Length; i++)
			acted [i] = false;
		action_num = 0;
	}
}

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour {
	
	private static int COMP_WAIT_TIME = 100;
	public static int INITIAL_HEALTH = 20;

	public enum MESSAGE { CHOOSE_SCHOOL, DRAW, DISCARD, DISCARD_ALL, DRAW_NEW_CARD, CHOOSE_AUGMENTATION, CHOOSE_ACTION };

	public CardBankScript cardBank;
	public PlayerScript[] players;
	public Card[] augmentations;
	public Text gameStatus;

	/* The cards currently flopped out. Hurry up and choose one already! */
	public List<Card> floppedCards;

	/* Whose turn was first, and who is going now? */
	private int first_turn, turn;

    /* if nothing was in the way, whose turn WOULD it be? */
    private int temp_first_turn;

	/* Temporary counter to pretend we have animations */
	private int counter;

	/* State machine for the game */
	public enum STATE { WAITING_ON_SCHOOL, DRAW_NEW_CARD, WAITING_ON_AUGMENTATION, WAITING_ON_ACTION, WAITING_ON_DISCARDS, RESOLVE_ACTIONS, ANIMATING_ACTION, GAME_OVER };
	public STATE state { get; private set; }

	void Awake () {
		first_turn = temp_first_turn = 0;
	}

	// Use this for initialization
	void Start () {
		state = STATE.WAITING_ON_SCHOOL;

		UpdateStatus ();

		for (int i = 0; i < players.Length; i++) {
			players [i].ID = i;
			players [i].health = INITIAL_HEALTH;
			players [i].max_health = INITIAL_HEALTH;

			players[i].Message(MESSAGE.CHOOSE_SCHOOL);
		}
	}

	void Update() {
		if (state == STATE.RESOLVE_ACTIONS) {
			ResolveNextAction ();

		} else if (state == STATE.ANIMATING_ACTION) {
			for (int i = players.Length - 1; i >= 0; i--) {
				players [i].actionDisplay.DisplayAction (players [i].action);
			}

			if (counter-- <= 0) {
				for (int i = players.Length - 1; i >= 0; i --) {
					players [i].actionDisplay.Clear ();
				}

                // Cycle the first turn
                temp_first_turn = (temp_first_turn + players.Length - 1) % players.Length;

				// Tell players to draw 2 cards
				Flop ();
				foreach (PlayerScript player in players) {
					player.augmentation = null;
					player.action = null;

					player.Message (MESSAGE.DRAW, 2);
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

		AirconsoleLogic.SyncState ();
	}

	void UpdateStatus() {

		switch (state) {
		case STATE.DRAW_NEW_CARD:
			gameStatus.text = "Waiting on player " + turn + " to draw a card";
			break;
		case STATE.WAITING_ON_SCHOOL:
			gameStatus.text = "Waiting on player " + turn + " to choose school";
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
		case STATE.GAME_OVER:
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
        first_turn = temp_first_turn;
        if (players[0].school.FirstPick() != players[1].school.FirstPick())
        {
            if (players[0].school.FirstPick())
                first_turn = 0;
            else
                first_turn = 1;
        }

        floppedCards = cardBank.Flop (count);
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

                // Pick who picks first augmentation
                first_turn = temp_first_turn;
                if (players[0].school.SecondMove() != players[1].school.SecondMove())
                {
                    if (players[0].school.SecondMove())
                        first_turn = 1;
                    else
                        first_turn = 0;
                }
                turn = first_turn;

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

		// Next turn...
		turn = (turn + 1) % players.Length;

		bool complete = false;
		if (turn == first_turn) {
			complete = true;

			// Everyone played! See if we have any chains going
			for (int i = 0; i < players.Length; i++) {
				PlayerScript player = players [i];

				player.augmentation.Instant (player.augmentation, players [1 - i].augmentation, player, players[1 - i]);
			}

			// Determine whether we're done with augmentation-ing
			foreach (PlayerScript player in players)
				if (player.augmentation.chainable && player.hand.Size > 0)
					complete = false;
		}

		if (complete) {
			// If everyone played we're gucci!
			SetState (STATE.WAITING_ON_ACTION);
		} else {
			UpdateStatus ();

			// See whether this player can actually go
			if (players [turn].augmentation == null || players [turn].augmentation.chainable)
				players [turn].Message (MESSAGE.CHOOSE_AUGMENTATION);
			else
				// Otherwise pretend they played
				PlayedAugmentation (turn);
		}
	}

	public void PlayedAction (int player_id) {
		// Tell this player to discard the rest of their cards (unless they have perseverance)
		players[player_id].Message (MESSAGE.DISCARD_ALL);

		// Check all players
		foreach (PlayerScript player in players) {
			if (player.action == null) {
				return;
			}
		}

		// All players chose an action!
		SetState(STATE.RESOLVE_ACTIONS);
	}

	/* Called after a player has successfully set their class */
	public void SchoolSelected (int player_id) {
		// Check all players
		foreach (PlayerScript player in players) {
			if (player.school == null) {
				return;
			}
		}

		// All players chose a school
		SetState (STATE.WAITING_ON_AUGMENTATION);
	}

	void ResolveNextAction () {
		Debug.Log ("Player 2 action: " + players [1].action.name);

		for (int i = 0; i < players.Length; i ++) {
			PlayerScript player = players [i];

			player.school.BeforeAction (player.action);

			// Augmentation effects
			player.BeforeAction (player.action);
		}

		// Do stuff
		Card.ActionResult[] results = new Card.ActionResult[players.Length];
		results [0] = new Card.ActionResult ();
		results [1] = new Card.ActionResult ();

		for (int i = 0; i < players.Length; i ++) {
			PlayerAction playerAction = players [i].action;
			PlayerAction otherAction = players [1 - i].action;

			int damage = Mathf.Max(playerAction.techAttack + playerAction.physicalAttack - otherAction.defense, 0);

			// Special case
			if (playerAction.name == "Attack" && otherAction.name == "Counter") {
				damage = 0;
			} else if (playerAction.name == "Counter" && otherAction.name == "Attack") {
				damage = playerAction.counterAttack;
			}

			results [i].action = playerAction;
			results [i].damage = damage;
			results [i].advancement = playerAction.advancement;
		}

		for (int i = 0; i < players.Length; i++) {
			PlayerScript player = players [i];

			// Augmentation effects
			player.AfterActionBeforeSchool (results [i], results[1 - i]);
		}

		for (int i = 0; i < players.Length; i++) {
			PlayerScript player = players [i];

			player.school.AfterAction (results[i], results[1 - i]);
		}

		for (int i = 0; i < players.Length; i++) {
			PlayerScript player = players [i];

			// Augmentation effects
			player.AfterAction (results [i], results[1 - i]);
		}

		for (int i = 0; i < players.Length; i ++) {
			PlayerScript player = players [i];
			Card.ActionResult result = results [i];

			player.health -= results [1 - i].damage;
			player.health += results [i].healing;
			player.school.advancement += result.advancement;

			player.Message (MESSAGE.DRAW, result.extraCards);
		}

		if (players [0].health > 0 && players [1].health > 0) {
			SetState (STATE.ANIMATING_ACTION);
			counter = COMP_WAIT_TIME;
		}
		else {
			// Someeebody lost
			SetState (STATE.GAME_OVER);

			if (players [0].health < players [1].health)
				gameStatus.text = "Opponent wins!";
			else if (players [0].health == players [1].health)
				gameStatus.text = "Tie game!";
			else
				gameStatus.text = "You win!";
		}
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

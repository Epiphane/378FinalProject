using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * PlayerScript that manages AI decision making. Right now it just
 * simply chooses everything randomly, but we can add strategy in
 * here at any time
 */
public class AIScript : PlayerScript {

	// Reference to the Deck that lays out new cards, so we can see them
	public CardBankScript cardBank;

	// Wait a second before making decisions
	private static int THINK_TIME = 1;

	// Use this for initialization
	public void Start () {
		if (cardBank == null) {
			GameObject CB = GameObject.Find ("CardBank");

			if (CB == null) {
				Debug.LogError ("CardBank not found!");
				return;
			}

			cardBank = CB.GetComponent<CardBankScript> ();
		}
	}

	// Pick a card, any card! (from the Flop)
	public void PickCard() {
		List<Card> options = cardBank.GetAvailableCards ();

		cardBank.PickCard (ID, options [Random.Range (0, options.Count)]);
	}

	// Choose an action to make
	public void DoAction() {
		this.PlayAction (PlayerAction.GetRandom ());
	}

	// Choose an augmentation to play
	public void ChooseAugmentation() {
		PlayAugmentation (hand.cards [Random.Range (0, hand.cards.Count)]);
	}

	// Discard extra cards in your hand!
	public void DiscardToOne() {
		while (hand.Size > 1) {
			Discard(hand.cards [0]);
		}

		gameManager.Discarded ();
	}

	/* Select a school */
	public void SelectSchool() {
		this.SetSchool(PlayerSchool.schools [Random.Range (0, 3)].Clone ());
	}

	/* For receiving information from the game state */
	public override void Message(GameManagerScript.MESSAGE message, object data) {
		base.Message (message, data);

		switch (message) {
		case GameManagerScript.MESSAGE.DRAW_NEW_CARD:
			// Think for a bit before picking a random card
			// ALSO: You can't directly call PickCard() from here because itll get all weird on ya
			// Ask Thomas if you want to know why, its gross
			Invoke ("PickCard", THINK_TIME);
			break;
		case GameManagerScript.MESSAGE.CHOOSE_AUGMENTATION:
			// See above
			Invoke ("ChooseAugmentation", THINK_TIME);
			break;
		case GameManagerScript.MESSAGE.CHOOSE_ACTION:
			// See above
			Invoke ("DoAction", THINK_TIME);
			break;
		case GameManagerScript.MESSAGE.DISCARD:
			Invoke ("DiscardToOne", THINK_TIME);
			break;
		case GameManagerScript.MESSAGE.CHOOSE_SCHOOL:
			Invoke ("SelectSchool", THINK_TIME);
			break;
		}
	}
}

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardBankScript : CardHolderScript {
	
	private DeckScript deck;
	private GameManagerScript game;

	// Use this for initialization
	public override void Awake () {
		base.Awake ();

		if (deck == null)
			deck = GetComponent<DeckScript> ();

		// For now, we're going to initialize a bank with 63 cards
		// - 21 each of red, blue, green
		// - 21 each of atk, spell, block
		// Giving us 7 of each type of card
		for (int i = 0; i < Card.colors.Length; i++) {
			Card.Color color = Card.colors [i];

			for (int j = 0; j < Card.types.Length; j++) {
				Card.Type type = Card.types [j];

				// Create 7 cards
				for (int n = 0; n < 7; n++) {
					deck.AddCard (new Card (type, color));
				}
			}
		}

		deck.Shuffle ();
	}

	void Start () {
		GameObject GM = GameObject.Find ("GameManager");

		if (GM == null) {
			Debug.LogError ("No GameManager found!");
		}

		game = GM.GetComponent<GameManagerScript> ();
	}

	public void Flop (int amountToDraw) {
		float yoffset = Mathf.Floor (amountToDraw / 4);// * 1.4f;

		while (amountToDraw-- > 0) {
			GameObject newCard = AddCard (deck.Draw ());

			newCard.transform.position = transform.position - new Vector3 ((amountToDraw % 4 + 1) * 2.6f, -Mathf.Floor(amountToDraw / 4) * 3 + yoffset, 0);
		}
	}

	public override void Reorganize () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public List<Card> GetAvailableCards() {
		return cards;
	}

	public void PickCard (int player_id, Card card) {
		if (game.DrawCard (player_id, card)) {
			RemoveCard (card);
		}
	}

	public override void CardSelected (Transform transform, Card card) {
		// Pass in 0 (the human player) because this is coming from a click event...
		// TODO is this a good idea? Who knows!
		PickCard(0, card);
	}
}

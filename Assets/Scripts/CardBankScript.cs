﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardBankScript : CardHolderScript {
	
	public DeckScript deck;
	public GameManagerScript game;

	// Use this for initialization
	public override void Awake () {
		base.Awake ();

		if (deck == null)
			deck = GetComponent<DeckScript> ();

		// Put 3 of each starter in the deck
		for (int i = 0; i < 12; i++) {
			for (int n = 0; n < 3; n++) {
				deck.AddCard (Card.cards [i].Clone ());
			}
		}

		// Put one of each special card in the deck
		for (int i = 12; i < Card.cards.Length; i++) {
			deck.AddCard (Card.cards [i].Clone ());
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

	public List<Card> Flop (int amountToDraw) {
		float yoffset = Mathf.Floor (amountToDraw / 4);// * 1.4f;

		List<Card> cards = new List<Card> ();

		while (amountToDraw-- > 0) { // TODO: also stop if the deck runs out
			Card newCard = deck.Draw ();
			GameObject newCardObject = AddCard (newCard);
			cards.Add (newCard);

			newCardObject.transform.position = transform.position - new Vector3 ((amountToDraw % 4 + 1) * 140, -Mathf.Floor(amountToDraw / 4) * 3 + yoffset, 0);
		}

		return cards;
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

		AirconsoleLogic.SyncState ();
	}

	public override void CardSelected (Transform transform, Card card) {
		// Pass in 0 (the human player) because this is coming from a click event...
		// TODO is this a good idea? Who knows!
		PickCard(0, card);
	}
}

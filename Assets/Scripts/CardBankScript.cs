using UnityEngine;
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
		
		for (int i = 0; i < Card.cards.Length; i++) {
			for (int n = 0; n < 6; n++) {
				deck.AddCard (Card.cards [i].Clone ());
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

			newCard.transform.position = transform.position - new Vector3 ((amountToDraw % 4 + 1) * 100, -Mathf.Floor(amountToDraw / 4) * 3 + yoffset, 0);
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

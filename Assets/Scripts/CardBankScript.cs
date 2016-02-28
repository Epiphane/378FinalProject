using UnityEngine;
using System.Collections;

public class CardBankScript : CardHolderScript {

	private Deck deck;

	private GameManagerScript game;

	// Use this for initialization
	void Awake () {
		deck = new Deck ();

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
		while (amountToDraw-- > 0) {
			GameObject newCard = AddCard (deck.Draw ());

			newCard.transform.position = transform.position - new Vector3 ((amountToDraw + 1) * 3, 0, 0);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void CardSelected (Transform transform, Card card) {
		Destroy (transform.gameObject);

		// Pass in 0 (the human player) because this is coming from a click event...
		// TODO is this a good idea? Who knows!
		game.DrawCard (0, card);
	}
}

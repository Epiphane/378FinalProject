using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class DeckScript : MonoBehaviour {

	public Text output;
	private List<Card> cards;

	public int length {
		get {
			return cards.Count;
		}
	}

	void Awake() {
		cards = new List<Card> ();
	}

	void UpdateDisplay () {
		if (output != null) {
			output.text = "Deck: " + cards.Count;
		}
	}

	public Card Draw () {
		Card next = cards [0];
		cards.RemoveAt (0);

		UpdateDisplay ();
		return next;
	}

	public void AddCard (Card card) {
		cards.Add (card);

		UpdateDisplay ();
	}

	public void Shuffle () {
		int n = cards.Count;
		while (n > 1) {
			int k = Random.Range (0, n);
			n--;
			Card value = cards[k];
			cards[k] = cards[n];
			cards[n] = value;
		}
	}

	public static Card RandomCard() {
		// Decide on a type
		Card.Type t = Card.types[Random.Range (0, Card.types.Length)];

		// Decide on a color
		Card.Color c = Card.colors[Random.Range (0, Card.colors.Length)];

		return new Card (t, c);
	}

}
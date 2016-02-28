using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HandScript : MonoBehaviour {

	private static float SPACING = 1;

	private List<Transform> cards = new List<Transform>();

	public GameObject cardPrefab;

	// Use this for initialization
	void Start () {
		// Add a dummy hand
		for (int i = 0; i < 3; i++) {
			GameObject card = AddCard (Deck.RandomCard ());
		}
	}

	GameObject AddCard(Card card) {
		GameObject newCard = GameObject.Instantiate (cardPrefab);

		// Set card to random type
		newCard.GetComponent<CardDisplayScript>().SetCard(card);
		newCard.transform.parent = transform;

		cards.Add (newCard.transform);
		ReorganizeHand ();

		return newCard;
	}

	void ReorganizeHand () {
		for (int i = 0; i < cards.Count; i++) {
			cards[i].transform.position = new Vector3(SPACING * (cards.Count - 2 * i), 0, 0);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

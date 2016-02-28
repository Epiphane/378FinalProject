using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HandScript : MonoBehaviour {

	private static float FAN_WIDTH = 1;
	private static float FAN_DEG = 3;

	private List<Transform> cards = new List<Transform>();

	public GameObject cardPrefab;
	public bool fan = true;

	// Use this for initialization
	void Start () {
		// Add a dummy hand
		for (int i = 0; i < 10; i++) {
			GameObject card = AddCard (Deck.RandomCard ());
		}

		ReorganizeHand ();
	}

	GameObject AddCard(Card card) {
		GameObject newCard = GameObject.Instantiate (cardPrefab);

//		GameObject anchor = new GameObject ();

		// Set card to random type
		newCard.GetComponent<CardDisplayScript>().SetCard(card);
//		newCard.transform.parent = anchor.transform;
//
//		anchor.transform.parent = transform;
		newCard.transform.parent = transform;

		cards.Add (newCard.transform);
//		ReorganizeHand ();

		return newCard;
	}

	void ReorganizeHand () {
		float leftMost = 0;

		for (int i = 0; i < cards.Count; i++) {
			float tilt = (cards.Count - 2.0f * i - 1) * FAN_WIDTH;
			float xval = tilt * -3 / cards.Count;
			// Set anchor
			cards [i].transform.position = transform.position + new Vector3(xval, 0, -i / 100.0f);
			cards [i].transform.rotation = Quaternion.Euler(new Vector3 (0, 0, 0));
			cards [i].transform.RotateAround (transform.position + new Vector3(xval, -10, 0), Vector3.forward, tilt);
		}
	}
	
	// Update is called once per frame
	void Update () {
//		ReorganizeHand (FAN_DEG);
	}
}

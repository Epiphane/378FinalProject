using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HandScript : CardHolderScript {

	private static float FAN_WIDTH = 1;

	public bool fan = true;

	// Use this for initialization
	void Start () {
		// Add a dummy hand
		for (int i = 0; i < 3; i++) {
			AddCard (Deck.RandomCard ());
		}
	}

	public override GameObject AddCard(Card card) {
		GameObject newCard = base.AddCard (card);
		ReorganizeHand ();

		return newCard;
	}

	public override void CardSelected (Transform transform, Card card) {
		RemoveCard (transform);
		Destroy (transform.gameObject);

		ReorganizeHand ();
	}

	void ReorganizeHand () {
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
	}
}

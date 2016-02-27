using UnityEngine;
using System.Collections;

public class HandScript : MonoBehaviour {

	private Transform[] cards;

	public GameObject cardPrefab;

	// Use this for initialization
	void Start () {
		// Add a dummy hand
		for (int i = 0; i < 3; i++) {
			GameObject card = AddDummyCard ();

			card.transform.position = new Vector3 (3 - 2 * i, 0, 0);
		}
	}

	GameObject AddDummyCard() {
		GameObject newCard = GameObject.Instantiate (cardPrefab);

		// Set card to random type
		CardDisplayScript.Color c = CardDisplayScript.Color.BLUE;
		CardDisplayScript.Type t = CardDisplayScript.Type.ATTACK;
		newCard.GetComponent<CardDisplayScript> ().SetColorAndType (c, t);

		newCard.transform.parent = transform;
		return newCard;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

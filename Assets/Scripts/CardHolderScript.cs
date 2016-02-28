using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardHolderScript : MonoBehaviour {

	public GameObject cardPrefab;

	protected List<Card> cards;
	protected List<Transform> cardTransforms;

	// Use this for initialization
	public virtual void Awake () {
		cards = new List<Card> ();
		if (cardPrefab != null) {
			cardTransforms = new List<Transform> ();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public virtual GameObject AddCard(Card card) {
		GameObject newCard = null;

		if (cardPrefab != null) {
			newCard = GameObject.Instantiate (cardPrefab);

			// Set card to random type
			newCard.GetComponent<CardDisplayScript> ().card = card;
			newCard.transform.parent = transform;

			cardTransforms.Add (newCard.transform);
		}

		cards.Add (card);

		return newCard;
	}
		
	public virtual void RemoveCard (Card card) {
		int index = cards.IndexOf (card);
		cards.RemoveAt (index);

		if (cardPrefab != null) {
			Destroy (cardTransforms[index].gameObject);
			cardTransforms.RemoveAt (index);
		}
	}

	public virtual void CardSelected (Transform transform, Card card) {
		Debug.LogError ("CardSelected not implemented!");
	}
}

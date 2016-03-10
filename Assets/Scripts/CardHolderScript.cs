using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardHolderScript : MonoBehaviour {

	public GameObject cardPrefab;

	protected List<Card> _cards;
	protected List<Transform> cardTransforms;

	public delegate void ShouldPickCard(Card card);
	public ShouldPickCard OnPickCard;
	public bool secret = false;

	/* Calculated variables */
	public List<Card> cards {
		get {
			return _cards;
		}
	}

	public int Size {
		get {
			return cards.Count;
		}
	}

	// Use this for initialization
	public virtual void Awake () {
		_cards = new List<Card> ();
		if (!secret) {
			cardTransforms = new List<Transform> ();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public virtual Vector3 StartingPoint() {
		return transform.position;
	}

	public virtual GameObject AddCard(Card card) {
		GameObject newCard = null;

		if (!secret) { // Don't show the AI's cards.
			newCard = GameObject.Instantiate (cardPrefab);

			// Set card to random type
			newCard.GetComponent<CardDisplayScript> ().card = card;
			newCard.transform.SetParent (transform);
			newCard.transform.position = StartingPoint();

			cardTransforms.Add (newCard.transform);
		}


		_cards.Add (card);

		if (!secret) {
			Reorganize ();
		}

		return newCard;
	}
		
	public virtual void RemoveCard (Card card) {
		int windex = _cards.IndexOf (card);
		if (windex < 0)
			return;

		_cards.RemoveAt (windex);

		if (!secret) {
			Destroy (cardTransforms[windex].gameObject);
			cardTransforms.RemoveAt (windex);
			Reorganize ();
		}
	}

	public void Clear () {
		while (_cards.Count > 0) {
			RemoveCard (_cards [0]);
		}
	}

	public virtual void Reorganize() {
		Debug.LogError ("Reorganize not implemented!");
	}

	public virtual void CardSelected (Transform transform, Card card) {
		Debug.LogError ("CardSelected not implemented!");
	}
}

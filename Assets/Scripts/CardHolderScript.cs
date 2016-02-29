using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardHolderScript : MonoBehaviour {

	public GameObject cardPrefab;

	protected List<Card> _cards;
	protected List<Transform> cardTransforms;

	public delegate bool ShouldPickCard(Card card);
	public ShouldPickCard OnPickCard;

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

		_cards.Add (card);

		if (cardPrefab != null)
			Reorganize ();

		return newCard;
	}
		
	public virtual void RemoveCard (Card card) {
		int index = _cards.IndexOf (card);
		if (index < 0)
			return;

		_cards.RemoveAt (index);

		if (cardPrefab != null) {
			Destroy (cardTransforms[index].gameObject);
			cardTransforms.RemoveAt (index);
		}

		if (cardPrefab != null)
			Reorganize ();
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

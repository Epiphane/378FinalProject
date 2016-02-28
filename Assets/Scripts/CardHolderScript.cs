using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardHolderScript : MonoBehaviour {

	public GameObject cardPrefab;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	protected List<Transform> cards = new List<Transform>();

	public virtual GameObject AddCard(Card card) {
		GameObject newCard = GameObject.Instantiate (cardPrefab);

		// Set card to random type
		newCard.GetComponent<CardDisplayScript>().card = card;
		newCard.transform.parent = transform;

		cards.Add (newCard.transform);

		return newCard;
	}
		
	public virtual void RemoveCard (Transform card) {
		cards.Remove (card);
	}

	public virtual void CardSelected (Transform transform, Card card) {
		Debug.LogError ("CardSelected not implemented!");
	}
}

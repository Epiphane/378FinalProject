using UnityEngine;
using System.Collections;

public class CardDisplayScript : MonoBehaviour {
	/* Objects to modify */
	private SpriteRenderer cardType;
	private SpriteRenderer cardFrame;

	/* Card object */
	private Card card = null;

	// Use this for initialization
	void Start () {
		cardFrame = transform.FindChild ("Frame").GetComponent<SpriteRenderer>();
		cardType = transform.FindChild ("Display").GetComponent<SpriteRenderer>();

		if (card != null)
			SetCard (card);
	}

	public void SetCard(Card card) {
		this.card = card;

		// Set type image
		if (cardType != null)
			cardType.sprite = CardDisplayManager.instance.DisplayType (card.GetType());

		// Set frame
		if (cardFrame != null)
			cardFrame.sprite = CardDisplayManager.instance.DisplayColor (card.GetColor());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseEnter() {
	}

	void OnMouseExit() {
	}

	void OnMouseUp() {
		HandScript hand = transform.parent.GetComponent<HandScript> ();

		if (hand != null) {
			hand.RemoveCard (transform);
		} 
		else {
			Debug.Log ("Removing Game Object but it's not in any hand");
			Destroy (gameObject);
		}
	}
}
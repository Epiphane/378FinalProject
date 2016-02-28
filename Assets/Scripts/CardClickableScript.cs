using UnityEngine;
using System.Collections;

public class CardClickableScript : MonoBehaviour {

	/* Card object */
	private CardDisplayScript cardDisplay = null;

	void Start() {
		cardDisplay = GetComponent<CardDisplayScript> ();
	}

	void OnMouseEnter() {
	}

	void OnMouseExit() {
	}

	void OnMouseUp() {
		// Are we in a hand?
		CardHolderScript parent = transform.parent.GetComponent<CardHolderScript> ();

		if (parent != null) {
			parent.CardSelected (transform, (cardDisplay != null) ? cardDisplay.card : null);
		} 
		else {
			Debug.Log ("Nothing is managing this Card!");
			Destroy (gameObject);
		}
	}
}

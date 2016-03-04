using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CardDisplayScript : MonoBehaviour {
	/* Objects to modify */
	private SpriteRenderer cardFrame;
	public Text description;

	/* Card object */
	private Card _card = null;

	// Use this for initialization
	void Start () {
		cardFrame = transform.FindChild ("Frame").GetComponent<SpriteRenderer>();
		description = transform.FindChild ("Description").GetComponent<Text>();

		if (_card != null)
			UpdateDisplay ();
	}

	public Card card {
		set {
			_card = value;

			UpdateDisplay ();
		}
		get {
			return _card;
		}
	}

	void UpdateDisplay() {
		// Set frame
		if (cardFrame != null)
			cardFrame.sprite = CardDisplayManager.instance.DisplayColor (_card.color);

		if (description != null)
			description.text = card.description;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
using UnityEngine;
using System.Collections;

public class CardDisplayScript : MonoBehaviour {
	/* Objects to modify */
	private SpriteRenderer cardType;
	private SpriteRenderer cardFrame;

	/* Card object */
	private Card _card = null;

	// Use this for initialization
	void Start () {
		cardFrame = transform.FindChild ("Frame").GetComponent<SpriteRenderer>();
		cardType = transform.FindChild ("Display").GetComponent<SpriteRenderer>();

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
		// Set type image
		if (cardType != null)
			cardType.sprite = CardDisplayManager.instance.DisplayType (_card.type);

		// Set frame
		if (cardFrame != null)
			cardFrame.sprite = CardDisplayManager.instance.DisplayColor (_card.color);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
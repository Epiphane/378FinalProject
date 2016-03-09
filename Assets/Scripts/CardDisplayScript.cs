using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CardDisplayScript : MonoBehaviour {
	/* Objects to modify */
	private Image cardFrame;
	public Text description, title;

	/* Card object */
	private Card _card = null;

	// Use this for initialization
	void Start () {
		if (transform.FindChild ("Frame") != null)
			cardFrame = transform.FindChild ("Frame").GetComponent<Image>();
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
			cardFrame.color = CardDisplayManager.instance.DisplayColor (_card.color);

		if (description != null)
			description.text = card.description;

		if (title != null)
			title.text = card.name;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
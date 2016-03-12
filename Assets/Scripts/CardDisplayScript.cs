using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CardDisplayScript : MonoBehaviour {
	/* Objects to modify */
	private Image cardFrame;
	public Text description;

	string description_base = @"<size=14><b>{0}</b> </size>
<size=2>.</size>
<size=12>{1}</size>";

	public Sprite redImg;
	public Sprite greenImg;
	public Sprite blueImg;

	/* Card object */
	private Card _card = null;

	// Use this for initialization
	void Start () {
		var frameTransform = transform.FindChild ("Frame");
		if (frameTransform != null) {
			cardFrame = frameTransform.GetComponent<Image> ();
			description = frameTransform.FindChild ("Description").GetComponent<Text>();
		}

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
    
		// Set card image
		Sprite cardSprite = null;

		if (cardFrame != null) {
			switch (_card.color) {
			case Card.Color.RED:
				cardSprite = redImg;
				break;
			case Card.Color.BLUE:
				cardSprite = blueImg;
				break;
			case Card.Color.GREEN:
				cardSprite = greenImg;
				break;
			default:
				Debug.LogWarning ("Invalid color for card: " + _card.color);
				break;
			}
			cardFrame.sprite = cardSprite;
		}

		if (description != null) {
			description.text = System.String.Format (description_base, _card.name, _card.description);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
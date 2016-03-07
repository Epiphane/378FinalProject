using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/**
 * This component is going to be what manages the animation state of the players.
 * 
 * In essence, the GameManager will tell each player's ActionDisplayScript what
 * the action resolved to, and these components will kick off animations (to
 * reflect what they did, what happened, etc)
 */
public class ActionDisplayScript : MonoBehaviour {

	// Image to display the action type
	public Image actionSprite;

	// Sprites that could get displayed
	public Sprite attack, tech, counter;

	// Prefabs for showing the augmentation
	public GameObject cardPrefab;
	private GameObject display;

	// Display the action itself
	public void DisplayAction(PlayerAction action) {
		actionSprite.enabled = true;
		switch (action.name) {
		case "Attack":
			actionSprite.sprite = attack;
			break;
		case "Counter":
			actionSprite.sprite = counter;
			break;
		case "Tech":
			actionSprite.sprite = tech;
			break;
		}
	}

	// Show the augmentation selected
	public void DisplayAugmentation(Card augmentation) {
		Clear ();

		if (cardPrefab != null) {
			display = GameObject.Instantiate (cardPrefab);

			display.GetComponent<CardDisplayScript> ().card = augmentation;

			// Animate the card from center to it's position
			display.transform.position = Vector3.zero;
			display.transform.SetParent(transform);
			display.GetComponent<UISmoothTransformScript> ().MoveTo (transform.position - new Vector3(0, 150, 0), 0.2f);
		}
	}

	public void Clear() {
		actionSprite.sprite = null;
		actionSprite.enabled = false;

		if (display != null) {
			Destroy (display);
			display = null;
		}
	}
		
}

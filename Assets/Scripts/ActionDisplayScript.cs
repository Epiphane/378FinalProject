using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ActionDisplayScript : MonoBehaviour {

	public GameObject cardPrefab;
	public Image actionSprite;

	public Sprite attack, tech, counter;

	private GameObject display;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

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

	public void DisplayAugmentation(Card augmentation) {
		Clear ();

		if (cardPrefab != null) {
			display = GameObject.Instantiate (cardPrefab);

			// Set card to random type
			display.GetComponent<CardDisplayScript> ().card = augmentation;
			display.transform.position = transform.position - new Vector3(0, 150, 0);
			display.transform.SetParent(transform);
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

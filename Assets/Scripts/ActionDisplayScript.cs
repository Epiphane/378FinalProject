using UnityEngine;
using System.Collections;

public class ActionDisplayScript : MonoBehaviour {

	public GameObject cardPrefab;

	private GameObject display;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Display(Card action) {
		Clear ();

		if (cardPrefab != null) {
			display = GameObject.Instantiate (cardPrefab);

			// Set card to random type
			display.GetComponent<CardDisplayScript> ().card = action;
			display.transform.position = transform.position;
			display.transform.parent = transform;
		}
	}

	public void Clear() {
		if (display != null) {
			Destroy (display);
			display = null;
		}
	}
		
}

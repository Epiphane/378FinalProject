using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour {

	public GameManagerScript gameManager;
	public CardHolderScript hand;
	public int ID;

	protected int numActions = 0;

	// Use this for initialization
	public virtual void Start () {
		if (gameManager == null) {
			GameObject GM = GameObject.Find ("GameManager");

			if (GM == null) {
				Debug.LogError ("GameManager not found!");
				return;
			}

			gameManager = GM.GetComponent<GameManagerScript> ();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public virtual void DrawCard(Card card) {
		hand.AddCard (card);
	}

	/* For receiving information from the game state */
	public virtual void Message(GameManagerScript.MESSAGE message, object data = null) {
		Debug.Log ("Received message " + message);
	}
}

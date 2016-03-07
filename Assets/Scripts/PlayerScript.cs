using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour {

	public GameManagerScript gameManager;
	public CardHolderScript hand;
	public DeckScript deck;
	public ActionDisplayScript actionDisplay;
	public PlayerDashboardScript dashboard;
	public int ID;

	/* Augmentation for the current round */
	public Card augmentation = null;
	public PlayerAction action = null;

	/* Health GUI display */
	public Text healthOutput;
	private int _health = GameManagerScript.INITIAL_HEALTH;
	public int max_health = GameManagerScript.INITIAL_HEALTH;

	public int health {
		get {
			return _health;
		}
		set {
			_health = value;
			if (healthOutput != null)
				healthOutput.text = value.ToString();
		}
	}

	// Use this for initialization
	public virtual void Awake () {
		// Set up the dashboard's reference
		if (dashboard != null)
			dashboard.player = this;

		gameManager = Utils.Find<GameManagerScript> (gameManager, "GameManager");

		if (deck == null)
			deck = GetComponent<DeckScript> ();

		// Create a basic deck to start with
		for (int i = 0; i < Card.cards.Length; i ++)
			deck.AddCard(Card.cards[i].Clone());

		deck.Shuffle ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public virtual void DrawCard(Card card) {
		hand.AddCard (card);
	}

	public virtual void PlayAugmentation(Card card) {
		this.augmentation = card;
		Discard (card);

		gameManager.PlayedAugmentation (ID);
	}

	public void Discard (Card card) {
		deck.AddCard (card);
		hand.RemoveCard (card);
	}

	public virtual void PlayAction(PlayerAction action) {
		this.action = action;

		gameManager.PlayedAction (ID);
	}

	/* For receiving information from the game state */
	public virtual void Message(GameManagerScript.MESSAGE message, object data = null) {
		switch (message) {
		case GameManagerScript.MESSAGE.DRAW:
			if (deck.length > 0) {
				hand.AddCard (deck.Draw ());
			}
			break;
		}
	}
}

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
	public PlayerSchool school = null;

	/* Health GUI display */
	public int health = GameManagerScript.INITIAL_HEALTH;
	public int max_health = GameManagerScript.INITIAL_HEALTH;

	// Use this for initialization
	public virtual void Awake () {
		// Set up the dashboard's reference
		if (dashboard != null)
			dashboard.player = this;

		// Choose random school. Just kidding, don't do that.
		// school = PlayerSchool.schools [Random.Range (0, 3)].Clone ();

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

	/* Sets the player's school to the given school */
	public virtual void SetSchool(PlayerSchool s) {
		// Set school
		this.school = s;
		// Set initial deck and draw hand
		this.school.GenerateDeck (deck);
		for (int i = 0; i < 3; i++) {
			hand.AddCard (deck.Draw ());
		}

		gameManager.SchoolSelected (ID);
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

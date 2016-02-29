using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour {

	public GameManagerScript gameManager;
	public CardHolderScript hand;
	public DeckScript deck;
	public ActionDisplayScript actionDisplay;
	public int ID;

	private int numActions = 0;
	private List<Card> currentAction;

	public Text healthOutput;
	private int _health = 20;

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
	public virtual void Start () {
		gameManager = Utils.Find<GameManagerScript> (gameManager, "GameManager");

		if (deck == null)
			deck = GetComponent<DeckScript> ();

		currentAction = new List<Card> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public virtual void DrawCard(Card card) {
		hand.AddCard (card);
	}

	public virtual void PlayCard(Card card) {
		currentAction.Add (card);
		deck.AddCard (card);
		hand.RemoveCard (card);

		if (currentAction.Count == numActions) {
			SendAction ();
		}
	}

	protected void SendAction() {
		gameManager.SetAction (ID, currentAction.ToArray ());
		numActions = 0;
	}

	protected bool DoneChoosingActions() {
		return numActions == 0;
	}

	/* For receiving information from the game state */
	public virtual void Message(GameManagerScript.MESSAGE message, object data = null) {
		switch (message) {
		case GameManagerScript.MESSAGE.MAKE_ACTION:
			if (numActions > 0)
				Debug.LogError ("Received a new MAKE_ACTION message when I'm still choosing!");

			numActions = (int)data;
			currentAction.Clear ();
			break;
		case GameManagerScript.MESSAGE.DRAW:
			if (deck.length > 0) {
				hand.AddCard (deck.Draw ());
			}
			break;
		}
	}
}

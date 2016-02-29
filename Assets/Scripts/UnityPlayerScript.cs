using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnityPlayerScript : PlayerScript {

	private List<Card> currentAction;

	// Use this for initialization
	public override void Start () {
		base.Start ();

		hand.OnPickCard = PickCard;
		currentAction = new List<Card> ();
	}

	public bool PickCard(Card card) {
		if (numActions > 0) {
			currentAction.Add (card);

			if (--numActions == 0) {
				gameManager.SetAction (ID, currentAction.ToArray ());
			}

			return true;
		}
		return false;
	}

	/* For receiving information from the game state */
	public override void Message(GameManagerScript.MESSAGE message, object data = null) {
		switch (message) {
		case GameManagerScript.MESSAGE.MAKE_ACTION:
			if (numActions > 0)
				Debug.LogError ("Received a new MAKE_ACTION message when I'm still choosing!");

			numActions = (int)data;
			currentAction.Clear ();
			break;
		}
	}
}

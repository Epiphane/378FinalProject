using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UnityPlayerScript : PlayerScript {

	/* For unity UI */
	public GameObject actions;

	// Use this for initialization
	public override void Awake () {
		base.Awake ();

		hand.OnPickCard = PickCard;
	}

	public void PickCard(Card card) {
		if (gameManager.CanPickAugmentation (ID)) {
			PlayAugmentation (card);
		} else if (gameManager.ShouldDiscard (ID)) {
			Discard (card);

			gameManager.Discarded ();
		}
	}

	public void PlayAction(string action) {
		if (action == "attack") {
			this.PlayAction (PlayerAction.actions [0].Clone ());
		} else if (action == "tech") {
			this.PlayAction (PlayerAction.actions [1].Clone ());
		} else if (action == "counter") {
			this.PlayAction (PlayerAction.actions [2].Clone ());
		} else if (action == "advance") {
			this.PlayAction (PlayerAction.actions [3].Clone ());
		}

		actions.transform.position = new Vector3 (10000, 0, 0);
	}

	/* For receiving information from the game state */
	public override void Message(GameManagerScript.MESSAGE message, object data = null) {
		base.Message (message, data);
		actions.transform.position = new Vector3 (10000, 0, 0);

		switch (message) {
		case GameManagerScript.MESSAGE.CHOOSE_ACTION:
			actions.transform.position = actions.transform.parent.position;
			break;
		}
	}
}

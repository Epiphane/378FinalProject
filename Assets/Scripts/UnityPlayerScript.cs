using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UnityPlayerScript : PlayerScript {

	/* For unity UI */ 
	public GameObject actions;
	public GameObject schools;

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

	public void PlayActionString(string action) {
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

	public void SchoolString(string school) {
		if (school == "aggression") {
			this.SetSchool (PlayerSchool.schools [0].Clone ());
		} else if (school == "tactics") {
			this.SetSchool (PlayerSchool.schools [1].Clone ());
		} else if (school == "focus") { 
			this.SetSchool (PlayerSchool.schools [2].Clone ());
		}

		schools.transform.position = new Vector3 (10000, 0, 0);
	}

	/* For receiving information from the game state */
	public override void Message(GameManagerScript.MESSAGE message, object data = null) {
		base.Message (message, data);
		actions.transform.position = new Vector3 (10000, 0, 0);
		schools.transform.position = new Vector3 (10000, 0, 0);

		switch (message) {
		case GameManagerScript.MESSAGE.CHOOSE_ACTION:
			actions.transform.position = actions.transform.parent.position;
			break;
		case GameManagerScript.MESSAGE.CHOOSE_SCHOOL:
			schools.transform.position = actions.transform.parent.position;
			break;
		}
	}
}

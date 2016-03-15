using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System.Linq;

public class AirConsolePlayerScript : PlayerScript {

    public int device_id;
    public bool ready { get; private set; }

    public void OnMessage(JToken data)
    {
        if (data["ready"] != null) {
			ready = true;
		} else if (data["card"] != null) {
			ChoseCard ((int)data ["card"]);
		} else if (data["attack"] != null) {
			PlayAction (PlayerAction.actions [0].Clone ());
		} else if (data["counter"] != null) {
			PlayAction (PlayerAction.actions [2].Clone ());
		} else if (data["tech"] != null) {
			PlayAction (PlayerAction.actions [1].Clone ());
        } else if (data["advance"] != null) {
			PlayAction (PlayerAction.actions [3].Clone ());
        }
    }

    void ChoseCard(int ndx) {
		if (gameManager.state == GameManagerScript.STATE.WAITING_ON_SCHOOL) {
			SetSchool (PlayerSchool.schools [ndx].Clone ());
		} else if (gameManager.state == GameManagerScript.STATE.WAITING_ON_AUGMENTATION || gameManager.state == GameManagerScript.STATE.WAITING_ON_DISCARDS) {
			if (gameManager.CanPickAugmentation (ID)) {
				PlayAugmentation (this.hand.cards[ndx]);
			}
		} else if (gameManager.state == GameManagerScript.STATE.DRAW_NEW_CARD) {
			gameManager.cardBank.PickCard(this.ID, gameManager.cardBank.cards[ndx]);
        }

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
	}

	public void SchoolString(string school) {
		if (school == "aggression") {
			this.SetSchool (PlayerSchool.schools [0].Clone ());
		} else if (school == "tactics") {
			this.SetSchool (PlayerSchool.schools [1].Clone ());
		} else if (school == "focus") { 
			this.SetSchool (PlayerSchool.schools [2].Clone ());
		}
	}

	/* For receiving information from the game state */
	public override void Message(GameManagerScript.MESSAGE message, object data = null) {
		base.Message (message, data);
	}

	// Airconsole messaging
	public void SyncState() {
		GameManagerScript.STATE state = gameManager.state;

		switch (state) {

		// Player should play a card from their hand. Have the mobile controller
		//  display a list of the player's cards.
		case GameManagerScript.STATE.WAITING_ON_AUGMENTATION:
			SendCards (hand.cards);
			break;
		case GameManagerScript.STATE.DRAW_NEW_CARD:
			SendCards (gameManager.cardBank.cards);
			break;
		case GameManagerScript.STATE.WAITING_ON_ACTION:
			AskPlayerForAction ();
			break;
		case GameManagerScript.STATE.WAITING_ON_DISCARDS:
			SendCards (hand.cards);
			break;
		}
	}

	public void SendCards(List<Card> cards) {
		var result_string = "{ \"newCards\": [ ";
		for (int ndx = 0; ndx < cards.Count; ndx++) {
			var card = cards [ndx];
			// Remove trailing comma, it screws up JSON parsing which is DUMB.
			var separator_comma = (ndx == cards.Count - 1) ? "" : ",";
			var color = Card.ColorToString (card.color);
			result_string += "{\"color\": \"" + color + "\", \"words\": \"" + card.description + "\"  }" + separator_comma;
		}

		result_string += "], ";
	
		if (gameManager.CanPickAugmentation (ID))
			result_string += "\"message\": \"Choose an augmentation!\"";
		else if (gameManager.CanDrawCard (ID))
			result_string += "\"message\": \"Pick a new card to add to your hand!\"";
		else
			result_string += "\"message\": \"Waiting...\"";

		result_string += "}";

		print ("Sending cardstring to " + device_id);

		AirConsole.instance.Message (device_id, result_string);
	}

	public void AskPlayerForAction() {
		AirConsole.instance.Message (device_id, "{ \"doAction\": true, \"message\": \"Choose an action!\" }");
	}

	public void AskPlayerForSchool() {
		AirConsole.instance.Message (device_id, "{ \"chooseSchool\": true, \"message\": \"Choose your school!\" }");
	}
}

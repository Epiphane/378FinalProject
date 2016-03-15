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

	/* Connection status display */
	public GameObject status;

    public void OnMessage(JToken data)
    {
		if (data ["ready"] != null) {
			ready = true;
			Debug.Log ("ready!");

			if (status != null && status.GetComponent<Text> () != null) {
				status.GetComponent<Text> ().text = "Ready!";
			}
		} else if (data ["chose_card0"] != null) {
			// Chose card 0
			ChoseCard (0);
		} else if (data ["chose_card1"] != null) {
			// Chose card 1
			ChoseCard (1);
		} else if (data ["chose_card2"] != null) {
			// Chose card 2
			ChoseCard (2);
		} else if (data ["card"] != null) {
			Debug.Log ("Card: " + data ["card"]);
			ChoseCard ((int)data ["card"]);
		} else if (data["attack"] != null) {
			this.PlayAction (PlayerAction.actions [0].Clone ());
		} else if (data["counter"] != null) {
			this.PlayAction (PlayerAction.actions [2].Clone ());
		} else if (data["tech"] != null) {
			this.PlayAction (PlayerAction.actions [1].Clone ());
		} else if (data["advance"] != null) {
			this.PlayAction (PlayerAction.actions [3].Clone ());
        }
    }

    void ChoseCard(int ndx) {
		if (gameManager.state == GameManagerScript.STATE.WAITING_ON_SCHOOL) {
			SetSchool (PlayerSchool.schools [ndx].Clone ());
		} else if (gameManager.state == GameManagerScript.STATE.WAITING_ON_AUGMENTATION || gameManager.state == GameManagerScript.STATE.WAITING_ON_DISCARDS) {
			if (gameManager.CanPickAugmentation (ID)) {
				PlayAugmentation (this.hand.cards[ndx]);
				SyncState ();
			}
		} else if (gameManager.state == GameManagerScript.STATE.DRAW_NEW_CARD) {
			gameManager.cardBank.PickCard(this.ID, gameManager.cardBank.cards[ndx]);
        }

    }

    void DoAction(int device_id, string action_name) {
        GameManagerScript manager = GameObject.FindObjectOfType<GameManagerScript>();

        ((UnityPlayerScript)manager.players[0]).PlayActionString(action_name);
    }

	/* For receiving information from the game state */
	public override void Message(GameManagerScript.MESSAGE message, object data = null) {
		base.Message (message, data);

		if (message == GameManagerScript.MESSAGE.CHOOSE_SCHOOL) {
			AskPlayerForSchool ();
		}
	}

	// Airconsole messaging
	public void SyncState() {
		if (!gameManager) {
			// Not in game
			return;
		}

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
		case GameManagerScript.STATE.WAITING_ON_SCHOOL:
			AskPlayerForSchool ();
			break;
		}
	}

	public void SendCards(List<Card> cards) {
		var result_string = "{ \"newCards\": [ ";
		for (int ndx = 0; ndx < cards.Count; ndx++) {
			var card = cards [ndx];
			result_string += card.ToJSON ();
			if (ndx < cards.Count - 1)
				result_string += ",";
		}

		result_string += "] }";

		print ("Sending cardstring: " + result_string);

		AirConsole.instance.Message (device_id, result_string);
	}

	public void AskPlayerForAction() {
		AirConsole.instance.Message (device_id, "{ \"doAction\": true }");
	}

	public void AskPlayerForSchool() {
		Debug.Log ("Choose your darn school already!");
		AirConsole.instance.Message (device_id, "{ \"chooseSchool\": true }");
	}

	public void CardWasTaken(int cardNdx) {
		AirConsole.instance.Message (device_id, "{ \"cardWasTaken\": " + cardNdx + " }");
	}
}

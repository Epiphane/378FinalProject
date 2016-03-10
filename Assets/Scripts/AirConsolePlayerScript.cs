using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using System.Linq;

public class AirConsolePlayerScript : PlayerScript {

    public int device_id;
    public bool ready { get; private set; }

    public void OnMessage(JToken data)
    {
        if (data["ready"] != null) {
            ready = true;
            Debug.Log("ready!");
        } else if (data["chose_card0"] != null) {
            // Chose card 0
            ChoseCard(device_id, 0);
        } else if (data["chose_card1"] != null) {
            // Chose card 1
            ChoseCard(device_id, 1);
        } else if (data["chose_card2"] != null) {
            // Chose card 2
            ChoseCard(device_id, 2);
        } else if (data["attack"] != null) {
            DoAction(device_id, "attack");
        } else if (data["counter"] != null) {
            DoAction(device_id, "counter");
        } else if (data["tech"] != null) {
            DoAction(device_id, "tech");
        } else if (data["advance"] != null) {
            DoAction(device_id, "advance");
        }
    }

    void ChoseCard(int device_id, int ndx) {
        GameManagerScript manager = GameObject.FindObjectOfType<GameManagerScript>();

        if (manager.state == GameManagerScript.STATE.WAITING_ON_AUGMENTATION || manager.state == GameManagerScript.STATE.WAITING_ON_DISCARDS) {
            Card selectedCard = manager.players[0].hand.cards[ndx];
            manager.players[0].hand.CardSelected(null, selectedCard);
        } else if (manager.state == GameManagerScript.STATE.DRAW_NEW_CARD) {
            Card selectedCard = manager.cardBank.cards[ndx];
            manager.cardBank.PickCard(0, selectedCard);
        }

    }

    void DoAction(int device_id, string action_name) {
        GameManagerScript manager = GameObject.FindObjectOfType<GameManagerScript>();

        ((UnityPlayerScript)manager.players[0]).PlayActionString(action_name);
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
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System.Linq;

public class AirconsoleLogic : MonoBehaviour {

	// Key is the device_id from AirConsole, value is the resulting PlayerScript object
	public Dictionary<int, UnityPlayerScript> activePlayers = new Dictionary<int, UnityPlayerScript>();

	// How many players are currently connected to the game?
	public static int numPlayers = 0;

	void Awake() {
		AirConsole.instance.onMessage += OnMessage;
		AirConsole.instance.onConnect += OnConnect;
		AirConsole.instance.onDisconnect += OnDisconnect;
	}

	public static int player0_id = -1;
	public static int player1_id = -1;


	void Start() {
		if (AirConsole.instance.IsAirConsoleUnityPluginReady ()) {
			List<int> ids = AirConsole.instance.GetControllerDeviceIds ();

			ids.ForEach ((device_id) => {
				OnConnect (device_id);
			});
		}
	}
		
	void OnConnect(int device_id) {
		numPlayers++;

		if (numPlayers == 1) {
			player0_id = device_id;
		} else if (numPlayers == 2) {
			player1_id = device_id;
		} else if (numPlayers > 2) {
			// TODO: A third (or later) player tried to join. Tell them to wait, maybe have a "honk" button lawl
		}


		SyncState ();
	}

	// Called when a controller disconnects. Pause the game if we don't
	//  have enough players to continue.
	void OnDisconnect(int device_id) {
		numPlayers--;

		if (device_id == player0_id) {
			// Player 0 left
			player0_id = -1;
		} else if (device_id == player1_id) {
			// Player 1 left
			player1_id = -1;
		}

		// TODO: logic here for if too many people have left
	}

	void ChoseCard(int device_id, int ndx) {
		GameManagerScript manager = GameObject.FindObjectOfType<GameManagerScript>();

		if (manager.state == GameManagerScript.STATE.WAITING_ON_AUGMENTATION || manager.state == GameManagerScript.STATE.WAITING_ON_DISCARDS) {
			if (device_id == player0_id) {
				Card selectedCard = manager.players [0].hand.cards [ndx];
				manager.players [0].hand.CardSelected (null, selectedCard);
			} else if (device_id == player1_id) {
				Card selectedCard = manager.players [1].hand.cards [ndx];
				manager.players [1].hand.CardSelected (null, selectedCard);
			}
		} else if (manager.state == GameManagerScript.STATE.DRAW_NEW_CARD) {
			if (device_id == player0_id) {
				Card selectedCard = manager.cardBank.cards[ndx];
				manager.cardBank.PickCard (0, selectedCard);
			} else if (device_id == player1_id) {
				Card selectedCard = manager.cardBank.cards[ndx];
				manager.cardBank.PickCard (1, selectedCard);
			}
		}
			
	}

	void DoAction(int device_id, string action_name) {
		GameManagerScript manager = GameObject.FindObjectOfType<GameManagerScript>();

		if (device_id == player0_id) {
			((UnityPlayerScript)manager.players [0]).PlayActionString (action_name);
		} else if (device_id == player1_id) {
			((UnityPlayerScript)manager.players [1]).PlayActionString (action_name);
		}
	}

	// Process a message sent from one of the controllers
	void OnMessage(int device_id, JToken data) {
		// TODO: pass in state and verify it here
		print (data);
		if (data ["chose_card0"] != null) {
			// Chose card 0
			ChoseCard(device_id, 0);
		} else if (data ["chose_card1"] != null) {
			// Chose card 1
			ChoseCard(device_id, 1);
		} else if (data ["chose_card2"] != null) {
			// Chose card 2
			ChoseCard(device_id, 2);
		} else if (data ["attack"] != null) {
			DoAction (device_id, "attack");
		} else if (data ["counter"] != null) {
			DoAction (device_id, "counter");
		} else if (data ["tech"] != null) {
			DoAction (device_id, "tech");
		} else if (data ["advance"] != null) {
			DoAction (device_id, "advance");
		}
	}

	public static void AskPlayerForAction(int player) {
		AirConsole.instance.Message (PlayerNum_to_id(player), "{ \"doAction\": true }");
	}

	public static int PlayerNum_to_id(int player) {
		var id = 0;
		if (player == 0) {
			id = player0_id;
		} else {
			id = player1_id;
		}
		return id;
	}

	public static void SendCards(int player, List<Card> cards) {
		var id = PlayerNum_to_id (player);

		var result_string = "{ \"newCards\": [ ";
		for (int ndx = 0; ndx < cards.Count; ndx++) {
			var card = cards [ndx];
			// Remove trailing comma, it screws up JSON parsing which is DUMB.
			var separator_comma = (ndx == cards.Count - 1) ? "" : ",";
			var color = Card.ColorToString (card.color);
			result_string += "{\"color\": \"" + color + "\", \"words\": \"" + card.description + "\"  }" + separator_comma;
		}

		result_string += "] }";

		print ("Sending cardstring: " + result_string);

		AirConsole.instance.Message (id, result_string );
	}

	public static void CardWasTaken(int player, int cardNdx) {
		var id = PlayerNum_to_id (player);

		AirConsole.instance.Message (id, "{ \"cardWasTaken\": " + cardNdx + " }");
	}

	/**
	 * Takes whatever is going on in the central game, and spits out
	 *  messages to all the AirConsole controllers, tellin 'em what to do.
	 */
	public static void SyncState() {
		if (!AirConsole.instance.IsAirConsoleUnityPluginReady ()) {
			// Controllers haven't loaded yet. FORGET ABOUT IT!!!
			return;
		}

		GameManagerScript manager = GameObject.FindObjectOfType<GameManagerScript>();

		PlayerScript[] players = manager.players;

		foreach (var player in players) {
			if (player is UnityPlayerScript) {

				switch (manager.state) {

				// Player should play a card from their hand. Have the mobile controller
				//  display a list of the player's cards.
				case GameManagerScript.STATE.WAITING_ON_AUGMENTATION:
					SendCards (player.ID, player.hand.cards);
					print ("Sending " + player.ID + " the cards " + player.hand.cards.Count);
					break;
				case GameManagerScript.STATE.DRAW_NEW_CARD:
					SendCards (player.ID, manager.cardBank.cards);
					print ("flopping " + player.ID + " the cards " + manager.cardBank.cards.Count);
					break;
				case GameManagerScript.STATE.WAITING_ON_ACTION:
					AskPlayerForAction (player.ID);
					break;
				case GameManagerScript.STATE.WAITING_ON_DISCARDS:
					SendCards (player.ID, player.hand.cards);
					break;
				}

			}
		}
	}


	void OnDestroy() {

		// unregister airconsole events on scene change
		if (AirConsole.instance != null) {
			AirConsole.instance.onMessage -= OnMessage;
			AirConsole.instance.onDisconnect -= OnDisconnect;
			AirConsole.instance.onConnect -= OnConnect;
		}
	}

}

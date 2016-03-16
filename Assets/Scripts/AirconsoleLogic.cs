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
	public static Dictionary<int, AirConsolePlayerScript> activePlayers = new Dictionary<int, AirConsolePlayerScript>();

	// How many players are currently connected to the game?
	public static int numPlayers = 0;
    public GameObject emptyAirconsolePlayer;

	void Awake() {
		AirConsole.instance.onMessage += OnMessage;
		AirConsole.instance.onConnect += OnConnect;
		AirConsole.instance.onDisconnect += OnDisconnect;

        // See if any AirConsoleLogic objects already exist
        if (GameObject.FindGameObjectsWithTag("AirConsoleLogic").Count<GameObject>() > 1)
        {
            // RIP
            Destroy(gameObject);
        }
        else
        {
            // Be Persistent
            DontDestroyOnLoad(gameObject);
        }
    }
		
	public static bool[] skip = new bool[2];
	public static int[] player_ids = new int[2];
	public static AirConsolePlayerScript[] players = new AirConsolePlayerScript[2];

	void Start() {
		if (AirConsole.instance.IsAirConsoleUnityPluginReady ()) {
			List<int> ids = AirConsole.instance.GetControllerDeviceIds ();

			ids.ForEach ((device_id) => {
        		OnConnect (device_id);
			});
		}

        Debug.Log(SceneManager.GetActiveScene().name);
	}
		
	void OnConnect(int device_id) {
		numPlayers++;

        // Assign to player dictionary
        GameObject newPlayerGO = GameObject.Instantiate(emptyAirconsolePlayer);

        AirConsolePlayerScript newPlayer = newPlayerGO.GetComponent<AirConsolePlayerScript>();
        newPlayer.device_id = device_id;
        activePlayers[device_id] = newPlayer;

		GameObject status = null;

		if (numPlayers == 1) {
			player_ids [0] = device_id;
			players [0] = newPlayer;
			players [0].status = status = GameObject.Find ("P1 Status");
		} else if (numPlayers == 2) {
			player_ids [1] = device_id;
			players [1] = newPlayer;
			players [1].status = status = GameObject.Find ("P2 Status");
        } else if (numPlayers > 2) {
            // TODO: A third (or later) player tried to join. Tell them to wait, maybe have a "honk" button lawl
        }

		if (status != null) {
			Text text = status.GetComponent<Text> ();

			if (text != null)
				text.text = "Connected";

			AirConsole.instance.Message(device_id, "{\"message\": \"Connected as player " + numPlayers + "!\"}");
		}

		SyncState ();
	}

	public bool IsReady() {
		return (players [0] != null && players [1] != null) && (players [0].ready && players [1].ready);
	}

	// Called when a controller disconnects. Pause the game if we don't
	//  have enough players to continue.
	void OnDisconnect(int device_id) {
		numPlayers--;

        // TODO: How do we handle people who leave?
		/*if (device_id == player0_id) {
			// Player 0 left
			player0_id = -1;
		} else if (device_id == player1_id) {
			// Player 1 left
			player1_id = -1;
		}*/

		// TODO: logic here for if too many people have left
	}

	// Process a message sent from one of the controllers
	void OnMessage(int device_id, JToken data) {
        if (activePlayers[device_id] != null)
            activePlayers[device_id].OnMessage(data);

		if (data ["skip"] != null) {
			if (device_id == players [0].device_id)
				skip [0] = true;
			if (numPlayers < 2 || device_id == players [1].device_id)
				skip [1] = true;
		}

		if (data ["replay"] != null)
			SceneManager.LoadScene ("versusAI");
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

		foreach (AirConsolePlayerScript player in players) {
			if (player != null)
				player.SyncState ();
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

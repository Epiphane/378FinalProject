using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System.Linq;

public class AirconsoleLogic : MonoBehaviour {
	
//	public Dictionary<int, PlayerMovement> activePlayers = new Dictionary<int, PlayerMovement>();

	void Awake() {
		AirConsole.instance.onMessage += OnMessage;
		AirConsole.instance.onConnect += OnConnect;
		AirConsole.instance.onDisconnect += OnDisconnect;
	}

	void Start() {
		if (AirConsole.instance.IsAirConsoleUnityPluginReady ()) {
			List<int> ids = AirConsole.instance.GetControllerDeviceIds ();

			ids.ForEach ((device_id) => {
				OnConnect (device_id);
				// TODO: lobby logic
//				if (activePlayers.ContainsKey(device_id)) {
//					AirConsole.instance.Message(device_id, "{\"controller\":true}");
//				}
//				else {
//					AirConsole.instance.Message(device_id, "{\"lobby\":true}");
//				}
			});
		}
	}


	/// <summary>
	/// We start the game if 2 players are connected and the game is not already running (activePlayers == null).
	/// 
	/// NOTE: We store the controller device_ids of the active players. We do not hardcode player device_ids 1 and 2,
	///       because the two controllers that are connected can have other device_ids e.g. 3 and 7.
	///       For more information read: http://developers.airconsole.com/#/guides/device_ids_and_states
	/// 
	/// </summary>
	/// <param name="device_id">The device_id that connected</param>
	void OnConnect(int device_id) {
//		if (possibleColors.Count == 0) {
			// No more space! Gotta wait brooooo
			//			AirConsole.instance.Message (device_id, "{\"lobby\":true}");
//			return;
//		}



//		AirConsole.instance.Message (device_id, ColorToJSONMessage(possibleColors[0]));
//		AirConsole.instance.Message (device_id, "{\"controller\":true}");
	}

	/// <summary>
	/// If the game is running and one of the active players leaves, we reset the game.
	/// </summary>
	/// <param name="device_id">The device_id that has left.</param>
	void OnDisconnect(int device_id) {
	}

	/// <summary>
	/// We check which one of the active players has moved the paddle.
	/// </summary>
	/// <param name="from">From.</param>
	/// <param name="data">Data.</param>
	void OnMessage(int device_id, JToken data) {
		int active_player = AirConsole.instance.ConvertDeviceIdToPlayerNumber(device_id);

		print (data);

		if (data ["chose_card1"] != null) {
			print("CHOSE CARD 1, DUH!");
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

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using NDream.AirConsole;

public class LobbyManagerScript : MonoBehaviour {

	private AirconsoleLogic airconsole;

	// Use this for initialization
	void Start () {
		airconsole = GameObject.FindObjectOfType<AirconsoleLogic> ();
	
	}
	
	// Update is called once per frame
	void Update () {
		if (airconsole.IsReady ()) {
			foreach (var player in AirconsoleLogic.players) {
				AirConsole.instance.Message (player.device_id, "{\"skip\":true}");
			}
			SceneManager.LoadScene ("Backstory");
		}
	}

	public void SinglePlayer() {
		SceneManager.LoadScene ("versusAI");
	}
}

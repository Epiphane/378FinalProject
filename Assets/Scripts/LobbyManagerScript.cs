using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LobbyManagerScript : MonoBehaviour {

	private AirconsoleLogic airconsole;

	// Use this for initialization
	void Start () {
		airconsole = GameObject.FindObjectOfType<AirconsoleLogic> ();
	
	}
	
	// Update is called once per frame
	void Update () {
		if (airconsole.IsReady ()) {
			SceneManager.LoadScene ("versusAI");
		}
	}
}

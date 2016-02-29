using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameOverWinnerScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		switch (PlayerPrefs.GetInt ("Winner")) {
		case -1:
			GetComponent<Text> ().text = "Tie!";
			break;
		case 1:
			GetComponent<Text> ().text = "You win!";
			break;
		case 2:
			GetComponent<Text> ().text = "You lose!";
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

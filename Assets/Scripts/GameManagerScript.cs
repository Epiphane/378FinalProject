using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManagerScript : MonoBehaviour {

	public CardBankScript cardBank;

	public PlayerScript[] players;

	// Use this for initialization
	void Start () {
		Flop ();
	}

	void Flop() {
		cardBank.Flop (3);
	}

	public void DrawCard(int player, Card card) {
		players [player].DrawCard (card);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

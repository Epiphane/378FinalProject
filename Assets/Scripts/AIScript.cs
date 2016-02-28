using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIScript : PlayerScript {

	public CardBankScript cardBank;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void PickCard(CardBankScript cardBank) {
		List<Card> options = cardBank.GetAvailableCards ();

		cardBank.PickCard (ID, options [Random.Range (0, options.Count)]);
	}
}

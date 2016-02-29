﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIScript : PlayerScript {
	
	public CardBankScript cardBank;

	private static int THINK_TIME = 1;

	// Use this for initialization
	public override void Start () {
		base.Start ();

		if (cardBank == null) {
			GameObject CB = GameObject.Find ("CardBank");

			if (CB == null) {
				Debug.LogError ("CardBank not found!");
				return;
			}

			cardBank = CB.GetComponent<CardBankScript> ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void PickCard() {
		List<Card> options = cardBank.GetAvailableCards ();

		cardBank.PickCard (ID, options [Random.Range (0, options.Count)]);
	}

	/* Pick a card to play */
	public void DoAction() {
		while (!DoneChoosingActions()) {
			int index = Random.Range (0, hand.Size);
			PlayCard(hand.cards [index]);
		}
	}

	/* For receiving information from the game state */
	public override void Message(GameManagerScript.MESSAGE message, object data) {
		base.Message (message, data);

		switch (message) {
		case GameManagerScript.MESSAGE.DRAW_NEW_CARD:
			// Think for a bit before picking a random card
			// ALSO: You can't directly call PickCard() from here because itll get all weird on ya
			// Ask Thomas if you want to know why, its gross
			Invoke ("PickCard", THINK_TIME);
			break;
		case GameManagerScript.MESSAGE.MAKE_ACTION:
			// See above
			Invoke ("DoAction", THINK_TIME);
			break;
		}
	}
}

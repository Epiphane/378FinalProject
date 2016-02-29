using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnityPlayerScript : PlayerScript {

	// Use this for initialization
	public override void Start () {
		base.Start ();

		hand.OnPickCard = PickCard;
	}

	public void PickCard(Card card) {
		if (!DoneChoosingActions ()) {
			PlayCard (card);
		}
	}
}
